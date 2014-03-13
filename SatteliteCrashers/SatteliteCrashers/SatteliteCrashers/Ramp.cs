using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SatteliteCrashers
{
    public class Ramp
    {

        Vector3 diff;
        Vector3 spec;
        Vector3 amb = .5f * new Vector3(0.997f, 0.377f, 0.787f);

        public float radius;
        public Model model;
        public Vector3 position;
        public float top;
        public Vector3 sphereCenter;
        Matrix world;

        float scale = 20;

        public Ramp(Model model, float radius, Vector3 position)
        {
            this.radius = radius;
            this.model = model;
            this.position = position;

            top = -1;
            float bottom = 1;

            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in modelMesh.MeshParts)
                {
                    var verts = new Vector3[meshPart.NumVertices * 2];
                    meshPart.VertexBuffer.GetData<Vector3>(verts);

                    foreach (Vector3 v in verts)
                    {
                        if (v.Z > top)
                            top = v.Z;
                        else if (v.Z < bottom)
                            bottom = v.Z;
                    }
                }
            }

            bottom *= scale;
            bottom = position.Y - bottom;
            top *= scale;
            top = position.Y - top;
             
            this.radius *= scale;
            sphereCenter = new Vector3(position.X, bottom - this.radius, position.Z);

            world = Matrix.Identity;
            
            world *= Matrix.CreateRotationX(MathHelper.PiOver2);
            world *= Matrix.CreateScale(scale); 
            world *= Matrix.CreateTranslation(position);

            diff = new Vector3(0.997f, 0.377f, 0.787f);
            spec = new Vector3(.1f, .1f, .1f);
        }

    

        public Vector3 getNormal(ref Vector3 pos)
        {
            if (pos.Y < top || Vector2.Distance(new Vector2(pos.X,pos.Z), new Vector2(sphereCenter.X, sphereCenter.Z)) > radius)
                return Vector3.Zero;

            float y = (float)(radius * radius - Math.Pow(pos.X - sphereCenter.X, 2) - Math.Pow(pos.Z - sphereCenter.Z, 2));
            y = (float)Math.Sqrt(y) + sphereCenter.Y;

            if (y > pos.Y)
                return Vector3.Zero;

            Data.player.position.Y = y;

            var normal = sphereCenter - new Vector3(pos.X, y, pos.Z);
            normal.Normalize();

            return normal;
        }

        public void render(RenderHelper renderHelper)
        {
            renderHelper.renderModel(model, world, Data.diffColor, spec, amb);
        }
    }
}
