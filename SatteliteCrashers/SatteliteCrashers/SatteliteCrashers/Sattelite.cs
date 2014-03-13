using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public class Sattelite
    {
        public Model model;
        public Matrix world;
        public String code = "666";

        public Sattelite(Model model, Vector3 pos)
        {
            this.model = model;
            this.world = Matrix.CreateScale(50);
            world *= Matrix.CreateRotationX(MathHelper.PiOver2);
            world *= Matrix.CreateTranslation(pos);
        }

        //public 

        public void render(RenderHelper renderHelper)
        {
            renderHelper.renderModel(model, world, Vector3.One, Vector3.Zero, .4f*Vector3.One);
        }
    }
}
