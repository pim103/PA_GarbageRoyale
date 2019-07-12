using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class MazeDataGenerator
    {
        public float placementThreshold;    // chance of empty space
        
        public Dictionary<string,string>[] roomLinksList = new Dictionary<string, string>[8];
        public Dictionary<string, int>[] roomTrap = new Dictionary<string, int>[8];
        public Dictionary<string, int>[] itemRoom = new Dictionary<string, int>[8];

        private int nbTrap;

        public MazeDataGenerator()
        {
            placementThreshold = .1f;                               // 1
        }

        public int[][,] FromDimensions(int sizeRows, int sizeCols)    // 2
        {
            int [][,] floors = new int[8][,];
            int[,] maze;
            int rMax;
            int cMax;
            int rand;

            for (int k = 0; k < 8; k++)
            {
                roomLinksList[k] = new Dictionary<string, string>();
                roomTrap[k] = new Dictionary<string, int>();
                itemRoom[k] = new Dictionary<string, int>();

                sizeCols = 8 * (10 - k) + 1;
                sizeRows = 8 * (10 - k) + 1;
                maze = new int[sizeRows, sizeCols];
                rMax = maze.GetUpperBound(0);
                cMax = maze.GetUpperBound(1);

                for (int i = 0; i <= rMax; i++)
                {
                    for (int j = 0; j <= cMax; j++)
                    {
                        rand = Mathf.RoundToInt((Random.Range(1, 8) % 8) + 1);
                        //1
                        if (i == 0 || j == 0 || i == rMax || j == cMax)
                        {
                            maze[i, j] = rand;
                        }
                        else if (i > (rMax / 2 - 5) && i < (rMax / 2 + 5) && j > cMax / 2 - 5 && j < cMax / 2 + 5 &&
                                 k == 0)
                        {
                            maze[i, j] = 0;
                        }
                        /*else if (k > 0 && floors[k - 1][i+4, j+4] == 2)
                        {
                            maze[i, j] = 3;
                        }*/
                        //2
                        else if (i % 2 == 0 && j % 2 == 0)
                        {
                            if (Random.value > placementThreshold)
                            {
                                //3
                                maze[i, j] = rand;

                                int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                                int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                                maze[i + a, j + b] = rand;
                            }
                        }
                    }
                }

                floors[k] = maze;
            }
            
            return floors;
        }

        public int[][,] RoomData(int sizeRows, int sizeCols, int[][,] maze, int nbItems)
        {
            int[][,] floorRooms = new int[8][,];
            int[,] rooms;
            int rMax;
            int cMax;
            int roll;
            bool noTrapDoorInArea;
            int frequency = 36;
            int trapDoorCount = 0;
            
            for (int k = 0; k < 8; k++)
            {
                sizeCols = 8 * (10 - k) + 1;
                sizeRows = 8 * (10 - k) + 1;   
                rooms = new int[sizeRows, sizeCols];
                rMax = rooms.GetUpperBound(0);
                cMax = rooms.GetUpperBound(1);
                trapDoorCount = 0;
                for (int i = 0; i <= rMax; i++)
                {
                    for (int j = 0; j <= cMax; j++)
                    {
                        rooms[i, j] = 9999;
                    }
                }

                for (int i = 0; i <= rMax; i++)
                {
                    for (int j = 0; j <= cMax; j++)
                    {
                        if (i > (rMax / 2 - 5) && i < (rMax / 2 + 5) && j > cMax / 2 - 5 && j < cMax / 2 + 5 &&
                            k == 0)
                        {
                            rooms[i, j] = 8;
                        }
                        else if (k>0 && (floorRooms[k-1][i+4, j+4] == 0 || floorRooms[k-1][i+4, j+4] == 3))
                        {
                            rooms[i, j] = 1;
                            
                        } else if (maze[k][i, j] == 0 && rooms[i,j] != 3 && rooms[i,j] != 2)
                        {
                            int rand = Random.Range(5, 13);
                            if (rand == 11)
                            {
                                rand = Random.Range(5, 13);
                            }
                            if(rand == 12)
                            {
                                int randItem = Random.Range(0, nbItems);
                                itemRoom[k].Add( i + ";" + j, randItem);
                            }
                            rooms[i, j] = rand;
                            //rooms[i, j] = 3;
                        }
                        if (i == 43 && j == 43)
                        {
                            rooms[i, j] = 11;
                        }
                    }
                }
                
                while (trapDoorCount < frequency)
                {
                    //if (k == 7) break;
                    for (int i = 0; i <= rMax; i++)
                    {
                        for (int j = 0; j <= cMax; j++)
                        {
                            noTrapDoorInArea = true;
                            for (int l = i - (3-k); l < i + (3-k); l++)
                            {
                                for (int m = j - (3-k); m < j + (3-k); m++)
                                {
                                    if (l > 0 && m > 0 && l < rMax && m < cMax)
                                    {
                                        if (rooms[l, m] == 0)
                                        {
                                            noTrapDoorInArea = false;
                                            break;
                                        }
                                    }
                                }

                                if (!noTrapDoorInArea) break;
                            }
                            //if (((k< 7 && maze[k+1][i, j] == 0) || k == 7) && i > 4 && j > 4 && i < rMax - 4 && j < cMax - 4 && frequency > trapDoorCount &&
                            //    noTrapDoorInArea && maze[k][i, j] == 0 && rooms[i,j] != 1 )
                            if ((i > 5 && j > 5 && i < rMax - 5 && j < cMax - 5 && frequency > trapDoorCount && noTrapDoorInArea && maze[k][i, j] == 0 && maze[k%7+1][i-4, j-4] == 0 && rooms[i,j] != 1 && rooms[i,j] != 0) && !(i > (rMax / 2 - 5) && i < (rMax / 2 + 5) && j > cMax / 2 - 5 && j < cMax / 2 + 5 && k == 0))
                            {
                                roll = Random.Range(0, cMax * rMax);
                                if (roll < cMax)
                                {
                                    if((Random.Range(0, 100) < 30) || k == 7) rooms[i, j] = 3;
                                    else rooms[i, j] = 0;
                                    trapDoorCount++;
                                    bool breaker = false;
                                    int nmin, nmax, pmin, pmax;
                                    int rangeCap;
                                    if (k < 7)
                                    {
                                        nmin = i - 5;
                                        nmax = i + 5;
                                        pmin = j - 5;
                                        pmax = j + 5;
                                        rangeCap = 100;
                                    }
                                    else
                                    {
                                        nmin = 0;
                                        nmax = cMax;
                                        pmin = 0;
                                        pmax = rMax;
                                        rangeCap = 5;
                                    }

                                    if (rooms[i, j] == 3)
                                    {
                                        for (int n = nmin; n <= nmax; n++)
                                        {
                                            for (int p = pmin; p <= pmax; p++)
                                            {
                                                if (n > 0 && p > 0 && maze[k][n, p] == 0 && rooms[n, p] > 2)
                                                {
                                                    if (Random.Range(0, 1000) < rangeCap)
                                                    {
                                                        /*
                                                        Debug.Log("étage : " + k + " trappe : " + i * 4 + " " + j * 4 +
                                                                  " bouton : " + n * 4 + " " + p * 4);
                                                        */
                                                        //roomLinksList[k].Add(i + ";" + j, n + ";" + p);
                                                        if (!roomTrap[k].ContainsKey(i + ";" + j))
                                                        {
                                                            roomTrap[k].Add(i + ";" + j, nbTrap);
                                                        }

                                                        if(!roomTrap[k].ContainsKey(n+";"+p))
                                                        {
                                                            roomTrap[k].Add(n + ";" + p, nbTrap);
                                                        }
                                                        nbTrap++;
                                                        rooms[n, p] = 2;
                                                        breaker = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if (breaker) break;
                                            if (n == i + 5) n = i - 5;
                                        }
                                    }
                                }
                                else
                                {
                                    int rand = Random.Range(5, 13);
                                    if (rand == 11)
                                    {
                                        rand = Random.Range(5, 13);
                                    }
                                    if (rand == 12 && !itemRoom[k].ContainsKey(i+";"+j))
                                    {
                                        int randItem = Random.Range(0, nbItems);
                                        itemRoom[k].Add(i + ";" + j, randItem);
                                    }
                                    rooms[i, j] = rand;
                                }
                            }
                        }
                    }
                }
                
                int randx;
                int randy;
                do
                {
                    randx = Random.Range(0, sizeCols);
                    randy = Random.Range(0, sizeRows);
                    Debug.Log(randx + " "+randy + " "+k + " "+maze[k][randx, randy]);
                } while (maze[k][randx, randy] != 0 || rooms[randx,randy] == 3|| rooms[randx,randy] == 2);
                
                rooms[randx, randy] = 13;

                floorRooms[k] = rooms;
                if (k < 3) frequency -= 6;
                else frequency-= 4;
            }
            
               
            return floorRooms;
        }
    }
}
