using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SatteliteCrashers
{
    public class Sun
    {
        public List<Texture2D> frames;
        public PlaneTexture planeTex;
        public Timer timer;
        public int frame;

        public Sun(PlaneTexture planeTex, List<Texture2D> frames)
        {
            this.planeTex = planeTex;
            this.frames = frames;

            timer = new Timer(100, true, () => { frame = Math.Abs(frame - 1); });
            timer.Start();

            //planeTex.effect.EmissiveColor = new Vector3(1, 1, 1);
            planeTex.effect.LightingEnabled = true;
        }

        public void update(GameTime gameTime)
        {
            timer.Update(gameTime);
            planeTex.effect.Texture = frames[frame];

            float c = Math.Max(0, 1 - Math.Max(0, -Data.player.position.Y - 500) / 1000);
            planeTex.effect.DiffuseColor = c * Vector3.One + (1 - c) * new Vector3(204 / 255f, 255 / 255f, 255 / 255f);
            //planeTex.effect.AmbientLightColor = 
        }

        public void render(RenderHelper renderHelper)
        {
            renderHelper.renderPlaneTex(planeTex);
        }
    }
}
