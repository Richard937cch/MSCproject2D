using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;


public class Gridgen : MonoBehaviour
{
    public bool editorValue = false;

    [Header("Map Setup")]
    public int width;
    public int height;
    public int Seed = 1234;
    public int tokenAmount = 8;
    public int hidentokenAmount = 8;
    public int invincibleAmount = 3;
    public int hideninvincibleAmount = 3;

    public MapType mapType = MapType.RandomNCA;
    public int WFCmap = 0;
    public BlockType blockType = BlockType.None;
    public BackType backType = BackType.None;
    public bool LavaMode = false;

    private Grid3D grid;

    [Header("Prefab")]
    public GameObject block;

    public GameObject background;

    public GameObject Adventurer;

    //public GameObject Enemy;

    public GameObject token;
    public GameObject invincible;

    public Vector3 spawnpoint;

    //private string mapcheck;

    [Header("RandomNCA")]
    public float fillProbability = 0.5f;

    [Header("Perlin")]
    public float Threshold = 0.4f;

    public float NoiseScale = 0.13f;

    private EnumManager enumManager;

    private WaveFunctionCollapse wfc;
    private SpriteShaper spriteShaper;
    private MazeGen mazeGen;
    
    public MapSettings mapSettings;
    
    private Stopwatch stopwatch;
    private Stopwatch stopwatch2;

    long Memory;
    void Start()
    {
        enumManager = GetComponent<EnumManager>();
        wfc = GetComponent<WaveFunctionCollapse>();
        spriteShaper = GetComponent<SpriteShaper>();
        mazeGen = GetComponent<MazeGen>();
        
        MenuParameter(); //receive setup from main manu
        Random.InitState(Seed);

        //spawn player
        spawnpoint = new Vector3(0, height/2+5, -0.3f);
        GameObject adventurer = Instantiate(Adventurer, spawnpoint, Quaternion.identity);
        stopwatch = new Stopwatch();
        stopwatch2 = new Stopwatch();
        GenerateGrid(); //Generating Map
        
    }

    void Update()
    {
        //AstarPath.active.Scan();
    }

    void GenerateGrid()
    {
        //int a=0;
        //print((MapType)a);
        long initialMemory = System.GC.GetTotalMemory(false);
        stopwatch.Start();
        stopwatch2.Start();
        switch (mapType)
        {
            case (MapType.RandomNCA):
                randomNoiseNCA();
                break;
            
            case (MapType.Perlin):
                perlinNoise("square");
                break;

            case (MapType.WFC):
                WaveFunctionCollapseMap(-1);
                break;
            case (MapType.WFC1):
                WaveFunctionCollapseMap(2);
                break;
            case (MapType.WFC2):
                WaveFunctionCollapseMap(3);
                break;
            case (MapType.SmoothPerlin):
                perlinNoise("smooth");
                break;
            case (MapType.Maze):
                Maze();
                break;
            case (MapType.Dot):
                DotMap();
                break;
            case (MapType.Flat):
                FlatMap("square");
                break;
            case (MapType.SmoothFlat):
                FlatMap("smooth");
                break;
            default:
                break;
        }

        
        stopwatch2.Stop();
        long finalMemory = System.GC.GetTotalMemory(false);
        SetTileBlockType(); //set block type (background or block)
        ScoreTokenSpawn();  //spawn score token
        PerkSpawn();        //spawn perk token
        Lava();             //spawn lava if lava mode is true
        

        AstarPath.active.Scan();
        print("mapGenerated");
        UnityEngine.Debug.Log($"Map generation took {stopwatch.ElapsedMilliseconds} ms.");
        UnityEngine.Debug.Log($"Map generation including tile took {stopwatch2.ElapsedMilliseconds} ms.");
        UnityEngine.Debug.Log($"Memory used for grid generation: {(Memory - initialMemory) / 1024.0f} KB");
        UnityEngine.Debug.Log($"Memory used for map generation: {(finalMemory - initialMemory) / 1024.0f} KB");
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
        stopwatch.Stop();
        Memory = System.GC.GetTotalMemory(false);
        InstantiateTile();
        
    }

    void perlinNoise(string ismooth)
    {
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = 0;
            }
        }

        float offsetX = Random.Range(-100000, 100000);
        float offsetY = Random.Range(-100000, 100000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float perlinValue = Mathf.PerlinNoise((x + offsetX) * NoiseScale, (y + offsetY) * NoiseScale);

                if (perlinValue < Threshold)
                {
                    grid[x, y, 0] = 1;
                }
            }
        }
        stopwatch.Stop();
        Memory = System.GC.GetTotalMemory(false);
        if (ismooth == "smooth")
        {
            //InstantiateTile();
            spriteShaper.GenerateSpriteShapesFromNoiseMap(grid);
            
        }
        else
        {
            InstantiateTile();
        }
        
        
    }

    void WaveFunctionCollapseMap(int wfcIndex)
    {
        grid = new Grid3D(width, height, 1);
        if (wfcIndex != -1) {WFCmap = wfcIndex;}
        wfc.GenerateMap(width, height, Seed, WFCmap);
        
        grid = wfc.getWFCTokenGrid();
        stopwatch.Stop();
        Memory = System.GC.GetTotalMemory(false);
        InstantiateTile();
    }

    void Maze()
    {
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = 0;
            }
        }
        grid = mazeGen.MazeGene(grid);
        stopwatch.Stop();
        Memory = System.GC.GetTotalMemory(false);
        InstantiateTile();
    }

    void DotMap()
    {
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = (Random.value < fillProbability) ? 1 : 0;
            }
        }

        //isolation
        isolation(3,true);

        stopwatch.Stop();
        Memory = System.GC.GetTotalMemory(false);
        InstantiateTile();
    }
    
    void FlatMap(string ismooth)
    {
        grid = new Grid3D(width, height, 1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y, 0] = 1;
            }
        }
        stopwatch.Stop();
        Memory = System.GC.GetTotalMemory(false);
        if (ismooth == "smooth")
        {
            //InstantiateTile();
            spriteShaper.GenerateSpriteShapesFromNoiseMap(grid);
            
        }
        else
        {
            InstantiateTile();
        }
        
        
    }

    void ScoreTokenSpawn()
    {
        TokenSpawn(token, tokenAmount, hidentokenAmount, 2);
        //HidenTokenSpawn(token, hidentokenAmount, 2);
    }

    void PerkSpawn()
    {
        TokenSpawn(invincible, invincibleAmount, hideninvincibleAmount, 3);
        //HidenTokenSpawn(invincible, hideninvincibleAmount, 3);
    }

    void TokenSpawn(GameObject TokenPrefab, int TAmount, int HTAmount, int tokenID)
    {
        List<Vector3> cellsWithValue0 = grid.FindCellsWithValue(0);
        List<Vector3> randomCells = grid.PickRandomCells(cellsWithValue0, TAmount);
        foreach (Vector3 cell in randomCells)
        {
            grid[cell] = tokenID;
        }
        //hiden (disable when block cannot be destroy => blocktype(reaction) != none)
        if (blockType != BlockType.None)
        {
            List<Vector3> cellsWithValue1 = grid.FindCellsWithValue(1);
            List<Vector3> randomCells1 = grid.PickRandomCells(cellsWithValue1, HTAmount);
            foreach (Vector3 cell in randomCells1)
            {
                grid[cell] = tokenID;
            }
        }
        
        //Instantiate token prefab
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y, 0] == tokenID) //token
                {
                    GameObject newperk = Instantiate(TokenPrefab, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, -0.1f), rotation);
                    newperk.transform.parent = transform;
                }

            }
        }
    }

    /*void HidenTokenSpawn(GameObject TokenPrefab, int TAmount, int tokenID)
    {
        List<Vector3> cellsWithValue0 = grid.FindCellsWithValue(1);
        List<Vector3> randomCells = grid.PickRandomCells(cellsWithValue0, TAmount);
        foreach (Vector3 cell in randomCells)
        {
            grid[cell] = tokenID;
        }
        //Instantiate token prefab
        Quaternion rotation = Quaternion.Euler(0, 0, 90);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y, 0] == tokenID) //token
                {
                    GameObject newperk = Instantiate(TokenPrefab, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, 0.0f), rotation);
                    newperk.transform.parent = transform;
                }

            }
        }
    }*/

    void Lava()
    {
        if (LavaMode)
        {
            spriteShaper.CreateLava(grid);
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
                    GameObject newblock = Instantiate(block, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, -0.2f), block.transform.rotation);
                    newblock.transform.parent = transform;
                    //enumManager.SetBlockType(newblock, blockType);
                }
                else                    //background
                {
                    GameObject newbackground = Instantiate(background, new Vector3(x-(float)width/2+0.5f, y-(float)height/2+0.5f, 0.1f), rotation);
                    newbackground.transform.parent = transform;
                }
                

            }
        }
    }

    void MenuParameter() //if not using editor setup, use setup from main menu
    {
        if (!editorValue)
        {
            width = mapSettings.width;
            height = mapSettings.height;
            tokenAmount = mapSettings.scoreTokenAmount;
            invincibleAmount = mapSettings.perkTokenAmount;
            hidentokenAmount = mapSettings.hidenScoreTokenAmount;
            hideninvincibleAmount = mapSettings.hidenPerkTokenAmount;
            mapType = mapSettings.mapType;
            blockType = mapSettings.blockType;
            backType = mapSettings.backType;
            Seed = mapSettings.seed;
            LavaMode = mapSettings.lavaMode;
            Time.timeScale = 1;
        }
        
    }

    void SetTileBlockType()
    {
        foreach (Transform child in transform)
        {
            
            // Get the ChildScript component from the child
            BlockReaction childScript = child.GetComponent<BlockReaction>();

            if (childScript != null)
            {
                if (childScript.tiletype != TileType.BackGrounds)
                {
                    enumManager.SetBlockType(childScript, blockType);
                }
            }
            /*else
            {
                Debug.LogWarning(child.name + " does not have a ChildScript component.");
            }*/
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

    void isolation(int dis, bool random)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y, 0] == 1)
                {
                    isolating(x, y, dis, random);
                }
            }
        }
        
    }
    
    void isolating(int x, int y, int dist, bool random)
    {
        int dis = dist;
        if (random)
        {
            dis = Random.Range(2, dist);
        }

        for (int i = dis*-1; i <= dis; i++)
        {
            for (int j = dis*-1; j <= dis; j++)
            {
                int neighborX = x + i;
                int neighborY = y + j;
                if (grid.isInGrid(neighborX, neighborY, 0) && !(neighborX==x && neighborY==y))
                {
                    grid[neighborX, neighborY, 0] = 0;
                }
            }
        }
    }


}
