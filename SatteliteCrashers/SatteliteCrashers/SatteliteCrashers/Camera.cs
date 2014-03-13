using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SatteliteCrashers
{
    /// <summary>
    /// DOES NOT SUPPORT ROTATION
    /// </summary>
    public class Camera
    {
        public Matrix view;
        Vector3 velocity;
        Vector3 position;
        Random rand = new Random();
        float lastHorizMag;
        float lastVertMag;

        public Camera(Vector3 pos)
        {
            view = Matrix.CreateLookAt(pos, pos + new Vector3(0,1,0), new Vector3(0,0,1));
            this.position = pos;
        }

        //public void update(Vector2 dp, float dr)
        //{
        //    rotation += dr;
        //    view *= Matrix.CreateRotationY(dr) * Matrix.CreateTranslation(Utils.getVec3(dp, 0));
        //}


        internal void update(Vector3 pos, Vector3 vel, bool inflight, float rot)
        {
            //weighted avg of cam vel and shreder vel
            velocity = (10 * velocity + vel) / 11;
            position += velocity;

            //Vector3 lookat = Vector3.Normalize(velocity);
            //lookat = Vector3.Transform(lookat, body.Orientation);
            if (inflight)
            {

 
                //int sign = Math.Sign(vel.Y);
                //float horizComp = 5 * (-Math.Abs(vel.Y) + .1f);
                //if (horizComp < 0)
                //    horizComp = 0;
                //position = pos - 15 * vel.Y * new Vector3(0, 1, 0) + horizComp * new Vector3(1, 0, 0);

                float horizMag = 5;
                if (vel.Y > 0)
                    horizMag -= Math.Min(5, 5*(30 * vel.Y * vel.Y));

                horizMag = (10*lastHorizMag + horizMag) / 11;
                lastHorizMag = horizMag;
                

                float vertMag = 10;
                vertMag *= (float)Math.Sqrt(10 * Math.Abs(vel.Y)) / 10;

                if (Data.player.controlManager.jump)
                    vertMag *= .7f;

                vertMag = (5 * lastVertMag + vertMag) / 6;
                lastVertMag = vertMag;

                Vector3 horizComp = horizMag * new Vector3(-1, 0, 0);
                Vector3 vertComp = vertMag * new Vector3(0, -Math.Sign(vel.Y) * 10, 0);

                Vector3 randcomp=Vector3.Zero;
                
                if (Math.Sign(velocity.Y) == 1)
                    randcomp = new Vector3(1 * (float)rand.NextDouble(), 0, 3 * (float)rand.NextDouble()) * vel.Y / 10f;
                

                Vector3 camPos = pos + horizComp + vertComp+randcomp;
                Vector3 dif = pos - camPos;
                //dif.Y = Math.Abs(dif.Y);

                Vector3 up = Vector3.Cross(dif, new Vector3(1, 0, 0));
                view = Matrix.CreateLookAt(camPos, pos, -up);

                //view *= Matrix.CreateRotationY(rot);
            }
            else
            {
                view = Matrix.CreateLookAt(pos - 15 * new Vector3(0, 1, 0), pos, new Vector3(0, 0, 1));
            }
        }
    }
}
