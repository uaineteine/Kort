#region Using Statement

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace ScytheEngine.Engine.Game.Player
{
    public class Player
    {
        public InventoryBar InvBar = new InventoryBar();
        public InventoryWindow InvWindow = new InventoryWindow();
        public int Team;
        public Texture2D TexRight;
        public Texture2D TexLeft;
        public Texture2D Tex;
        public Rectangle Rect;
        public Rectangle RectDet;
        public Vector2 Pos = new Vector2(450, 452);
        public int Facing = 0;
        public Vector2 PlayerVeloctiy = Vector2.Zero;
        public float JumpVeloctiy = 0f;
        public float DownForce = 0f;
        public bool Falling = false;
        public bool Jumping = false;
        public bool IntersectingLeft = false;
        public bool IntersectingRight = false;
        public float PlayerHealth = 100f;
        public void LoadContent(ContentManager Content)
        {
            TexRight = Content.Load<Texture2D>("Images/Game/Player/BaseRight");
            TexLeft = Content.Load<Texture2D>("Images/Game/Player/BaseLeft");
            Tex = TexRight;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, (int)Tex.Width, (int)Tex.Height);
            RectDet = new Rectangle((int)Pos.X, (int)Pos.Y, (int)Tex.Width, (int)Tex.Height + 1);
            InvBar.LoadContent(Content);
            InvWindow.LoadContent(Content);
        }
        public void Update(MouseState MS, MouseState MSOld,KeyboardState KS, KeyboardState KSOld)
        {
            PlayerVeloctiy *= .75f;
            UpdateMovement(KS, KSOld);
            UpdateGravity(KS, KSOld);
            Rect.X = (int)Pos.X;
            Rect.Y = (int)Pos.Y;
            RectDet.X = (int)Pos.X;
            RectDet.Y = (int)Pos.Y;
            Pos += PlayerVeloctiy;
            if (InvWindow.Shown == false)
            {
                InvBar.Update(MS, MSOld, KS, KSOld);
            }
            if (InvWindow.Shown == true)
            {
                InvWindow.Update(MS, MSOld, KS, KSOld);
            }
        }
        public void UpdateGravity(KeyboardState KS, KeyboardState KSOld)
        {
            if (Jumping == true)
            {
                Pos.Y += JumpVeloctiy;
                JumpVeloctiy += 1f;
                if (JumpVeloctiy == 0)
                {
                    Falling = true;
                    Jumping = false;
                }
            }
            if (Falling == false)
            {
                DownForce = 0f;
                if (KS.IsKeyDown(Keys.Space))
                {
                    if (!KSOld.IsKeyDown(Keys.Space))
                    {
                        JumpVeloctiy = -14f;
                        Jumping = true;
                    }
                }
            }
            if (Falling == true)
            {
                if (DownForce < 8)
                {
                    DownForce += 1;
                }
                Pos.Y += DownForce;
            }
        }
        public void UpdateMovement(KeyboardState KS, KeyboardState KSOld)
        {
            if (IntersectingRight == false)
            {
                if (KS.IsKeyUp(Keys.A))
                {
                    if (KS.IsKeyDown(Keys.D))
                    {
                        IntersectingLeft = false;
                        PlayerVeloctiy.X = 4;
                        Tex = TexRight;
                    }
                }
            }
            if (IntersectingLeft == false)
            {
                if (KS.IsKeyUp(Keys.D))
                {
                    if (KS.IsKeyDown(Keys.A))
                    {
                        IntersectingRight = false;
                        PlayerVeloctiy.X = -4;
                        Tex = TexLeft;
                    }
                }
            }
            if (Tex == TexRight)
            {
                Facing = 0;
            }
            else
            {
                Facing = 1;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Tex, Pos, Color.White);
            if (InvWindow.Shown == false)
            {
                InvBar.Draw(spriteBatch);
            }
            if (InvWindow.Shown == true)
            {
                InvWindow.Draw(spriteBatch);
            }
        }
    }
}