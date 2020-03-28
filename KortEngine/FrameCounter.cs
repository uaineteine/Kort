using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KortEngine
{
    public static class FrameCounter
    {

        public static long TotalFrames { get; private set; }
        public static float TotalSeconds { get; private set; }
        public static float AverageFramesPerSecond { get; private set; }
        public static float CurrentFramesPerSecond { get; private set; }

        public const int MAXIMUM_SAMPLES = 100;

        private static Queue<float> _sampleBuffer = new Queue<float>();

        static bool Update(float deltaTime)
        {
            CurrentFramesPerSecond = 1.0f / deltaTime;

            _sampleBuffer.Enqueue(CurrentFramesPerSecond);

            if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
            {
                _sampleBuffer.Dequeue();
                AverageFramesPerSecond = _sampleBuffer.Average(i => i);
            }
            else
            {
                AverageFramesPerSecond = CurrentFramesPerSecond;
            }

            TotalFrames++;
            TotalSeconds += deltaTime;
            return true;
        }
        public static void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Update(deltaTime);

            var fps = string.Format("FPS: {0}", AverageFramesPerSecond);

            spriteBatch.DrawString(Fonts.fontsarr[0], fps, new Vector2(1, 1), Color.Black);

            // other draw code here
        }
    }
}
