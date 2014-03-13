using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public delegate void VoidDel();

    public struct Keyset
    {
        public Keys up;
        public Keys down;
        public Keys left;
        public Keys right;
        public Keys jump;
        public Keys attack1;
        public Keys attack2;
        public Keys action1;
        public Keys action2;
        public Keys alt1;
        public Keys alt2;

        public Keyset(Keys up, Keys down, Keys left, Keys right, Keys jump, Keys attack1, Keys attack2, Keys action1, Keys action2, Keys alt1, Keys alt2)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
            this.jump = jump;
            this.attack1 = attack1;
            this.attack2 = attack2;
            this.action1 = action1;
            this.action2 = action2;
            this.alt1 = alt1;
            this.alt2 = alt2;
        }
    }

    public class ControlManager
    {
        public static int keyboardCounter = 0;

        public VoidDel update;
        Keyset keyset;
        int player;

        public Vector2 dir1;
        public Vector2 dir2;
        public bool jump;
        public bool attack1;
        public bool attack2;
        public bool action1;
        public bool action2;
        public bool alt1;
        public bool alt2;

        public bool newjump;
        public bool newattack1;
        public bool newattack2;
        public bool newaction1;
        public bool newaction2;
        public bool newalt1;
        public bool newalt2;

        public ControlManager(int player)
        {
            this.player = player;
            var gs = GamePad.GetState((PlayerIndex)player);

            if (gs.IsConnected)
                update = updateGamePad;
            else
            {
                update = updateKeyboard;
                keyset = getKeys(keyboardCounter++);
            }
        }

        public void updateKeyboard()
        {
            newjump = false;
            newattack1 = false;
            newattack2 = false;
            newaction1 = false;
            newaction2 = false;
            newalt1 = false;
            newalt2=false;


            var kbs = Keyboard.GetState();

            dir1 = Vector2.Zero;
            dir2 = Vector2.Zero;

            if(kbs.IsKeyDown(keyset.left))
            {
                dir1.X = -1;
            }
            else if(kbs.IsKeyDown(keyset.right))
            {
                dir1.X = 1;
            }
            if(kbs.IsKeyDown(keyset.up))
            {
                dir1.Y = 1;
            }
            else if(kbs.IsKeyDown(keyset.down))
            {
                dir1.Y = -1;
            }

            if(dir1 != Vector2.Zero)
                dir1.Normalize();

            if (kbs.IsKeyDown(keyset.jump))
            {
                if (!jump)
                    newjump = true;

                jump = true;
            }
            else
                jump = false;

            if (kbs.IsKeyDown(keyset.attack1))
            {
                if (!attack1)
                    newattack1 = true;

                attack1 = true;
            }
            else
                attack1 = false;

            if (kbs.IsKeyDown(keyset.attack2))
            {
                if (!attack2)
                    newattack2 = true;

                attack2 = true;
            }
            else
                attack2 = false;

            if (kbs.IsKeyDown(keyset.action1))
            {
                if (!action1)
                    newaction1 = true;

                action1 = true;
            }
            else
                action1 = false;

            if (kbs.IsKeyDown(keyset.action2))
            {
                if (!action2)
                    newaction2 = true;

                action2 = true;
            }
            else
                action2 = false;

            if (kbs.IsKeyDown(keyset.alt1))
            {
                if (!alt1)
                    newalt1 = true;
                alt1 = true;
                dir2.X = -1;
            }
            else
                alt1 = false;

            if (kbs.IsKeyDown(keyset.alt2))
            {
                if (!alt2)
                    newalt2 = true;
                alt2 = true;
                dir2.X = 1;
            }
            else
                alt2 = false;

            
        }

        public void updateGamePad()
        {
            newjump = false;
            newattack1 = false;
            newattack2 = false;
            newaction1 = false;
            newaction2 = false;

            var gps = GamePad.GetState((PlayerIndex)player);

            dir1 = gps.ThumbSticks.Left;
            dir2 = gps.ThumbSticks.Right;

            if (gps.IsButtonDown(Buttons.A))
            {
                if (!jump)
                    newjump = true;

                jump = true;
            }
            else
                jump = false;

            if (gps.Triggers.Right > 0)
            {
                if (!attack1)
                    newattack1 = true;

                attack1 = true;
            }
            else
                attack1 = false;

            if (gps.IsButtonDown(Buttons.RightShoulder))
            {
                if (!attack2)
                    newattack2 = true;

                attack2 = true;
            }
            else
                attack2 = false;

            if (gps.Triggers.Left > 0)
            {
                if (!action1)
                    newaction1 = true;

                action1 = true;
            }
            else
                action1 = false;

            if (gps.IsButtonDown(Buttons.LeftShoulder))
            {
                if (!action2)
                    newaction2 = true;

                action2 = true;
            }
            else
                action2 = false;
        }

        public static Keyset getKeys(int player)
        {
            switch (player)
            {
                case 0:
                    return new Keyset(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.T, Keys.Y, Keys.G, Keys.H, Keys.Q, Keys.E);
                default:
                    return new Keyset(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl, Keys.RightShift, Keys.RightAlt, Keys.N, Keys.X, Keys.RightControl, Keys.NumPad0);
            }
        }
    }
}
