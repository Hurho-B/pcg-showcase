using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    public Vector2 size;
    public Vector2 offset;
    public int startPos = 0;
    public GameObject room;

    List<Cell> board;
    string currentSceneName;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        MazeGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Deletes the existing list + gameObjects
            // Generates a new dungeon
            board.Clear();
            for (int i = (transform.childCount - 1); i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
                SceneManager.LoadScene(currentSceneName);
            }
            MazeGenerator();
        }
    }

    void GenerateDungeon()
    {
        // After MazeGenerator(), new dungeon list is brought in
        // and used to make the actual environment/gameObjects
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];

                if (currentCell.visited)
                {
                    RoomBehaviour newRoom = Instantiate(room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);

                    newRoom.name += " " + i + "-" + j;
                }
            }

        }
    }

    void MazeGenerator()
    {
        // Generating the workable area
        board = new List<Cell>();
        for (int i = 0; i < (size.x * size.y); i++)
        {
            board.Add(new Cell());
        }

        for (int i = 0; i < size.x; i++)
        {

            int currentCell = startPos;
            Stack<int> path = new Stack<int>();
            int k = 0;

            while (k < 1000)
            {
                k++;

                // Marking current cell, checking adjacent cells if
                // they've already been visited
                // This type of check means no 3 or 4 way junctions
                board[currentCell].visited = true;
                List<int> neighbors = CheckNeighbors(currentCell);

                if (currentCell == board.Count - 1)
                {
                    break;
                }


                if (neighbors.Count == 0)
                {
                    if (path.Count == 0)
                    {   
                        break;
                    }
                    else
                    {
                        currentCell = path.Pop();
                    }
                }
                else
                {
                    path.Push(currentCell);
                    int newCell = neighbors[Random.Range(0, neighbors.Count)];

                    // This amalgamation marks which doors and walls
                    // end up being toggled
                    if (newCell > currentCell)
                    {
                        if (newCell - 1 == currentCell)
                        {
                            board[currentCell].status[1] = true;
                            currentCell = newCell;
                            board[currentCell].status[3] = true;
                        }
                        else
                        {
                            board[currentCell].status[2] = true;
                            currentCell = newCell;
                            board[currentCell].status[0] = true;
                        }
                    }
                    else
                    {
                        if (newCell + 1 == currentCell)
                        {
                            board[currentCell].status[3] = true;
                            currentCell = newCell;
                            board[currentCell].status[1] = true;
                        }
                        else
                        {
                            board[currentCell].status[0] = true;
                            currentCell = newCell;
                            board[currentCell].status[2] = true;
                        }
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        // Checks North neighbor
        if (cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - size.x));
        }

        // Checks South neighbor
        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + size.x));

        }

        // Checks East neighbor
        if ((cell + 1) % size.y != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }

        // Checks West neighbor
        if (cell % size.y != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - 1));

        }
        return neighbors;
    }
}
