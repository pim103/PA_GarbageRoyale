using System.Collections.Generic;
using UnityEngine;

namespace GarbageRoyale.Scripts
{
    public class MazeMeshGenerator : MonoBehaviour
    {    
        // generator params
        public float width;     // how wide are hallways
        public float height;    // how tall are hallways

        private int nbPipe;
        private int nbDoors;

        public Dictionary<int, GameObject> pipes = new Dictionary<int, GameObject>();
        public Dictionary<GameObject, int> buttonsTrap = new Dictionary<GameObject, int>();
        public Dictionary<int, GameObject> buttonsTrapReversed = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> trap = new Dictionary<int, GameObject>();
        public Dictionary<int, GameObject> doors = new Dictionary<int, GameObject>();

        public MazeMeshGenerator()
        {
            nbPipe = 0;
            width = 4f;
            height = 4f;
        }

        public Mesh FromData(int[,] data, int ypos, GameObject [] Prefabs, int[,] rooms, GameObject floorTransition, Dictionary<string, int> idTrap, Dictionary<string, int> itemRoom, bool isReplay)
        {
            Mesh maze = new Mesh();
    
            //3
            List<Vector3> newVertices = new List<Vector3>();
            List<Vector2> newUVs = new List<Vector2>();
    
            maze.subMeshCount = 2;
            List<int> floorTriangles = new List<int>();
            List<int> wallTriangles = new List<int>();
    
            int rMax = data.GetUpperBound(0);
            int cMax = data.GetUpperBound(1);
            float halfH = height * .5f + ypos;
    
            //4
            for (int i = 0; i <= rMax; i++)
            {
                for (int j = 0; j <= cMax; j++)
                {
                    if((i == 40 && j == 40) || (i == 44 && j == 44))
                    {
                        /*
                        var newDoor = Instantiate(Prefabs[9], new Vector3(j * width + ypos, ypos, i * width + ypos),
                                        Quaternion.identity);
                        newDoor.GetComponent<OpenDoorScript>().doorId = nbDoors;
                        doors.Add(nbDoors, newDoor);
                        nbDoors++;

                        continue;
                        */
                        GameObject ItemRoom = Instantiate(Prefabs[12], new Vector3(j * width + ypos, ypos, i * width + ypos), Quaternion.identity);
                        ItemSpawner itemSpawn = ItemRoom.GetComponent<ItemSpawner>();

                        //Debug.Log(itemRoom[i + ";" + j]);

                        itemSpawn.itemType = 8;
                        itemSpawn.initItems();
                    }

                    if (data[i, j] == 0)
                    {
                        if (rooms[i, j] != 9999 && rooms[i, j] != 9)
                        {
                            if(rooms[i, j] == 3)
                            {
                                // Add door
                                var newTrap = Instantiate(Prefabs[rooms[i,j]], new Vector3(j * width+ypos, ypos, i * width+ypos), Quaternion.identity);
                                //Debug.Log(idTrap[i + ";" + j]);
                                trap.Add(idTrap[i+";"+j], newTrap);
                            }
                            else if (rooms[i, j] == 2)
                            {
                                // Add button
                                var newButton = Instantiate(Prefabs[rooms[i, j]], new Vector3(j * width + ypos, ypos, i * width + ypos), Quaternion.identity);
                                //Debug.Log(idTrap[i + ";" + j]);
                                buttonsTrap.Add(newButton, idTrap[i + ";" + j]);
                                buttonsTrapReversed.Add(idTrap[i + ";" + j], newButton);
                            }
                            //TODO Instantiate Random ItemRoom
                            else if(rooms[i, j] == 12)
                            {
                                if(!isReplay)
                                {
                                    GameObject ItemRoom = Instantiate(Prefabs[12], new Vector3(j * width + ypos, ypos, i * width + ypos), Quaternion.identity);
                                    ItemSpawner itemSpawn = ItemRoom.GetComponent<ItemSpawner>();

                                    //Debug.Log(itemRoom[i + ";" + j]);

                                    itemSpawn.itemType = itemRoom[i + ";" + j];
                                    itemSpawn.initItems();
                                }
                                else
                                {
                                    Instantiate(Prefabs[8], new Vector3(j * width + ypos, ypos, i * width + ypos), Quaternion.identity);
                                }
                            }
                            else
                            {
                                Instantiate(Prefabs[rooms[i, j]], new Vector3(j * width + ypos, ypos, i * width + ypos), Quaternion.identity);
                            }
                        } else if (rooms[i, j] == 9)
                        {
                            int rand = Random.Range(0, 2);
                            if (rooms[i, j-1] == 9999 && rooms[i, j+1] == 9999)
                            {
                                if (rand == 0)
                                {
                                    var newDoor = Instantiate(Prefabs[rooms[i, j]], new Vector3(j * width + ypos, ypos, i * width + ypos),
                                        Quaternion.identity);
                                    newDoor.GetComponent<OpenDoorScript>().doorId = nbDoors;
                                    doors.Add(nbDoors, newDoor);
                                    nbDoors++;
                                }
                                else
                                {
                                    var newDoor = Instantiate(Prefabs[rooms[i, j]], new Vector3(j * width + ypos, ypos, i * width + ypos),
                                        Quaternion.Euler(new Vector3(0,180,0)));
                                    newDoor.GetComponent<OpenDoorScript>().doorId = nbDoors;
                                    doors.Add(nbDoors, newDoor);
                                    nbDoors++;
                                }
                                
                            } else if (rooms[i+1, j] == 9999 && rooms[i-1,j] == 9999)
                            {
                                if (rand == 0)
                                {
                                    var newDoor = Instantiate(Prefabs[rooms[i, j]],
                                        new Vector3(j * width + ypos, ypos, i * width + ypos),
                                        Quaternion.Euler(new Vector3(0, 90, 0)));
                                    newDoor.GetComponent<OpenDoorScript>().doorId = nbDoors;
                                    doors.Add(nbDoors, newDoor);
                                    nbDoors++;
                                }
                                else
                                {
                                    var newDoor = Instantiate(Prefabs[rooms[i, j]],
                                        new Vector3(j * width + ypos, ypos, i * width + ypos),
                                        Quaternion.Euler(new Vector3(0, -90, 0)));
                                    newDoor.GetComponent<OpenDoorScript>().doorId = nbDoors;
                                    doors.Add(nbDoors, newDoor);
                                    nbDoors++;
                                }
                            }
                            else
                            {
                                Instantiate(Instantiate(Prefabs[8], new Vector3(j * width + ypos, ypos, i * width + ypos), Quaternion.identity));
                            }
                        }
                        // floor
                        if (rooms[i, j] != 1)
                        {
                            AddQuad(Matrix4x4.TRS(
                                new Vector3(j * width +ypos , ypos, i * width + ypos ),
                                Quaternion.LookRotation(Vector3.up),
                                new Vector3(width, width, 1)
                            ), ref newVertices, ref newUVs, ref floorTriangles);
                        } 

                        // ceiling
                        if (rooms[i, j] != 0 && rooms[i, j] != 3)
                        {
                            AddQuad(Matrix4x4.TRS(
                                new Vector3(j * width + ypos , height + ypos, i * width + ypos ),
                                Quaternion.LookRotation(Vector3.down),
                                new Vector3(width, width, 1)
                            ), ref newVertices, ref newUVs, ref floorTriangles);
                        } else Instantiate(floorTransition, new Vector3(j * width+ypos, ypos+4, i * width+ypos), Quaternion.identity);


                        // walls on sides next to blocked grid cells
    
                        if (i - 1 < 0 || data[i-1, j] >= 1)
                        {
                            AddQuad(Matrix4x4.TRS(
                                new Vector3(j * width+ypos, halfH, (i-.5f) * width+ypos),
                                Quaternion.LookRotation(Vector3.forward),
                                new Vector3(width, height, 1)
                            ), ref newVertices, ref newUVs, ref wallTriangles);

                            if (i - 1 >= 0 && data[i - 1, j] == 2)
                            {
                                GameObject newPipe = Instantiate(Prefabs[4], new Vector3(j * width + ypos, halfH - 0.5f, (i - .5f) * width + ypos), Quaternion.Euler(new Vector3(0, 0, 0)));
                                newPipe.GetComponent<PipeScript>().pipeIndex = nbPipe;
                                pipes.Add(nbPipe, newPipe.transform.gameObject);
                                nbPipe++;
                            }
                        }
    
                        if (j + 1 > cMax || data[i, j+1] >= 1)
                        {
                            AddQuad(Matrix4x4.TRS(
                                new Vector3((j+.5f) * width+ypos, halfH, i * width+ypos),
                                Quaternion.LookRotation(Vector3.left),
                                new Vector3(width, height, 1)
                            ), ref newVertices, ref newUVs, ref wallTriangles);

                            if(j+1 <= cMax && data[i, j + 1] == 2)
                            {
                                GameObject newPipe = Instantiate(Prefabs[4], new Vector3((j + .5f) * width + ypos, halfH - 0.5f, i * width + ypos), Quaternion.Euler(new Vector3(0, 270, 0)));
                                newPipe.GetComponent<PipeScript>().pipeIndex = nbPipe;
                                pipes.Add(nbPipe, newPipe.transform.gameObject);
                                nbPipe++;
                            }
                        }
    
                        if (j - 1 < 0 || data[i, j-1] >= 1)
                        {
                            AddQuad(Matrix4x4.TRS(
                                new Vector3((j-.5f) * width+ypos, halfH, i * width+ypos),
                                Quaternion.LookRotation(Vector3.right),
                                new Vector3(width, height, 1)
                            ), ref newVertices, ref newUVs, ref wallTriangles);

                            if (j - 1 >= 0 && data[i, j - 1] == 2)
                            {
                                GameObject newPipe = Instantiate(Prefabs[4], new Vector3((j - .5f) * width + ypos, halfH - 0.5f, i * width + ypos), Quaternion.Euler(new Vector3(0, 90, 0)));
                                newPipe.GetComponent<PipeScript>().pipeIndex = nbPipe;
                                pipes.Add(nbPipe, newPipe.transform.gameObject);
                                nbPipe++;
                            }
                        }
    
                        if (i + 1 > rMax || data[i+1, j] >= 1)
                        {
                            AddQuad(Matrix4x4.TRS(
                                new Vector3(j * width+ypos, halfH, (i+.5f) * width+ypos),
                                Quaternion.LookRotation(Vector3.back),
                                new Vector3(width, height, 1)
                            ), ref newVertices, ref newUVs, ref wallTriangles);

                            if (i + 1 <= rMax && data[i + 1, j] == 2)
                            {
                                GameObject newPipe = Instantiate(Prefabs[4], new Vector3(j * width + ypos, halfH - 0.5f, (i + .5f) * width + ypos), Quaternion.Euler(new Vector3(0, 180, 0)));
                                newPipe.GetComponent<PipeScript>().pipeIndex = nbPipe;
                                pipes.Add(nbPipe, newPipe.transform.gameObject);
                                nbPipe++;
                            }
                        }
                    }
                }
            }
    
            maze.vertices = newVertices.ToArray();
            maze.uv = newUVs.ToArray();
        
            maze.SetTriangles(floorTriangles.ToArray(), 0);
            maze.SetTriangles(wallTriangles.ToArray(), 1);
    
            //5
            maze.RecalculateNormals();
            //Instantiate(Prefabs[13], new Vector3(150, ypos, 150), Quaternion.identity);
            return maze;
        }

        //1, 2
        private void AddQuad(Matrix4x4 matrix, ref List<Vector3> newVertices,
            ref List<Vector2> newUVs, ref List<int> newTriangles)
        {
            int index = newVertices.Count;
    
            // corners before transforming
            Vector3 vert1 = new Vector3(-.5f, -.5f, 0);
            Vector3 vert2 = new Vector3(-.5f, .5f, 0);
            Vector3 vert3 = new Vector3(.5f, .5f, 0);
            Vector3 vert4 = new Vector3(.5f, -.5f, 0);
    
            newVertices.Add(matrix.MultiplyPoint3x4(vert1));
            newVertices.Add(matrix.MultiplyPoint3x4(vert2));
            newVertices.Add(matrix.MultiplyPoint3x4(vert3));
            newVertices.Add(matrix.MultiplyPoint3x4(vert4));
    
            newUVs.Add(new Vector2(1, 0));
            newUVs.Add(new Vector2(1, 1));
            newUVs.Add(new Vector2(0, 1));
            newUVs.Add(new Vector2(0, 0));
    
            newTriangles.Add(index+2);
            newTriangles.Add(index+1);
            newTriangles.Add(index);
    
            newTriangles.Add(index+3);
            newTriangles.Add(index+2);
            newTriangles.Add(index);
        }
        
    }
}
