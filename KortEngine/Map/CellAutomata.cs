using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KortEngine
{
    public static class CellAutomata
    {
        public static bool[,] CellMap(
            int Width, int ChunkWidth, int Height, int cellAutomaDepth, float chanceToStartAlive,
            int simulationSteps, int deathLimit, int birthLimit)
        {
            Random rndm = new Random();
            //cell automata-caves
            bool[,] cellmap = new bool[Width * ChunkWidth, Height];
            for (int x = 0; x < Width * ChunkWidth; x++)
            {
                for (int y = cellAutomaDepth; y < Height; y++)
                {
                    if (rndm.NextDouble() < chanceToStartAlive)
                    {
                        cellmap[x, y] = true;
                    }
                }
            }

            //do simulation step
            for (int i = 0; i < simulationSteps; i++)
            {
                cellmap = doSimulationStep(cellmap, ChunkWidth, Width, Height, cellAutomaDepth, deathLimit, birthLimit);
            }

            return cellmap;
        }
        //Returns the number of cells in a ring around (x,y) that are alive.
        public static int countAliveNeighbours(bool[,] cellmap, int x, int y,
            int Width, int ChunkWidth, int Height)
        {
            int count = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    //If we're looking at the middle point
                    if (i == 0 && j == 0)
                    {
                        //Do nothing, we don't want to add ourselves in!
                    }
                    //In case the index we're looking at it off the edge of the map
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= ChunkWidth * Width || neighbour_y >= Height)
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else if (cellmap[neighbour_x, neighbour_y])
                    {
                        count = count + 1;
                    }
                }
            }
            return count;
        }

        static bool[,] doSimulationStep(bool[,] oldMap, int ChunkWidth, int Width, int Height,
            int cellAutomaDepth, int deathLimit, int birthLimit)
        {
            bool[,] newMap = new bool[ChunkWidth * Width, Height];
            //Loop over each row and column of the map
            for (int x = 0; x < ChunkWidth * Width; x++)
            {
                for (int y = cellAutomaDepth; y < Height; y++)
                {
                    int nbs = countAliveNeighbours(oldMap, x, y, Width, ChunkWidth, Height);
                    //The new value is based on our simulation rules
                    //First, if a cell is alive but has too few neighbours, kill it.
                    if (oldMap[x, y])
                    {
                        if (nbs < deathLimit)
                        {
                            newMap[x, y] = false;
                        }
                        else
                        {
                            newMap[x, y] = true;
                        }
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (nbs > birthLimit)
                        {
                            newMap[x, y] = true;
                        }
                        else
                        {
                            newMap[x, y] = false;
                        }
                    }
                }
            }
            return newMap;
        }
    }
}