#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace KortEngine.DayNight
{
    public class Sun
    {
        public Texture2D Tex;
        public float Rotation = 0f;
        public Vector2 Pos = Vector2.Zero;
        public float Scale = 1f;
        public void Update()
        {
            Rotation += 0.2f;
        }
        public void LoadContent(ContentManager Content)
        {
            Tex = Content.Load<Texture2D>("Sun");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Tex, Pos, null, Color.White, Rotation, new Vector2(Tex.Width, Tex.Height) / 2, Scale, SpriteEffects.None, 1f);
            spriteBatch.End();
        }
    }
}