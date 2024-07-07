using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGen : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject wallPrefab;
    public float wallThickness = 0.1f;
    public float cellSize = 1.0f;

    private Grid3D grid;


    void Start()
    {
        //GenerateMaze();
        //DrawMaze();
    }

    public Grid3D MazeGene(Grid3D map)
    {
        width = (map.Width/2) * 2;
        height = (map.Height/2) * 2;
        //intialize map with 1 (all wall)
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = 1;
            }
        }
        GenerateMaze(1,1);

        //int offsetx = map.Width - width;
        //int offsety = map.Height - height;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y, 0] = grid[x, y, 0];
            }
        }
        return map;
        
    }

    /*void GenerateMaze()
    {
        //intialize map with 1 (all wall)
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = 1;
            }
        }

        // Start the maze generation from the top-left corner
        GenerateMaze(1, 1);
    }*/

    void GenerateMaze(int x, int y)
    {
        //visited(x,y,0) = true;
        //print("m");
        grid[x, y, 0] = 0; // visited, set value to 0 (path)

        while (true)
        {
            List<int> directions = new List<int> { 0, 1, 2, 3 }; // Up, Right, Down, Left
            Shuffle(directions);

            bool moved = false;

            foreach (int direction in directions)
            {
                int nx = x;
                int ny = y;

                int step = 2;
                switch (direction)
                {
                    case 0: ny+=step; break;
                    case 1: nx+=step; break;
                    case 2: ny-=step; break;
                    case 3: nx-=step; break;
                }

                if (nx >= 0 && ny >= 0 && nx < width && ny < height && !visited(nx, ny, 0))
                {
                    DeWall(x, y, direction);
                    //DeWall(nx, ny, ((direction + 2) % 4));

                    GenerateMaze(nx, ny);
                    moved = true;
                }
            }

            if (!moved) break;
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }

    bool visited(int x, int y, int z)
    {
        if (grid[x,y,z] == 0)
        {
            return true;
        }
        return false;
    }

    void DeWall(int x, int y, int direction)
    {
        Vector3[] directions = { Vector3.up, 
                                 Vector3.right, 
                                 Vector3.down, 
                                 Vector3.left, };
        Vector3 vec = new Vector3(x, y, 0) + directions[direction];
        grid[vec] = 0; 
        
    }

    /*void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.walls[0])
                    CreateWall(new Vector3(x * cellSize,  (y + 0.33f) * cellSize, 0), new Vector3(cellSize, wallThickness, wallThickness));
                if (cell.walls[1])
                    CreateWall(new Vector3((x + 0.33f) * cellSize,  y * cellSize, 0), new Vector3(wallThickness, cellSize, wallThickness));
                if (cell.walls[2])
                    CreateWall(new Vector3(x * cellSize,  (y - 0.33f) * cellSize, 0), new Vector3(cellSize, wallThickness, wallThickness));
                if (cell.walls[3])
                    CreateWall(new Vector3((x - 0.33f) * cellSize,  y * cellSize, 0), new Vector3(wallThickness, cellSize, wallThickness));
            }
        }
    }

    void CreateWall(Vector3 position, Vector3 scale)
    {
        GameObject wall = Instantiate(wallPrefab, position, Quaternion.identity);
        wall.transform.localScale = scale;
    }*/

    /*class Cell
    {
        public bool[] walls = { true, true, true, true }; // Up, Right, Down, Left
        public bool visited = false;
    }*/
}
