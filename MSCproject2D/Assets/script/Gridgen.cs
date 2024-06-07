using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridgen : MonoBehaviour
{
    public int width;
    public int height;
    public float fillProbability = 0.5f;

    private Grid3D grid;

    public GameObject tree;

    public GameObject dirt;

    public GameObject Adventurer;

    public GameObject Enemy;

    public GameObject token;

    public Vector3 spawnpoint;

    private string mapcheck;

    public int Seed = 1234;

    

    void Start()
    {
        Random.InitState(Seed);
        //spawnpoint = new Vector3(width/2, 0.58f, height/2);
        spawnpoint = new Vector3(0, 40, -0.58f);
        //Adventurer.transform.position = spawnpoint;
        //Enemy.transform.position = new Vector3(width/2 + 2.0f, 0.5f, height/2);
        GameObject adventurer = Instantiate(Adventurer, spawnpoint, Quaternion.identity);
        //GameObject adventurer = Instantiate(Adventurer, new Vector3(width/2, 0.5f, height/2), Quaternion.identity);
        GenerateGrid();
    }

    void GenerateGrid()
{
    // Initialize the grid with random values
    grid = new Grid3D(width, height, 1);
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            grid[x, y, 0] = (Random.value < fillProbability) ? 1 : 0;
        }
    }

    //widen path
    //widenPath();
    

    /////////////////////////////////////////////////////

    // Apply cellular automata rules
    for (int i = 0; i < 8; i++) // Repeat for a few iterations for smoother results (5)
    {
        Grid3D newGrid = new Grid3D(width, height, 1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                
                if (grid[x,y,0] == 1)
                {
                    if (aliveNeighbors < 2 || aliveNeighbors > 3)
                        newGrid[x, y, 0] = 0; // Cell dies
                    else
                        newGrid[x, y, 0] = 1; // Cell survives
                }
                else
                {
                    if (aliveNeighbors == 3)
                        newGrid[x, y, 0] = 1; // Cell becomes alive
                    else
                        newGrid[x, y, 0] = 0; // Cell remains dead
                }
            }
        }

        // Update the grid with the new values
        grid = newGrid;
    }

    List<Vector3> cellsWithValue0 = grid.FindCellsWithValue(0);
        List<Vector3> randomCells = grid.PickRandomCells(cellsWithValue0, 8);
        foreach (Vector3 cell in randomCells)
        {
            grid[cell] = 2;
        }

    // Now, instantiate objects based on the final grid state
    Quaternion rotation = Quaternion.Euler(0, 0, 90);
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            if (grid[x, y, 0] == 1) //tree
            {
                GameObject newTree = Instantiate(tree, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, 0.0f), rotation);
                newTree.transform.parent = transform;
            }
            else
            {
                GameObject newDirt = Instantiate(dirt, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, 0.0f), rotation);
                newDirt.transform.parent = transform;
            }
            if (grid[x, y, 0] == 2) //token
            {
                GameObject newtoken = Instantiate(token, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, -0.1f), rotation);
                newtoken.transform.parent = transform;
            }

        }
    }
}

int CountAliveNeighbors(int x, int y)
{
    int count = 0;
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            int neighborX = x + i;
            int neighborY = y + j;
            if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
            {
                count += grid[neighborX, neighborY, 0];
            }
        }
    }
    count -= grid[x, y, 0]; // Exclude the cell itself
    return count;
}
/*
void widenPath()
{
    int[,] newGrid1 = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 1)
                {
                for (int i = -2; i <= 1; i++)
                {
                    
                    for (int j = -1; j <= 1; j++)
                    {   
                        int neighborX = x + i;
                        int neighborY = y + j;
                        if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                        {
                            if (newGrid1[neighborX, neighborY] == 1)
                            {
                                newGrid1[x,y] = 0;
                            }
                        }            
                    }
                    
                }
                
                }
                
            }
        }

        // Update the grid with the new values
        grid = newGrid1;
}*/


    /*
    void GenerateGrid()
    {
        grid = new int[width, height];
        Quaternion rotation = Quaternion.Euler(90, 0, 0);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = (Random.value < fillProbability) ? 1 : 0;
                if (grid[x, y] == 1)
                {
                    GameObject newTree = Instantiate(tree, new Vector3(x, 0.0f, y), rotation);
                    newTree.transform.parent = transform;
                }
                else
                {
                    GameObject newDirt = Instantiate(dirt, new Vector3(x, 0.0f, y), rotation);
                    newDirt.transform.parent = transform;
                }
            }
        }
    }
*/

    /*
    void GenerateGrid()
    {
        grid = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = (Random.value < fillProbability) ? 1 : 0;
                if (grid[x, y] == 1)
                {
                    Instantiate(tree);
                    tree.transform.position = new Vector3(x, 0.0f, y);
                }
                if (grid[x, y] == 0)
                {
                    Instantiate(dirt);
                    dirt.transform.position = new Vector3(x, 0.0f, y);
                }
            }
        }
        for (int y=0; y < height; y++)
        {
            for (int x=0; x< width; x++)
            {
                mapcheck += grid[x, y];
            }
            print(mapcheck);
            mapcheck = "";
        }
    }
    */

}
