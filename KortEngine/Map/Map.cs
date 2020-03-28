using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace KortEngine
{
    public partial class Map
    {
        //all in hiehgt from the ground
        //sky is at logical height 0 (as we index this)
        public const int Height = 512;
        public const int Width = 128;
        public const int ChunkWidth = 64;
        public const int DefaultLandHeight = 350;
        public const int OceanHeight = 340;
        //generation constants-actual height
        public const int MaxMountainHeight = DefaultLandHeight + 100;
        public const int LowestPoint = OceanHeight - 100;
        public const int SmoothTimes = 6;
        //generation constants-cellautomata-logical height
        public const int CADepthCaves = Height - (DefaultLandHeight - 25);
        public const int CAVarNeg = 10;
        public const int CAVarPos = 5;
        public const int CavesdeathLimit = 3;
        public const int CavesbirthLimit = 4;
        public const int CACaveSteps = 2;
        public const float CACaveschance = 0.35f;
        //cell automate-treasure
        public const int treasureHiddenLimit = 5;
        //trees
        public const float ChanceTrees = 0.2f;
        public const float RareChanceTrees = 0.02f;
        public const int TreeMinHeight = 5;
        public const int TreeMaxHeight = 14;
        public const int RareTreeHeightMin = TreeMinHeight + 10;
        public const int RareTreeHeightMax = TreeMaxHeight + 10;
        public const int TreeDistanceLimit = 1;

        //map logic data
        Tile[,,] MapData = new Tile[ChunkWidth, Width, Height];
        int VisChunksArrayLength = 0;
        int[] VisChunksArray = new int[5];

        //all map tiles and such
        public const int NoTiles = 6;
        Texture2D[] TileArray = new Texture2D[NoTiles];

        //random numbers
        Random rndm = new Random();

        //default Font and tile
        SpriteFont font;
        Texture2D tile;

        //Gravity
        float Gravity = 0.1f;

        //timer-for map sub update
        //private System.Timers.Timer SecondTimer= new System.Timers.Timer(1000);

        public void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>("FontDefault");
            tile = Content.Load<Texture2D>("Tile");
            for (int i = 0; i < NoTiles; i++)
                TileArray[i] = Content.Load<Texture2D>("Tile" + i.ToString());

            // Hook up the Elapsed event for the timer.
            //SecondTimer.Elapsed += new ElapsedEventHandler(MapSubUpdate);
            //SecondTimer.Enabled = true;
        }

        private void MapSubUpdate(float camxpos)//object sender, ElapsedEventArgs e)
        {
            float xfloat = camxpos / (Width * 10);
            int x = Convert.ToInt32(xfloat);
            int index = 0;
            for (int i = x - 2; i < x + 3; i++)
            {
                if (i < 0)
                    continue;
                if (i >= ChunkWidth)
                    continue;
                VisChunksArray[index] = i;
                index += 1;
            }
            VisChunksArrayLength = index;
        }

        void UpdateAllTiles()
        {
            Parallel.For(0, ChunkWidth, cx =>
            {
                UpdateChunk(cx);
            });
        }
        void UpdateChunk(int cx)
        {
            Parallel.For(0, Width, x =>
            {
                Parallel.For(0, Height, y =>
                {
                    MapData[cx, x, y].SetNewBoundingBox(GetTilePos(cx, x, y));
                    switch (MapData[cx, x, y].Type)
                    {
                        case 0:                                     //air
                            MapData[cx, x, y].Collidable = false;
                            break;
                        case 2:                                     //water
                            MapData[cx, x, y].Collidable = false;
                            break;
                        case 4:                                 //tree wood
                            MapData[cx, x, y].Collidable = false;
                            break;
                        case 5:                                 //tleaves
                            MapData[cx, x, y].Collidable = false;
                            break;
                        default:
                            MapData[cx, x, y].Collidable = true;
                            break;
                    }
                });
            });
        }

        public void Update(KeyboardState ks, KeyboardState KSOld, float camxpos)
        {
            MapSubUpdate(camxpos);
            UpdatePlayers(ks, KSOld);
        }

        Vector2 GetTilePos(int cx, int x, int y)
        {
            return new Vector2((10 * Width * cx) + (10 * x), 10 * y);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < VisChunksArrayLength; i++)
            {
                int cx = VisChunksArray[i];
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (MapData[cx, x, y].Type != 0)
                            spriteBatch.Draw(
                                TileArray[MapData[cx, x, y].Type - 1],
                                GetTilePos(cx, x, y),
                                Color.White);
                    }
                }
            }
            for (int i = 0; i < Players.Count; i++)
                Players[i].Draw(spriteBatch);
        }
        public Texture2D RecolourDefaultTile(GraphicsDevice graphics, float colourLevel)
        {
            MemoryStream memoryStream = new MemoryStream();
            tile.SaveAsPng(memoryStream, tile.Width, tile.Height); //Or ( memoryStream, texture.Width, texture.Height )
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(memoryStream);

            Color[] pixels = new Color[bmp.Width * bmp.Height];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    System.Drawing.Color c = bmp.GetPixel(x, y);
                    if (c.R == 0)
                    {
                        if (c.A == 255)
                            pixels[(y * bmp.Width) + x] = new Color(colourLevel, colourLevel, colourLevel, c.A);
                        else
                            pixels[(y * bmp.Width) + x] = new Color(c.R, c.G, c.B, c.A);
                    }
                }
            }

            Texture2D myTex = new Texture2D(graphics,bmp.Width,bmp.Height);

            myTex.SetData<Color>(pixels);

            return myTex;
        }
    }
}