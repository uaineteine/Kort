using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace KortEngine.DayNight
{
    static class DayNightClass
    {
        static bool Day = true;
        static Sun TheSun = new Sun();         //Just one for now, not too large on the thinking just yet haha

        public static void LoadContent(ContentManager Content)
        {
            TheSun.LoadContent(Content);
        }
        public static void Update()
        {
            if (Day)
                TheSun.Update();
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            if (Day)
                TheSun.Draw(spriteBatch);
        }
    }
}
