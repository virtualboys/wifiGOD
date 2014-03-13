using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SatteliteCrashers
{
    public static class KeyboardManager
    {
        public static bool zero;
        public static bool one;
        public static bool two;
        public static bool three;
        public static bool four;
        public static bool five;
        public static bool six;
        public static bool seven;
        public static bool eight;
        public static bool nine;
        public static bool newzero;
        public static bool newone;
        public static bool newtwo;
        public static bool newthree;
        public static bool newfour;
        public static bool newfive;
        public static bool newsix;
        public static bool newseven;
        public static bool neweight;
        public static bool newnine;

        public static char currentPress;

        public static void update()
        {


            currentPress = '\0';
            if (newzero)
                currentPress = '0';
            else if (newone)
                currentPress = '1';
            else if (newtwo)
                currentPress = '2';
            else if (newthree)
                currentPress = '3';
            else if (newfour)
                currentPress = '4';
            else if (newfive)
                currentPress = '5';
            else if (newsix)
                currentPress = '6';
            else if (newseven)
                currentPress = '7';
            else if (neweight)
                currentPress = '8';
            else if (newnine)
                currentPress = '9';

            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.D0) || ks.IsKeyDown(Keys.NumPad0)) 
            {
                newzero = !zero;
                zero = true;
            }
            else
            {
                newzero = zero = false;
            }
            if (ks.IsKeyDown(Keys.D1) || ks.IsKeyDown(Keys.NumPad1))
            {
                newone = !one;
                one = true;
            }
            else
            {
                newone = one = false;
            }
            if (ks.IsKeyDown(Keys.D2) || ks.IsKeyDown(Keys.NumPad2))
            {
                newtwo = !two;
                two = true;
            }
            else
            {
                newtwo = two = false;
            }
            if (ks.IsKeyDown(Keys.D3) || ks.IsKeyDown(Keys.NumPad3))
            {
                newthree = !three;
                three = true;
            }
            else
            {
                newthree = three = false;
            }
            if (ks.IsKeyDown(Keys.D4) || ks.IsKeyDown(Keys.NumPad4))
            {
                newfour = !four;
                four = true;
            }
            else
            {
                newfour = four = false;
            }
            if (ks.IsKeyDown(Keys.D5) || ks.IsKeyDown(Keys.NumPad5))
            {
                newfive = !five;
                five = true;
            }
            else
            {
                newfive = five = false;
            }
            if (ks.IsKeyDown(Keys.D6) || ks.IsKeyDown(Keys.NumPad6))
            {
                newsix = !six;
                six = true;
            }
            else
            {
                newsix = six = false;
            }
            if (ks.IsKeyDown(Keys.D7) || ks.IsKeyDown(Keys.NumPad7))
            {
                newseven = !seven;
                seven = true;
            }
            else
            {
                newseven = seven = false;
            }
            if (ks.IsKeyDown(Keys.D8) || ks.IsKeyDown(Keys.NumPad8))
            {
                neweight = !eight;
                eight = true;
            }
            else
            {
                neweight = eight = false;
            }
            if (ks.IsKeyDown(Keys.D9) || ks.IsKeyDown(Keys.NumPad9))
            {
                newnine = !nine;
                nine = true;
            }
            else
            {
                newnine = nine = false;
            }
        }
    }
}
