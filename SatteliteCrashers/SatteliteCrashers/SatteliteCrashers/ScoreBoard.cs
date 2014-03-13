using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public static class ScoreBoard
    {
        static SpriteFont font;
        static Viewport view;
        static Vector2 pos;
        public static void Init(ContentManager content,Viewport view){
            font = content.Load<SpriteFont>("font");
            ScoreBoard.view = view;
            pos = new Vector2(view.Width / 7f, view.Height / 7f);
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            String output = Convert.ToString(-(int)(Data.player.position.Y + Data.ramp.sphereCenter.Y - Data.ramp.radius));
            Vector2 FontOrigin = font.MeasureString(output) / 2f;
            Random rand = new Random();
            if (Data.player.velocity.Y > 0)
            {
                pos += Data.player.velocity.Y * (new Vector2((float)rand.NextDouble() - .5f, (float)rand.NextDouble() - .5f)) * 3f;
            }
            spriteBatch.DrawString(font, output, pos,Color.Black,0,FontOrigin,1.0f,SpriteEffects.None,.5f);
            spriteBatch.End();

        }
    }
}
