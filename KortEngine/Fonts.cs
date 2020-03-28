using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Threading.Tasks;

namespace KortEngine
{
    public static class Fonts
    {
        public static SpriteFont[] fontsarr = new SpriteFont[1];
        public static void Load(ContentManager Content)
        {
            fontsarr[0] = Content.Load<SpriteFont>("Font1");
        }
    }
}
