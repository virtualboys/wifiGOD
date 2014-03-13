using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public class PlaneTexture
    {
        public Texture2D tex;
        public Matrix world;
        public VertexPositionNormalTexture[] verts;
        public BasicEffect effect;

        public PlaneTexture(Texture2D tex, Vector3 pos, float size, GraphicsDevice graphics, BasicEffect effect = null)
        {
            this.tex = tex;
            //world = Matrix.CreateScale(size);
            world = Matrix.CreateTranslation(pos);

            if (effect == null)
            {
                this.effect = new BasicEffect(graphics);
                this.effect.World = world;
                this.effect.TextureEnabled = true;
                this.effect.Texture = tex;
            }
            else
                this.effect = effect;

            Vector3 normal = new Vector3(0, 1, 0);

            verts = new VertexPositionNormalTexture[4];
            verts[0] = new VertexPositionNormalTexture(pos + new Vector3(-tex.Width * size, 0, -tex.Height * size), normal, new Vector2(0, 0));
            verts[1] = new VertexPositionNormalTexture(pos + new Vector3(-tex.Width * size, 0, tex.Height * size), normal, new Vector2(0, 1));
            verts[2] = new VertexPositionNormalTexture(pos + new Vector3(tex.Width * size, 0, -tex.Height * size), normal, new Vector2(1, 0));
            verts[3] = new VertexPositionNormalTexture(pos + new Vector3(tex.Width * size, 0, tex.Height * size), normal, new Vector2(1, 1));
            
        }
    }
}
