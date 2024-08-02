using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteShaper : MonoBehaviour
{
    public Grid3D noiseMap;
    public SpriteShapeController spriteShapeController;
    public float cellSize = 1.0f;

    public GameObject sprite;

    public GameObject lava;

    public GameObject checkblock;

    /*void Start()
    {
        noiseMap = new Grid3D(10, 10,0); // Example size
        noiseMap.InitializeGrid();
        GenerateNoiseMap();
        GenerateSpriteShapesFromNoiseMap();
    }

    void GenerateNoiseMap()
    {
        // Example noise generation: randomly set some cells to 1
        for (int x = 0; x < noiseMap.Width; x++)
        {
            for (int y = 0; y < noiseMap.Height; y++)
            {
                if (Random.value > 0.5f)
                {
                    noiseMap[x, y, 0] = 1;
                }
            }
        }
    }*/

    public void CreateLava(Grid3D map)
    {
        noiseMap = map;
        List<Vector3> edgePoints = new List<Vector3>();
        for (int i = -100; i<=100; i++)
        {
            edgePoints.Add(new Vector3(i, 0, 0));
        }
        edgePoints.Add(new Vector3(100, -100, 0));
        edgePoints.Add(new Vector3(-100, -100, 0));
        
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        GameObject newsprite = Instantiate(lava, new Vector3(0, noiseMap.Height/2+10, -1), rotation);
        //newsprite.transform.parent = transform;
        spriteShapeController = newsprite.GetComponent<SpriteShapeController>();
        Spline spline = spriteShapeController.spline;
        spline.Clear();
        print(edgePoints.Count);
        for (int i = 0; i < edgePoints.Count; i++)
        {
            //print(edgePoints[i]);
            Vector3 point = edgePoints[i] * cellSize;
            //spline.InsertPointAt(i, new Vector2(point.x-noiseMap.Width/2+0.5f, point.y-noiseMap.Height-9.5f));
            spline.InsertPointAt(i, new Vector2(point.x, point.y-noiseMap.Height/2-10));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        spriteShapeController.RefreshSpriteShape();
        print("createlava");
    }

    public void GenerateSpriteShapesFromNoiseMap(Grid3D map)
    {
        noiseMap = map;
        preprocess();

        List<List<Vector3>> chunks = FindChunks();

        foreach (List<Vector3> chunk in chunks)
        {
            List<Vector3> edgePoints = LoopEdgePoints(chunk);
            CreateSpriteShape(edgePoints);
        }
    }

    void preprocess() // preprocess the map to avoid glitch, e.c. cleaning single layer edgepoint
    {
        for (int i = 0; i < noiseMap.Width; i++)
        {
            for (int j = 0; j < noiseMap.Height; j++)
            {
                if (noiseMap[i, j, 0] == 1)
                {
                    if ((noiseMap[i, j+1, 0] != 1 && noiseMap[i, j-1, 0] != 1) ||
                        (noiseMap[i+1, j, 0] != 1 && noiseMap[i-1, j, 0] != 1)  )
                    {
                        noiseMap[i, j, 0] = 0;
                    }
                }
            }
        }
    }

    List<List<Vector3>> FindChunks()
    {
        List<List<Vector3>> chunks = new List<List<Vector3>>();
        bool[,] visited = new bool[noiseMap.Width, noiseMap.Height];

        for (int y = 0; y < noiseMap.Height; y++)
        {
            for (int x = 0; x < noiseMap.Width; x++)
            {
                if (noiseMap[x, y, 0] == 1 && !visited[x, y])
                {
                    List<Vector3> chunk = new List<Vector3>();
                    FloodFill(x, y, visited, chunk);
                    chunks.Add(chunk);
                }
            }
        }

        return chunks;
    }

    void FloodFill(int x, int y, bool[,] visited, List<Vector3> chunk)
    {
        Stack<Vector3> stack = new Stack<Vector3>();
        stack.Push(new Vector3(x, y, 0));

        while (stack.Count > 0)
        {
            Vector3 pos = stack.Pop();
            int px = Mathf.FloorToInt(pos.x);
            int py = Mathf.FloorToInt(pos.y);

            if (px < 0 || py < 0 || px >= noiseMap.Width || py >= noiseMap.Height || visited[px, py] || noiseMap[px, py, 0] == 0)
                continue;

            visited[px, py] = true;
            chunk.Add(new Vector3(px, py, 0));

            stack.Push(new Vector3(px + 1, py, 0));
            stack.Push(new Vector3(px - 1, py, 0));
            stack.Push(new Vector3(px, py + 1, 0));
            stack.Push(new Vector3(px, py - 1, 0));
        }
    }

    List<Vector3> LoopEdgePoints(List<Vector3> chunk)
    {
        List<Vector3> edgePoints = new List<Vector3>();

        chunk = FindEdgePoints(chunk);
        // Find the starting point (any edge point)
        Vector3 startPoint = chunk[0];
        foreach (var point in chunk)
        {
            if (IsEdgePoint(point))
            {
                startPoint = point;
                break;
            }
        }
        
        // Directions in clockwise order (right, down, left, up)
        Vector3[] directions = { Vector3.right, Vector3.right+Vector3.down,
                                 Vector3.down, Vector3.down+Vector3.left,
                                 Vector3.left, Vector3.left+Vector3.up, 
                                 Vector3.up, Vector3.up+Vector3.right };

        // Trace the boundary
        Vector3 currentPoint = startPoint;
        Vector3 previousDirection = Vector3.up; // Assume we enter the first edge point from above

        int time = 0;
        //print(startPoint);
        do
        {
            time++;
            bool findedge = false;
            noiseMap[currentPoint] = -2;   //value = 2 if the point is add;
            if (chunk.Contains(currentPoint))
            {
                chunk.Remove(currentPoint);
            }
            List<Vector3> edgeCandidates = new List<Vector3>();
            foreach (var direction in directions)
            {
                Vector3 nextPoint = currentPoint + direction;
                if (noiseMap.isInGrid(nextPoint) && IsInChunk(chunk, nextPoint))
                {
                    //print(nextPoint + "/" + IsEdgePoint(nextPoint)+"/"+IsCheck(nextPoint));
                    
                    
                    if (IsEdgePoint(nextPoint) && !IsCheck(nextPoint))
                    {
                        //print("addEdge");
                        edgeCandidates.Add(nextPoint);
                        //break;
                    }
                }
                
            }
            //print(edgeCandidates.Count);
            foreach (var edgec in edgeCandidates)
            {
                if (IsSameOrientation(currentPoint, edgec))
                {
                    //print("a");
                    currentPoint = edgec;
                    //previousDirection = direction;
                    edgePoints.Add(currentPoint);
                    findedge = true;
                    break;
                }
            }

            if (!findedge && chunk.Count>0)
            {
                //print("bb");
                foreach (var edgec in edgeCandidates)
                {
                    if (ShareSameBacktile(currentPoint, edgec))
                    {
                        //print("b");
                        currentPoint = edgec;
                        //previousDirection = direction;
                        edgePoints.Add(currentPoint);
                        findedge = true;
                        break;
                    }
                }
            }

            if (!findedge && chunk.Count>0 && edgeCandidates.Count > 0)
            {
                print("c");
                currentPoint = edgeCandidates[0];
                //previousDirection = direction;
                edgePoints.Add(currentPoint);
                findedge = true;
                    
                
            }

            if (!findedge && chunk.Count>0)
            {
                print("d");
                print("dc"+currentPoint);
                //GameObject newcheck = Instantiate(checkblock, currentPoint, Quaternion.identity);
                //newcheck.transform.SetParent(transform);
                Vector3 nextPoint =FindNearestEdgePoint(chunk,currentPoint);
                print("dn"+nextPoint);
                currentPoint = nextPoint;
                //edgePoints.Add(currentPoint);
                chunk.Remove(nextPoint);
                findedge = true;
                //break;
            }



        } while (chunk.Count>0);//currentPoint != startPoint && time <= 80 && 
        edgePoints.Add(startPoint);
        //noiseMap[startPoint] = 2;
        //print(time);
        //print("chunkcc"+chunk.Count);
        return edgePoints;
    }

    List<Vector3> FindEdgePoints(List<Vector3> chunk) //find all edgepoints in a chunk
    {
        List<Vector3> edgePoints = new List<Vector3>();

        foreach (Vector3 point in chunk)
        {

            if (IsEdgePoint(point))
            {
                edgePoints.Add(point);
            }
        }

        return edgePoints;
    }

    Vector3 FindNearestEdgePoint(List<Vector3> chunk, Vector3 point) //find nearest edgepoint from input point
    {
        float min = noiseMap.Width;
        Vector3 Nearest = point;
        foreach (Vector3 p in chunk)
        {
            if (IsEdgePoint(p))
            {
                if(Vector3.Distance(point, p) < min)
                {
                    min = Vector3.Distance(point, p);
                    Nearest = p;
                }
            }
        }
        return Nearest;
    }

    bool IsEdgePoint(Vector3 point) //check if is edgepoint 
    {
        int x = Mathf.FloorToInt(point.x);
        int y = Mathf.FloorToInt(point.y);

        if (noiseMap[x, y, 0] == 0) return false;

        int[,] directions = new int[,]
        {
            {0, 1},
            {1, 0},
            {0, -1},
            {-1, 0}
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int nx = x + directions[i, 0];
            int ny = y + directions[i, 1];

            if (nx < 0 || ny < 0 || nx >= noiseMap.Width || ny >= noiseMap.Height || noiseMap[nx, ny, 0] == 0)
            {
                return true;
            }
        }

        return false;
    }

    bool IsCheck(Vector3 point) //if is check, value should be 2
    {
        if (noiseMap[point] == -2) { return true; }
        else { return false; }
    }

    bool IsInChunk(List<Vector3> chunk, Vector3 point) // if a point is in given chunk
    {
        foreach (Vector3 p in chunk)
        {
            if (point == p) { return true; } 
            
        }
        return false; 
    }

    bool IsSameOrientation(Vector3 pointa, Vector3 pointb) //if two point were facing same way to background tile
    {
        Vector3[] directions = { Vector3.right, 
                                 Vector3.down, 
                                 Vector3.left, 
                                 Vector3.up, };
        foreach (var d in directions)
        {
            if (noiseMap[pointa+d]==0 || noiseMap[pointa+d]==-1)
            {
                //if (noiseMap[pointa+d] == noiseMap[pointb+d])
                if (noiseMap[pointb+d]==0 || noiseMap[pointb+d]==-1)
                {
                    //print(noiseMap[pointa+d]);
                    return true;
                }
            }
            
        }
        return false;
    }

    bool ShareSameBacktile(Vector3 pointa, Vector3 pointb) //share same neighbor backgroun tile
    {
        List<Vector3> neighbora = new List<Vector3>();
        List<Vector3> neighborb = new List<Vector3>();
        Vector3[] directions = { Vector3.right, 
                                 Vector3.down, 
                                 Vector3.left, 
                                 Vector3.up, };
        foreach (var d in directions)
        {
            if(noiseMap[pointa+d]==0 || noiseMap[pointa+d]==-1) 
            {
                neighbora.Add(pointa+d);
            }
            if(noiseMap[pointb+d]==0 || noiseMap[pointb+d]==-1) 
            {
                neighborb.Add(pointb+d);
            }
        }
        foreach(var a in neighbora)
        {
            foreach(var b in neighborb)
            {
                if (a==b)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void CreateSpriteShape(List<Vector3> edgePoints)
    {
        //print("createsprite");
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        //GameObject newsprite = Instantiate(sprite, new Vector3(0, 0, 0), rotation); //noiseMap.Height/2+5
        GameObject newsprite = Instantiate(sprite, new Vector3(0, noiseMap.Height/2+10, 0), rotation);
        //newsprite.transform.position += new Vector3((float)noiseMap.Width/2-0.5f, (float)noiseMap.Height/2-0.5f, 0.5f);
        newsprite.transform.parent = transform;
        spriteShapeController = newsprite.GetComponent<SpriteShapeController>();
        Spline spline = spriteShapeController.spline;
        spline.Clear();
        //print(edgePoints.Count);
        for (int i = 0; i < edgePoints.Count; i++)
        {
            //print(edgePoints[i]);
            Vector3 point = edgePoints[i] * cellSize;
            spline.InsertPointAt(i, new Vector2(point.x-noiseMap.Width/2+0.5f, point.y-noiseMap.Height-9.5f));
            spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        }

        spriteShapeController.RefreshSpriteShape();
        //newsprite.transform.position -= new Vector3((float)noiseMap.Width/2-0.5f, (float)noiseMap.Height/2-0.5f, 0.5f);
        //newsprite.transform.position -= new Vector3(20f, 20f, 0.5f);
    }
}
