using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public struct VertexPositionColorNormal
    {
        Vector3 position;
        Color color;
        Vector3 normal;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
         new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
         new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
         new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        public VertexPositionColorNormal(Vector3 pos, Color color, Vector3 normal)
        {
            this.position = pos;
            this.color = color;
            this.normal = normal;
        }
    }
    public class RenderHelper
    {
        Effect renderEffect;
        GraphicsDevice graphics;
        Matrix proj;

        public RenderHelper(Effect renderEffect, GraphicsDevice graphics, Matrix proj)
        {
            this.renderEffect = renderEffect;
            this.graphics = graphics;
            graphics.RasterizerState = RasterizerState.CullNone;
            renderEffect.Parameters["Projection"].SetValue(proj);
            this.proj = proj;
        }

        public void begin()
        {
            renderEffect.Parameters["View"].SetValue(Data.player.camera.view);
            //renderEffect.Parameters["View"].SetValue(Matrix.CreateLookAt(Vector3.Zero, new Vector3(0,0,1), new Vector3(0,-1,0)));
            renderEffect.CurrentTechnique.Passes[0].Apply();
            renderEffect.Parameters["lightDir"].SetValue(new Vector3(0,1,0));
            renderEffect.Parameters["ambient"].SetValue(1f);

            var r = new RasterizerState();
            r.CullMode = CullMode.None;
            graphics.RasterizerState = r;
            graphics.DepthStencilState = DepthStencilState.Default;
            //graphics.RasterizerState.


            graphics.BlendState = BlendState.AlphaBlend;
        }

        public void render(VertexMesh vertexMesh)
        {
            renderEffect.Parameters["World"].SetValue(vertexMesh.world);
            graphics.DrawUserIndexedPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleList, vertexMesh.verts, 0, vertexMesh.verts.Count(), vertexMesh.indices, 
                0, 8, VertexPositionColorNormal.VertexDeclaration);

        }

        public void renderPlaneTex(PlaneTexture planeTex)
        {
            planeTex.effect.Projection = proj;
            planeTex.effect.View = Data.player.camera.view;
            

            foreach (EffectPass pass in planeTex.effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleStrip, planeTex.verts, 0, 2);
            }
        }
        
        internal void renderModel(Model model, Matrix world, Vector3 diffuse, Vector3 spec, Vector3 amb)
        {
 
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //diffuse = new Vector3(0.997f, 0.377f, 0.787f);
                    effect.LightingEnabled = true; // Turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = diffuse;
                    effect.DirectionalLight0.Direction = new Vector3(.7f, 1, 0);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = spec;
                    effect.DirectionalLight0.Direction.Normalize();
                    //effect.DirectionalLight1.DiffuseColor = new Vector3(1f, 1f, 1f);
                    //effect.DirectionalLight1.Direction = new Vector3(0, 1, 0);
                    effect.AmbientLightColor = amb; 
                    //effect.SpecularColor = spec;
                    effect.SpecularPower = 100;
                    effect.PreferPerPixelLighting = true;
                    effect.World = world;
                    effect.View = Data.player.camera.view;
                    effect.Projection = Data.proj;

                    effect.FogColor = new Vector3(.5f,.5f,.5f);
                    effect.FogEnabled = true;
                    effect.FogStart = 200;

                    if (model != Data.sattelite.model)
                        effect.FogEnd = 3000;
                    else
                        effect.FogEnd = 80000;
                }

                mesh.Draw();
            }
        }
    }
}
