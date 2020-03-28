using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KortEngine
{
    partial class Map
    {
        public List<Player> Players = new List<Player>();
        void UpdatePlayers(KeyboardState KS, KeyboardState KSOld)
        {
            for (int i=0; i < Players.Count; i++)
            {
                Players[i].Update(KS, KSOld);
                if (Players[i].IsFalling)
                {
                    //add gravity to acceleration
                    Players[i].AddVelocity(new Vector2(0, Gravity));

                    for (int visi = 0; visi < VisChunksArrayLength; visi++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            for (int y = 0; y < Height; y++)
                            {
                                if (MapData[VisChunksArray[visi], x, y].Collidable)
                                    if (MapData[VisChunksArray[visi], x, y].Rect.Intersects(Players[i].playerRec))
                                    {
                                        if (Players[i].Jumped)
                                        {
                                            Players[i].Jumped = false;
                                        }
                                        else
                                        {
                                            Players[i].Grounded(GetTilePos(VisChunksArray[visi], x, y).Y);
                                            //need to break nested loop for performance reasons
                                            //https://stackoverflow.com/questions/324831/breaking-out-of-a-nested-loop
                                            break;
                                        }
                                    }
                            }
                        }
                    }
                }
            }
        }
    }
}
