using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public struct AnimationData
    {
        public int frameHeight;
        public int frameWidth;
        public int[] numFrames;
        public int[] numRows;
        public int interval;

        public AnimationData(int frameWidth, int frameHeight, int[] numFrames, int[] numRows, int interval = 5)
        {
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.numFrames = numFrames;
            this.numRows = numRows;
            this.interval = interval;
        }
        public AnimationData(int frameWidth, int frameHeight, int numFrames, int[] numRows, int interval = 5)
        {
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.numFrames = new int[numRows.Count()];
            for (int i = 0; i < this.numFrames.Count(); i++)
                this.numFrames[i] = numFrames;
            this.numRows = numRows;
            this.interval = interval;
        }
        public AnimationData(int frameWidth, int frameHeight, int numFrames, int numRows, int interval = 5)
        {
            this.frameHeight = frameHeight;
            this.frameWidth = frameWidth;
            this.numFrames = new int[1];
            this.numFrames[0] = numFrames;
            this.numRows = new int[1];
            this.numRows[0] = numRows;
            this.interval = interval;
        }
    }
    public class Animation
    {
        public int currentFrame;
        public int currentRow;
        public int rowMultiplier = 0;
        public int animationQueue = -1;
        public AnimationData animationData;
        public bool continuous;

        public Timer timer;


        public Animation(AnimationData animationData, bool continuous = true)
        {
            this.animationData = animationData;
            this.timer = new Timer(animationData.interval, true, AdvanceFrame);
            this.continuous = continuous;

            if (continuous)
                timer.Start();
        }


        public void changeAnimation(int newRowMult)
        {
            animationQueue = -1; 
            rowMultiplier = newRowMult;
            currentFrame = 0;
            currentRow = 0;
        }

        public Rectangle getSpriteRectangle()
        {
            int row = 0;
            for (int i = 0; i < rowMultiplier; i++) {
                row += animationData.numRows[i];
            }
            return new Rectangle(currentFrame * animationData.frameWidth, (row + currentRow) * animationData.frameHeight, animationData.frameWidth, animationData.frameHeight);
        }

        public Vector2 getSpriteOrigin()
        {
            return getSpriteOrigin(Vector2.Zero, 1);
        }

        public Vector2 getSpriteOrigin(Vector2 offset, float scale)
        {
            Vector2 center = new Vector2(animationData.frameWidth / 2f, animationData.frameHeight / 2f);
            center += offset / scale;

            return center;
        }

        public void AdvanceFrame()
        {
            currentFrame++;
            if (currentFrame >= animationData.numFrames[rowMultiplier])
            {
                currentFrame = 0;
                currentRow++;
                if (currentRow >= animationData.numRows[rowMultiplier])
                {
                    currentRow = 0;

                    if (animationQueue != -1)
                    {
                        rowMultiplier = animationQueue;
                        animationQueue = -1;
                    }

                    else if (!continuous)
                        timer.Stop();
                }
            }
        }

        public void Destroy()
        {
          
        }
    }
}
