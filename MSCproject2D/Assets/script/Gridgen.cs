using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gridgen : MonoBehaviour
{
    [Header("Map Setup")]
    public int width;
    public int height;
    public int Seed = 1234;
    public int tokenAmount = 8;
    

    public MapType mapType = MapType.RandomNCA;
    public BlockType blockType = BlockType.None;
    public BackType backType = BackType.None;

    private Grid3D grid;

    [Header("Prefab")]
    public GameObject block;

    public GameObject background;

    public GameObject Adventurer;

    //public GameObject Enemy;

    public GameObject token;

    public Vector3 spawnpoint;

    //private string mapcheck;

    [Header("RandomNCA")]
    public float fillProbability = 0.5f;

    [Header("Perlin")]
    public float Threshold = 0.4f;

    public float NoiseScale = 0.13f;

    private EnumManager enumManager;

    private WaveFunctionCollapse wfc;

    void Start()
    {
        enumManager = GetComponent<EnumManager>();
        wfc = GetComponent<WaveFunctionCollapse>();
        Random.InitState(Seed);
        //spawnpoint = new Vector3(width/2, 0.58f, height/2);
        spawnpoint = new Vector3(0, height/2+5, 0);
        //Adventurer.transform.position = spawnpoint;
        //Enemy.transform.position = new Vector3(width/2 + 2.0f, 0.5f, height/2);
        GameObject adventurer = Instantiate(Adventurer, spawnpoint, Quaternion.identity);
        //GameObject adventurer = Instantiate(Adventurer, new Vector3(width/2, 0.5f, height/2), Quaternion.identity);
        GenerateGrid();
    }

    void GenerateGrid()
    {
        

        switch (mapType)
        {
            case (MapType.RandomNCA):
                randomNoiseNCA();
                break;
            
            case (MapType.Perlin):
                perlinNoise();
                break;

            case (MapType.WFC):
                wfc.GenerateMap(width, height, Seed);
                break;

            default:
                break;
        }
    

        

       
    }

    void randomNoiseNCA()
    {
        //add random value
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = (Random.value < fillProbability) ? 1 : 0;
            }
        }
    

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
        TokenSpawn();
        InstantiateTile();
    }

    void perlinNoise()
    {
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = 0;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * NoiseScale, y * NoiseScale);

                if (perlinValue < Threshold)
                {
                    grid[x, y, 0] = 1;
                }
            }
        }

        TokenSpawn();
        InstantiateTile();
    }

    void TokenSpawn()
    {
        //token spawn
        List<Vector3> cellsWithValue0 = grid.FindCellsWithValue(0);
        List<Vector3> randomCells = grid.PickRandomCells(cellsWithValue0, tokenAmount);
        foreach (Vector3 cell in randomCells)
        {
            grid[cell] = 2;
        }
    }

    void InstantiateTile()
    {
         //Instantiate Tile objects
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y, 0] == 1) //block
                {
                    GameObject newblock = Instantiate(block, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, 0.0f), block.transform.rotation);
                    newblock.transform.parent = transform;
                    enumManager.SetBlockType(newblock, blockType);
                }
                else                    //background
                {
                    GameObject newbackground = Instantiate(background, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, 0.0f), rotation);
                    newbackground.transform.parent = transform;
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


}
