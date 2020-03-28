using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace KortEngine
{
    class Tile
    {
        public const int Size = 10;
        public int Type;
        public bool Collidable = true;
        private Rectangle tileRectangle;
        public Rectangle Rect { get {return tileRectangle;} }
        public Tile(int tiletype)
        {
            Type = tiletype;
        }
        public void SetNewBoundingBox(Vector2 Pos)
        {
            tileRectangle = new Rectangle(Pos.ToPoint(), new Point(Size, Size));
        }
    }
}
