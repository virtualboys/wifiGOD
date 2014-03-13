using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SatteliteCrashers
{
    public class Timer
    {
        public double timer;
        public int timerLength;
        public int tempLength;
        public bool isRunning;
        public bool continuous;
        
        public VoidDel onTimerEndCallback;

        public Timer(int timerLength, bool continuous, VoidDel onTimerEndCallback)
        {
            this.timerLength = timerLength;
            this.continuous = continuous;
            isRunning = false;
            this.onTimerEndCallback = onTimerEndCallback;  
        }

        public void Update(GameTime gameTime)
        {
            if (isRunning)
            {
                timer += gameTime.ElapsedGameTime.TotalMilliseconds;

                int length = tempLength > 0 ? tempLength : timerLength;

                if (timer > length)
                {
                    timer = 0;
                    tempLength = 0;

                    if (onTimerEndCallback != null)
                        onTimerEndCallback();

                    if (!continuous)
                        isRunning = false;
                }
            }
        }

        public void Start()
        {
            isRunning = true;
        }

        public void Start(int duration)
        {
            tempLength = duration;
            Start();
        }

        public void Start(double duration)
        {
            Start((int)duration);
        }

        public void Pause()
        {
            isRunning = false;
        }

        public void Stop()
        {
            isRunning = false;
            timer = 0;
        }
    }
}
