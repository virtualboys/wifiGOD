using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public static class Data
    {
        public static Player player;
        public static Ramp ramp;
        public static Matrix proj;
        public static List<Cloud> clouds;
        public static Sun sun;
        public static List<PlaneTexture> stars;
        public static Vector3 diffColor = new Vector3(0.997f, 0.377f, 0.787f);
        public static Sattelite sattelite;
        //public static Vector3 diffColor = new Vector3(255/255f, 204/255f, 229/255f);
    }
}
