using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public class Cloud
    {
        public Model model;
        public Matrix world;
        public Vector3 pos;
        public Vector3 vel;
        Vector3 color;
        float scale;
        float rot;

        Random rand;

        public Cloud(Model model, Vector3 pos, float speed, float scale)
        {
            this.model = model;
            this.pos = pos;
            this.scale = scale;
            color = new Vector3(255f / 255, 102f / 255, 255f / 255);

            rand = new Random();
            vel = speed * new Vector3((float)rand.NextDouble() - .5f, ((float)rand.NextDouble() - .5f) / 10, (float)rand.NextDouble() - .5f);
            this.rot = (float)rand.NextDouble() * MathHelper.TwoPi;

            foreach (ModelMesh mesh in model.Meshes)
                foreach (BasicEffect effect in mesh.Effects)
                    effect.VertexColorEnabled = false;
        }

        public void update()
        {
            //vel = Vector3.Transform(vel, rot);

            pos += vel;

            world = Matrix.CreateRotationX(MathHelper.PiOver2);
            world *= Matrix.CreateRotationY(this.rot);
            world *= Matrix.CreateScale(scale);
            world *= Matrix.CreateTranslation(pos);
        }

        public void render(RenderHelper renderHelper)
        {
            renderHelper.renderModel(model, world, Data.diffColor * .2f, Vector3.Zero, Vector3.One);
        }
    }
}
