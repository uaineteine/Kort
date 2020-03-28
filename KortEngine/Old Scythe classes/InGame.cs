#region Using Statements

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScytheEngine.GUI;

#endregion

namespace ScytheEngine.Engine.Game
{
    public class InGameHandler
    {
        public Camera2d cam = new Camera2d();
        public TileEngine.Map Map = new TileEngine.Map(50, 256);
        public TileEngine.TileDraw Tiles = new TileEngine.TileDraw();
        public DayCycle.DayNight DayNightCycle = new DayCycle.DayNight();
        public Player.Player Player1 = new Player.Player();
        public Texture2D Bg;
        public GUI.GameScreens.Map MapScreen = new GUI.GameScreens.Map();
        public bool MapShown = false;
        public Compass EastAndWest = new Compass(1);
        public TimeSpan timeSpan;
        public void Initialize()
        {
            cam.Pos = new Vector2(Player1.Pos.X, Player1.Pos.Y - 150);
            MapScreen.Initialize(new Rectangle(0, 0, 900, 600));
        }
        public void LoadContent(ContentManager Content)
        {
            Bg = Content.Load<Texture2D>("Images/Game/Bg_1");
            Tiles.LoadTiles(Content);
            Map.GenerateRectangles();
            Map.GenerateCollideable();
            DayNightCycle.LoadContent(Content);
            Player1.LoadContent(Content);
            MapScreen.LoadContent(Content);
            EastAndWest.LoadContent(Content);
        }
        public void Update(GameTime gameTime, MouseState MS, MouseState MSOld, KeyboardState KS, KeyboardState KSOld)
        {
            timeSpan += gameTime.ElapsedGameTime;
            if (MapShown == false)
            {
                if (KS.IsKeyDown(Keys.M))
                {
                    if (!KSOld.IsKeyDown(Keys.M))
                    {
                        timeSpan = TimeSpan.Zero;
                        MapShown = true;
                    }
                }
                Map.UpdateVisibleChunks(cam.Pos);
                Player1.Update(MS, MSOld, KS, KSOld);
                UpdateCollison();
                if (Player1.Pos.X <= 450)
                {
                    cam.Pos = new Vector2(cam.Pos.X, Player1.Pos.Y - 150);
                }
                else if (Player1.Pos.X + 18 >= Map.MapWidthP - 450)
                {
                    cam.Pos = new Vector2(cam.Pos.X, Player1.Pos.Y- 150);
                }
                else
                {
                    cam.Pos = new Vector2(Player1.Pos.X, Player1.Pos.Y - 150);
                }
                DayNightCycle.Update();
                EastAndWest.Update(Map.ChunksSize, cam.Pos.X);
            }
            if (MapShown == true)
            {
                if (timeSpan > TimeSpan.FromSeconds(.01))
                {
                    if (KS.IsKeyDown(Keys.M))
                    {
                        if (!KSOld.IsKeyDown(Keys.M))
                        {
                            timeSpan = TimeSpan.Zero;
                            MapShown = false;
                        }
                    }
                }
            }
        }
        public void UpdateCollison()
        {
            for (int c = 0; c < Map.Chunks.Count; c++)
            {
                if (Map.Chunks[c].Visible == true)
                {
                    for (int x = 0; x < Map.Chunks[0].Column.Count; x++)
                    {
                        for (int y = 0; y < Map.Chunks[0].Column[0].Row.Count; y++)
                        {
                            if (Map.Chunks[c].Column[x].Row[y].TileID > 0)
                            {
                                if (Map.Chunks[c].Column[x].Row[y].Collideable == true)
                                {
                                    if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectLeft) == true)
                                    {
                                        if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectTop) == false)
                                        {
                                            Player1.PlayerVeloctiy.X = 0;
                                            Player1.IntersectingRight = true;
                                            Player1.Pos.X = Map.Chunks[c].Column[x].Row[y].Rect.X - 36;
                                        }
                                    }
                                    if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectRight) == true)
                                    {
                                        if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectTop) == false)
                                        {
                                            Player1.PlayerVeloctiy.X = 0;
                                            Player1.IntersectingLeft = true;
                                            Player1.Pos.X = Map.Chunks[c].Column[x].Row[y].Rect.X + 16;
                                        }
                                    }
                                    if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectTop) == true)
                                    {
                                        if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectLeft) == false & Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectRight) == false)
                                        {
                                            Player1.IntersectingRight = false;
                                            Player1.IntersectingLeft = false;
                                            Player1.Falling = false;
                                            Player1.Pos.Y = Map.Chunks[c].Column[x].Row[y].Rect.Y - 108;
                                        }
                                    }
                                    if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectBottom) == true)
                                    {
                                        if (Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectLeft) == false & Player1.Rect.Intersects(Map.Chunks[c].Column[x].Row[y].RectRight) == false)
                                        {
                                            Player1.IntersectingLeft = false;
                                            Player1.IntersectingRight = false;
                                            Player1.Falling = true;
                                            Player1.Pos.Y = Map.Chunks[c].Column[x].Row[y].Rect.Y + 16;
                                        }
                                    }
                                    else
                                    {
                                        if (Player1.Pos.X + 36 >= Map.MapWidthP)
                                        {
                                            Player1.IntersectingLeft = false;
                                            Player1.IntersectingRight = true;
                                        }
                                        if (Player1.Pos.X <= 0)
                                        {
                                            Player1.IntersectingRight = false;
                                            Player1.IntersectingLeft = true;
                                        }
                                        else
                                        {
                                            Player1.IntersectingRight = false;
                                            Player1.IntersectingLeft = false;
                                        }
                                    }
                                    if (Player1.Pos.X + 36 >= Map.MapWidthP)
                                    {
                                        Player1.IntersectingLeft = false;
                                        Player1.IntersectingRight = true;
                                        Player1.Pos.X = Map.MapWidthP - 36;
                                    }
                                    if (Player1.Pos.X <= 0)
                                    {
                                        Player1.IntersectingRight = false;
                                        Player1.IntersectingLeft = true;
                                    }
                                }
                            }
                        }
                        if (Player1.Jumping == false)
                        {
                            if (Player1.Falling == false)
                            {
                                //No1
                                bool Collide1 = Map.Chunks[0].Column.All(column =>
                                    column.Row.All(row =>
                                    row.Rect.Intersects(Player1.RectDet) == false));
                                //No2
                                bool Collide2 = Map.Chunks[1].Column.All(column =>
                                    column.Row.All(row =>
                                    row.Rect.Intersects(Player1.RectDet) == false));
                                if (Collide1 == true & Collide2 == true)
                                {
                                    Player1.Falling = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(Bg, Vector2.Zero, Color.White);
            spriteBatch.End();
            DayNightCycle.Draw(spriteBatch);
            //Camera 2D
            spriteBatch.Begin(SpriteSortMode.Deferred,
                null, null, null, null, null,
                cam.get_transformation(graphicsDevice));
            Tiles.DrawTiles(spriteBatch, Map.Chunks);
            Player1.Draw(spriteBatch);
            spriteBatch.End();
            //EndCamera
            EastAndWest.DrawEaW(spriteBatch);
            if (MapShown == true)
            {
                MapScreen.Draw(spriteBatch);
            }
        }
    }
}