using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KortEngine
{
    partial class Map
    {
        public void GenerateNewMap(int maptype)
        {
            //actual height of land

            switch (maptype)
            {
                default:
                    //set default 0
                    InitialiseMapData();
                    int[,] HeightData = InitialiseHeightData();

                    //smoothing
                    Smooth(ref HeightData, SmoothTimes);

                    //now that we have height data-write to the tilemap
                    SetMapDataFromHeight(HeightData);

                    //trees add
                    GenerateTrees(HeightData);

                    bool[,] cellmap = CellAutomata.CellMap(Width, ChunkWidth, Height, CADepthCaves,
                        CACaveschance, CACaveSteps, CavesdeathLimit, CavesbirthLimit);

                    //generateCaves
                    GenerateCaves(cellmap);

                    //treasure
                    PlaceTreasuresInCaves(cellmap);
                    break;
            }
            UpdateAllTiles();
        }
        private void InitialiseMapData()
        {
            Parallel.For(0, ChunkWidth, cx =>
            {
                Parallel.For(0, Width, x =>
                {
                    Parallel.For(0, Height, y =>
                    {
                        MapData[cx, x, y] = new Tile(0);
                    });
                });
            });
        }
        private void PlaceTreasuresInCaves(bool[,] cellmap)
        {
            //How hidden does a spot need to be for treasure?
            for (int x = 0; x < ChunkWidth * Width; x++)
            {
                for (int y = CADepthCaves; y < Height; y++)
                {
                    if (!cellmap[x, y])
                    {
                        int nbs = CellAutomata.countAliveNeighbours(cellmap, x, y, ChunkWidth, Width, Height);
                        if (nbs >= treasureHiddenLimit)
                        {
                            //Place chest
                            /*
                            int cx = (x / Width);
                            int xnew = x % Width;
                            MapData[cx, xnew, y] = 4;
                            */
                        }
                    }
                }
            }
        }
        private void GenerateTrees(int[,] HeightData)
        {
            bool[,] TreeData = new bool[ChunkWidth, Width];
            Parallel.For(0, ChunkWidth, cx =>
            {
                Parallel.For(0, Width, x =>
                {
                    TreeData[cx, x] = false;
                });
            });
            for (int cx = 0; cx < ChunkWidth; cx++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (rndm.NextDouble() < ChanceTrees)
                    {
                        int y = Height - HeightData[cx, x];
                        if (MapData[cx, x, y].Type == 1 || MapData[cx, x, y].Type == 6) //must be dirt or grass
                        {
                            if (MapData[cx, x, y - 1].Type == 0) //must be open air above
                            {
                                if (IsThereTreeNeighbour(cx, x, TreeData) == false) //no neighbours
                                {
                                    int treeHeight = rndm.Next(TreeMinHeight, TreeMaxHeight);//regular
                                    if (rndm.NextDouble() < RareChanceTrees)
                                    {
                                        //override height
                                        treeHeight = rndm.Next(RareTreeHeightMin, RareTreeHeightMax);
                                    }
                                    //place tree
                                    Parallel.For(y - treeHeight, y, yi =>
                                    {
                                        MapData[cx, x, yi].Type = 4;//wood
                                    });
                                    //place greens
                                    PlaceTreeGreens(cx, x, y, treeHeight);
                                    TreeData[cx, x] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool IsThereTreeNeighbour(int cx, int x, bool[,] treeData)
        {
            for (int i = -TreeDistanceLimit; i < TreeDistanceLimit + 1; i++)
            {
                int xnew = x + i;
                int cnew = cx;
                if (xnew < 0)
                {
                    if (cnew != 0)
                    {
                        xnew = Width - xnew;
                        cnew -= 1;
                    }
                    else continue;
                }
                if (xnew > Width - 1)
                {
                    if (cnew != ChunkWidth - 1)
                    {
                        xnew = xnew - Width;
                        cnew += 1;
                    }
                    else continue;
                }
                if (treeData[cnew, xnew] == true)
                    return true;
            }
            return false;
        }

        private void PlaceTreeGreens(int cx, int x, int y, int treeHeight)
        {
            //list of co-ordinates
            int[,] coordinates = {
                { 0, -3 }, { 0, -2 }, { 0, -1 }, { -1, -2},
                { 1, -2}, { -2, -1}, {-1, -1}, {1, -1}, {2, -1}
            };
            for (int i = 0; i < 9; i++)
            {
                int xnew = x + coordinates[i, 0];
                int cnew = cx;
                if (xnew < 0)
                {
                    if (cnew != 0)
                    {
                        xnew = Width - xnew;
                        cnew -= 1;
                    }
                    else continue;
                }
                if (xnew > Width - 1)
                {
                    if (cnew != ChunkWidth - 1)
                    {
                        xnew = xnew - Width;
                        cnew += 1;
                    }
                    else continue;
                }
                //it has to be air
                if (MapData[cnew, xnew, y - treeHeight + coordinates[i, 1]].Type == 0)
                    MapData[cnew, xnew, y - treeHeight + coordinates[i, 1]].Type = 5;
            }
        }
        private void GenerateCaves(bool[,] cellmap)
        {
            //now if alive set to 0
            Parallel.For(0, Width * ChunkWidth, x =>
            {
                Parallel.For(CADepthCaves + rndm.Next(-CAVarNeg, CAVarPos), Height, y =>
                {
                    if (cellmap[x, y] == true)
                    {
                        int cx = (x / Width);
                        int xnew = x % Width;
                        if (MapData[cx, xnew, y].Type != 2)
                            MapData[cx, xnew, y].Type = 0;
                    }
                });
            });
        }
        private void SetMapDataFromHeight(int[,] HeightData)
        {
            //don't forget to initialse folks
            InitialiseMapData();
            Parallel.For(0, ChunkWidth, cx =>
            {
                Parallel.For(0, Width, x =>
                {
                    int stoneDisplacement = rndm.Next(3, 5);
                    Parallel.For(Height - HeightData[cx, x], Height - HeightData[cx, x] + stoneDisplacement, y =>
                    {
                        MapData[cx, x, y].Type = 1;
                    });
                    Parallel.For(Height - HeightData[cx, x] + stoneDisplacement, Height, y =>
                    {
                        MapData[cx, x, y].Type = 3;
                    });
                    if (HeightData[cx, x] < OceanHeight)
                        Parallel.For(Height - OceanHeight, Height - HeightData[cx, x], y =>
                        {
                            //water
                            MapData[cx, x, y].Type = 2;
                        });
                    else
                    {
                        MapData[cx, x, Height - HeightData[cx, x]].Type = 6;
                    }
                });
            });
        }
        private int[,] InitialiseHeightData()
        {
            int[,] HeightData = new int[ChunkWidth, Width];
            Parallel.For(0, ChunkWidth, cx =>
            {
                Parallel.For(0, Width, x =>
                {
                    //1 in 5 chance of being a random digit-but repeat the loop twice
                    if (rndm.Next() % 4 == 0)
                        HeightData[cx, x] = rndm.Next(LowestPoint, MaxMountainHeight);
                    else //set to default
                        HeightData[cx, x] = DefaultLandHeight;
                });
            });
            return HeightData;
        }
        private void Smooth(ref int[,] HeightData, int SmoothTimes)
        {
            int[,] HeightDataNew = new int[ChunkWidth, Width];
            for (int i = 0; i < SmoothTimes; i++)
            {
                for (int cx = 0; cx < ChunkWidth; cx++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        HeightDataNew[cx, x] = GetNewHeight(cx, x, HeightData);
                    }
                }

                HeightData = HeightDataNew;
            }
        }
        private int GetNewHeight(int cx, int x, int[,] HeightData)
        {
            int newHeight = 0;
            int count = 0;
            for (int i = -2; i < 3; i++)
            {
                /*if (cx == 0)
                {
                    if (x == 0 | x == 1)
                    {
                        continue;
                    }
                }
                if (cx == ChunkWidth - 1)
                {
                    if (x == Width - 1 | x == Width - 2)
                    {
                        continue;
                    }
                }*/

                int xnew = x + i;
                int cnew = cx;
                if (xnew < 0)
                {
                    if (cnew != 0)
                    {
                        xnew = Width - xnew;
                        cnew -= 1;
                    }
                    else continue;
                }
                if (xnew > Width - 1)
                {
                    if (cnew != ChunkWidth - 1)
                    {
                        xnew = xnew - Width;
                        cnew += 1;
                    }
                    else continue;
                }
                newHeight += HeightData[cnew, xnew];
                count += 1;
            }
            return newHeight / count;
        }
    }
}
