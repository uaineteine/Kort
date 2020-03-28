using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace KortEngine
{
    public class Player
    {
        string Name;
        int ID;

        public Vector2 pos = Vector2.Zero;
        Vector2 Acceleration = Vector2.Zero;
        Vector2 Velocity = Vector2.Zero;
        public Rectangle playerRec;
        public bool Falling = true;
        public bool Jumped = false;
        public bool IsFalling { get { return Falling; } }
        public float MaxFallSpeed = 6f;
        private int SubSteps = 4;

        Texture2D playerTex;

        public Player(string name, int id, Vector2 SpawnPos)
        {
            Name = name;
            ID = id;
            pos = SpawnPos;
        }

        public void LoadContent(ContentManager Content)
        {
            playerTex = Content.Load<Texture2D>("testSprite");
        }
        public void AddVelocity(Vector2 vel)
        {
            Velocity += vel;
            if (Velocity.Y > MaxFallSpeed)
                Velocity.Y = MaxFallSpeed;
            if (Velocity.Y < -MaxFallSpeed)
                Velocity.Y = -MaxFallSpeed;
        }
        public void Grounded(float tilevertpos)
        {
            if (Falling)
            {
                Falling = false;
                Velocity.Y = 0;
                pos.Y = tilevertpos - playerTex.Height;
            }
        }
        public void Update(KeyboardState KS, KeyboardState KSOld)
        {
            if (Jumped)
            {
                if (Velocity.Y < 0)
                    Jumped = false;
            }
            if (KS.IsKeyDown(Keys.L))
                pos += new Vector2(2, 0);
            if (KS.IsKeyDown(Keys.I))
                pos += new Vector2(0, -2);
            if (KS.IsKeyDown(Keys.J))
                pos += new Vector2(-2, 0);
            if (KS.IsKeyDown(Keys.K))
                pos += new Vector2(0, 2);
            if (KS.IsKeyDown(Keys.Space) & KSOld.IsKeyUp(Keys.Space))
            {
                if (!Falling)
                    Jump();
            }
            Velocity += Acceleration;
            playerRec = NewCollisionBox(pos);
            Vector2 SubVel = Velocity / SubSteps;
            for (int i = 0; i < SubSteps; i++)
            {
                pos += SubVel;
            }
        }
        void Jump()
        {
            Velocity.Y -= 3f;
            Falling = true;
            Jumped = true;
        }
        Rectangle NewCollisionBox(Vector2 playerpos)
        {
            return new Rectangle(playerpos.ToPoint(), new Point(playerTex.Width, playerTex.Height));
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTex, pos, Color.White);
        }
    }
}
