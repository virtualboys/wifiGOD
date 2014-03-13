using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace SatteliteCrashers
{
    public class Player
    {

        public Vector3 position;
        public Vector3 velocity;
        public Vector3 rotation;
        Vector3 angVel;

        public Timer phoneAnimTimer;
        public int phoneFrame;

        public Camera camera;
        public ControlManager controlManager;

        Model model;
        Matrix world;
        SoundEffect sound;
        SoundEffectInstance instance;

        SoundEffect wind;
        SoundEffectInstance windInstance;

        bool inflight = true;

        int cyclesToSlow;
        bool slowing;

        public StringBuilder triedCode;
        public bool markedClear;

        public Texture2D phoneCurT;
        public List<Texture2D> phone;
        public List<Texture2D> phoneInputs;
        public List<Texture2D> phoneEnter;
        public float phoneScale = .3f;
        public float phonePos;
        public float phoneVel;
        public bool searching = true;
        public int currNumF = 10;
        bool doneStartAnim;

        SoundEffect yesButton;
        SoundEffect noButton;
        SoundEffect search;
        SoundEffectInstance searchInst;
        SoundEffect found;

        public SoundEffect success;

        public Player(Vector3 position, Model model, SoundEffect sound, SoundEffect wind, List<Texture2D> phone, List<Texture2D> phoneInputs, 
            List<Texture2D> phoneEnter, SoundEffect y, SoundEffect n, SoundEffect search, SoundEffect find, SoundEffect success)
        {
            this.yesButton = y;
            this.noButton = n;
            this.search = search;
            this.success = success;
            this.found = find;
            this.phone = phone;
            this.phoneInputs = phoneInputs;
            this.phoneEnter = phoneEnter;
            phoneCurT = phone.First();
            this.sound = sound;
            this.wind = wind;
            camera = new Camera(position);

            controlManager = new ControlManager(0);

            this.model = model;
            this.position = position;
            rotation.Y = MathHelper.Pi;
            rotation.Z = -MathHelper.Pi;
            rotation.X = MathHelper.Pi;

            triedCode = new StringBuilder();

            phoneAnimTimer = new Timer(100, true, phoneAdvFrame);
            phoneAnimTimer.Start();
        }

        private void phoneAdvFrame()
        {
            phoneFrame++;
            if (phoneFrame >= currNumF)
            {
                phoneFrame = 0;

                if (currNumF == 4)
                    doneStartAnim = true;
            }
        }

        public void update(GameTime gameTime)
        {
            phoneAnimTimer.Update(gameTime);
            tryHack();
            controlManager.update();

            if (!controlManager.jump)
                velocity += new Vector3(0, .003f, 0);
            else
                velocity += new Vector3(0, .0003f, 0);

            float loudness = Math.Min(1,velocity.Length() - .4f + .003f * (-position.Y - 300) );
            if (loudness > 0)
            {
                
                if (windInstance == null)
                {
                    windInstance = wind.CreateInstance();
                    windInstance.Volume = loudness;
                    windInstance.IsLooped = true;
                    windInstance.Play();

                }
                else
                {
                    windInstance.Volume = loudness*.3f;
                    windInstance.Pitch = Math.Min(1,Math.Max(-1,loudness - 1 - (300+position.Y) * .001f)) ;
                }
            }
            else
            {

            }

            Vector3 normal = Data.ramp.getNormal(ref position);

            if (normal != Vector3.Zero)
            {
                if (instance == null)
                {
                    instance = sound.CreateInstance();
                    instance.Volume = 1;
                    instance.Play();
                }

                inflight = false;

                velocity *= 1.01f;
                float angle = -(float)Math.Acos(Vector3.Dot(normal, velocity) / velocity.Length());
                angle -= Math.Sign(angle) * MathHelper.PiOver2;
                Vector3 axis = Vector3.Cross(normal, velocity);
                axis.Normalize();

                velocity = rotateVec(axis, velocity, angle);
                velocity = rotateVec(new Vector3(0, 1, 0), velocity, angVel.Y);
 
                float horizVel = (float)Math.Sqrt(velocity.X * velocity.X + velocity.Z * velocity.Z);

                rotation.X = -(float)Math.Atan2(horizVel, velocity.Y) + MathHelper.Pi;
                rotation.Y = (float)Math.Atan2(velocity.X, velocity.Z) - MathHelper.Pi;

            }
            else if(position.Y < Data.ramp.position.Y + Data.ramp.top)
            {
                inflight = true;
                if (instance != null)
                {
                    instance.Dispose();
                    instance = null;
                }
            }

            position += velocity;

            input();

            rotation += angVel;

            //if(position.Y < 8)
            camera.update(position, velocity, inflight, rotation.Y);

            world = Matrix.Identity;
            //world *= Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
            ////world *= Matrix.pis

            clamp(ref rotation.X);
            clamp(ref rotation.Y);
            clamp(ref rotation.Z);

            if(inflight)
            {
                
                world *= Matrix.CreateRotationY(rotation.Y);
                world *= Matrix.CreateRotationX(rotation.X);
            }
            else
            {
                world *= Matrix.CreateRotationX(rotation.X);
                world *= Matrix.CreateRotationY(rotation.Y);
            }


            world *= Matrix.CreateTranslation(position + new Vector3(0,-2,0));


            phonePos += phoneVel;
            if (phonePos > phone.First().Height*phoneScale)
                phonePos = phone.First().Height*phoneScale;
            else if (phonePos < 150)
                phonePos = 150;

            bool wasSearching = searching;
            searching = position.Y > -500;
            if (wasSearching != searching)
            {
                phoneFrame = 0;
                doneStartAnim = false;
            }

            if (searching)
            {
                currNumF = 10;

                if (searchInst == null)
                {

                    searchInst = search.CreateInstance();
                    searchInst.IsLooped = true;
                    //searchInst.Volume = .5f;
                    searchInst.Play();
                }
            }
            else
            {
                currNumF = 4;

                if (searchInst != null)
                {
                    found.Play();
                    searchInst.Stop();
                    searchInst = null;
                }
            }

            if (currNumF == 10)
            {
                phoneCurT = phone[phoneFrame];
            }
            else
            {
                if (doneStartAnim)
                {
                    phoneCurT = phoneInputs[triedCode.Length];
                }
                else
                {
                    phoneCurT = phoneEnter[phoneFrame];
                }
            }

        }

        public void input()
        {
            if (controlManager.dir1.X != 0)
            {
                //int sign = Math.Sign(rotation.X);
                //if (sign == 0)
                //    sign = 1;
                //angVel.Y += sign * controlManager.dir1.X / 300;
                angVel.Y += -controlManager.dir1.X / 300;

                float threshold = .1f;
                if (!inflight)
                    threshold = .2f;
                if (Math.Abs(angVel.Y) > threshold)
                    angVel.Y = Math.Sign(angVel.Y) * threshold;
            }
            else
            {
                angVel.Y *= .95f;
                if (Math.Abs(angVel.Y) < .001f)
                    angVel.Y = 0;

            }

            if (controlManager.dir1.Y != 0)
            {
                angVel.X += controlManager.dir1.Y / 300;
                if (Math.Abs(angVel.X) > .1f)
                    angVel.X = Math.Sign(angVel.X) * .1f;
            }
            else
            {
                angVel.X *= .95f;
                if (Math.Abs(angVel.X) < .001f)
                    angVel.X = 0;

            }

            if (inflight && controlManager.jump)
            {
                strafe();

                slow();
            }
            else if (!inflight && controlManager.newjump)
            {
                inflight = true;
                velocity = new Vector3(0, -1, 0) * velocity.Length();
            }
            else if (inflight)
            {
                velocity.X *= .98f;
                velocity.Z *= .98f;

                cyclesToSlow = 0;
                slowing = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                phoneVel = 15;
            else
                phoneVel = -15;
        }

        public void tryHack()
        {
            if (!withinRange())
                return;

            if (KeyboardManager.currentPress != '\0')
            {
                //if (markedClear)
                //{
                //    triedCode.Clear();
                //    markedClear = 
                //}

                triedCode.Append(KeyboardManager.currentPress);

                if (triedCode.Length > Data.sattelite.code.Length || KeyboardManager.currentPress != Data.sattelite.code.ElementAt(triedCode.Length - 1))
                {
                    triedCode.Clear();
                    noButton.Play();
                }
                else
                    yesButton.Play();
            }
        }

        public bool withinRange()
        {
            return position.Y < -500;
        }

        public void strafe()
        {
            if (Game1.markforwin != 0)
                return;
            Vector3 down = new Vector3(0, 1, 0);
            Vector3 pointing = down;
            pointing = Vector3.Transform(pointing, Matrix.CreateRotationX(rotation.X));

            float fac1 = 4;
            float fac2 = 2;

            float desX = Vector3.Transform(down, Matrix.CreateRotationZ(-rotation.Y)).X / (fac1 * Math.Max(1, 100 * angVel.Length()));

            velocity.X += (desX - velocity.X) / 20;

            
            float desZ = (float) Math.Abs(Vector3.Cross(pointing,down).Length()) * Vector3.Dot(pointing, down) / (fac2 * Math.Max(1, 100 * angVel.Length()));

            if (rotation.X > MathHelper.Pi)
                desZ = -desZ;

            velocity.Z += (desZ - velocity.Z) / 5;
        }

        public void slow()
        {
            if (cyclesToSlow == 0 && !slowing)
            {
                cyclesToSlow = 50;
                slowing = true;
            }
            else if (cyclesToSlow != 0)
                cyclesToSlow--;

            if (velocity.Y > 0 && cyclesToSlow != 0)
                velocity.Y *= 1f - (50 - cyclesToSlow) * .001f;
        }

        public void render(RenderHelper renderHelper)
        {
            renderHelper.renderModel(model, world, Data.diffColor, new Vector3(0.997f, 0.377f, 0.787f), .5f * new Vector3(0.997f, 0.377f, 0.787f));
        }

        public void clamp(ref float angle)
        {
            if (angle > Math.PI * 2f)
            {
                angle -= (float)Math.PI * 2f;
            }
            if (angle < 0)
            {
                angle = (float)Math.PI * 2 + angle;
            }
        }

        public Vector3 rotateVec(Vector3 axis, Vector3 vec, float angle)
        {
            return vec * (float)Math.Cos(angle) + Vector3.Cross(axis, vec) * (float)Math.Sin(angle)
                    + axis * Vector3.Dot(axis, vec) * (1 - (float)Math.Cos(angle));
        }
    }
}
