using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    private int mapWidth;
    private int mapHeight;
    //public List<TileData> tileTypes;
    [System.Serializable]
    public class TileTypes
    {
        public List<TileData> tileTypes;
    }
    [System.Serializable]
    public class WFCMapTypes
    {
        public List<TileTypes> mapTypes;
    }

    public WFCMapTypes MAP = new WFCMapTypes();


    private TileData[,] map;
    private Dictionary<Vector2Int, List<TileData>> possibleTiles;

    private int mapID;

    private Grid3D tokengrid; //grid map for token generation

    public GameObject background;

    /*void Start()
    {
        GenerateMap();
    }*/

    public void GenerateMap(int Width, int Height, int seed, int mapT)
    {
        mapWidth = Width;
        mapHeight = Height;
        Random.InitState(seed);
        mapID = mapT;
        map = new TileData[mapWidth, mapHeight];
        tokengrid = new Grid3D(mapWidth, mapHeight, 1);
        possibleTiles = new Dictionary<Vector2Int, List<TileData>>();

        // Initialize possible tiles for each position
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                possibleTiles[position] = new List<TileData>(MAP.mapTypes[mapID].tileTypes);
            }
        }

        //available position
        List<Vector2Int> openPositions = new List<Vector2Int>();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                openPositions.Add(new Vector2Int(x, y));
            }
        }

        // Generate the map using WFC
        while (openPositions.Count > 0)
        {
            //randomly pick a available position
            /*
            int randomIndex = Random.Range(0, openPositions.Count);
            Vector2Int position = openPositions[randomIndex];
            openPositions.RemoveAt(randomIndex);*/
            Vector2Int position = GetPositionWithLeastEntropy(openPositions);
            openPositions.Remove(position);

            // Select a tile for this position
            TileData selectedTile = SelectTileForPosition(position);
            map[position.x, position.y] = selectedTile;

            // Instantiate the tile at the position
            GameObject PlaceTile = Instantiate(selectedTile.tilePrefab, new Vector3(position.x-(float)mapWidth/2+0.5f, position.y-(float)mapHeight/2+0.5f, 0), selectedTile.tilePrefab.transform.rotation);
            PlaceTile.transform.parent = transform;

            //mark on tokengrid
            if (selectedTile.tilePrefab == background)
            {
                tokengrid[position.x, position.y, 0] = 0;
            }
            else
            {
                tokengrid[position.x, position.y, 0] = 1;
            }



            // Propagate constraints to neighbors
            PropagateConstraints(position, selectedTile);
        }
    }

    Vector2Int GetPositionWithLeastEntropy(List<Vector2Int> openPositions)
    {
        Vector2Int bestPosition = openPositions[0];
        int smallestEntropy = possibleTiles[bestPosition].Count;

        foreach (Vector2Int position in openPositions)
        {
            int entropy=0;
            if (possibleTiles[position].Count > 0)
            {
                entropy = possibleTiles[position].Count;
            }
            
            if (entropy < smallestEntropy)
            {
                bestPosition = position;
                smallestEntropy = entropy;
            }
        }

        return bestPosition;
    }

    TileData SelectTileForPosition(Vector2Int position)
    {
        List<TileData> possible = possibleTiles[position];
        if (possible.Count > 0)
        {
            return possible[Random.Range(0, possible.Count)];
        }
        else
        {
            //print("out of options");
            return MAP.mapTypes[mapID].tileTypes[0];
        }
        
    }

    void PropagateConstraints(Vector2Int position, TileData placedTile)
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(1, 0)    // Right
        };

        List<TileData>[] allowedNeighbors = new List<TileData>[]
        {
            placedTile.allowedNeighborsUp,
            placedTile.allowedNeighborsDown,
            placedTile.allowedNeighborsLeft,
            placedTile.allowedNeighborsRight
        };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int neighborPosition = position + directions[i];
            if (IsWithinBounds(neighborPosition))
            {
                List<TileData> neighborPossibleTiles = possibleTiles[neighborPosition];
                neighborPossibleTiles.RemoveAll(tile => !allowedNeighbors[i].Contains(tile));
                if (neighborPossibleTiles.Count == 0)
                {
                    //print("123");
                    neighborPossibleTiles.Add(MAP.mapTypes[mapID].tileTypes[0]);
                }
            }
        }
    }

    bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < mapWidth && position.y >= 0 && position.y < mapHeight;
    }

    public Grid3D getWFCTokenGrid()
    {
        return tokengrid;
    }
}
