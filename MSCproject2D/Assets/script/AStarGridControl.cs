using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarGridControl : MonoBehaviour
{
    public Vector3 newRotation;
    GridGraph gridGraph;
    GridGraphEditor gridGraphEditor;
    GameObject mapGen;


    void Start()
    {
        // Find the AstarPath component
        AstarPath astarPath = Object.FindFirstObjectByType<AstarPath>();
        mapGen = GameObject.Find("MapGenerator");
        

        if (astarPath != null)
        {
            // Loop through all graphs and find the GridGraph
            foreach (NavGraph graph in astarPath.graphs)
            {
                if (graph is GridGraph)
                {
                    gridGraph = graph as GridGraph;

                    // Modify the rotation of the GridGraph
                    //gridGraph.rotation.x = ;
                    //print(gridGraph.rotation);
                    // Recalculate the grid to apply changes
                    //AstarPath.active.Scan();

                    Debug.Log("FindGridGraph");
                }
            }
        }
        else
        {
            Debug.LogError("AstarPath component not found in the scene.");
        }
    }

    void Update()
    {
        
        //DrawRotationField (gridGraph);
        gridGraph.rotation = RoundVector3(new Vector3(-90 + mapGen.transform.rotation.eulerAngles.z, 270, 90));
        //gridGraph.rotation.x = -90+mapGen.transform.rotation.z*180;
        //AstarPath.active.Scan();
        PartialUpdateGridGraph(gridGraph);
        //gridGraph.RelocateNodes(gridGraph.center,Quaternion.Euler(gridGraph.rotation),gridGraph.nodeSize,gridGraph.aspectRatio,gridGraph.isometricAngle);
        //print(gridGraph.rotation);
        print(mapGen.transform.rotation.eulerAngles.z);
    }

    private void UpdateGridGraphNodes(GridGraph gridGraph)
    {
        // Update the graph bounds and node positions
        gridGraph.UpdateTransform();

        // Initialize the nodes and grid
        gridGraph.GetNodes(node => 
        {
            GridNode gridNode = node as GridNode;

            // Recalculate node positions and connections based on the new rotation
            if (gridNode != null)
            {
                // Calculate the node's position
                gridNode.position = (Int3)gridGraph.GraphPointToWorld(node.position.x, node.position.z, 0);
                // Perform additional updates as needed
            }
        });

        // Optionally, update the graph's connections if needed
        gridGraph.GetNodes(node =>
        {
            GridNode gridNode = node as GridNode;
            if (gridNode != null)
            {
                gridGraph.CalculateConnectionsForCellAndNeighbours(gridNode.XCoordinateInGrid, gridNode.ZCoordinateInGrid);
            }
        });
    }

    void DrawRotationField (GridGraph graph) {
			
				var right = Quaternion.Euler(graph.rotation) * Vector3.right;
				var angle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;
                angle = mapGen.transform.rotation.z;
				if (angle < 0) angle += 360;
				if (Mathf.Abs(angle - Mathf.Round(angle)) < 0.001f) angle = Mathf.Round(angle);
				
				
				
				graph.rotation = RoundVector3(new Vector3(-90 + angle, 270, 90));
				
			
		}

    public static Vector3 RoundVector3 (Vector3 v) 
    {
			const int Multiplier = 2;

			if (Mathf.Abs(Multiplier*v.x - Mathf.Round(Multiplier*v.x)) < 0.001f) v.x = Mathf.Round(Multiplier*v.x)/Multiplier;
			if (Mathf.Abs(Multiplier*v.y - Mathf.Round(Multiplier*v.y)) < 0.001f) v.y = Mathf.Round(Multiplier*v.y)/Multiplier;
			if (Mathf.Abs(Multiplier*v.z - Mathf.Round(Multiplier*v.z)) < 0.001f) v.z = Mathf.Round(Multiplier*v.z)/Multiplier;
			return v;
		}

    private void PartialUpdateGridGraph(GridGraph gridGraph)
    {
        // Update the graph's transform
        gridGraph.UpdateTransform();

        // Determine the area to update
        // For this example, let's update the entire grid
        // You can change this to only update specific regions if needed
        for (int x = 0; x < gridGraph.width; x++)
        {
            for (int z = 0; z < gridGraph.depth; z++)
            {
                // Get the node at the current position
                GridNode node = gridGraph.GetNode(x, z) as GridNode;

                if (node != null)
                {
                    // Recalculate the node's position
                    node.position = (Int3)gridGraph.GraphPointToWorld(x, z, 0);

                    // Update connections for the node and its neighbors
                    gridGraph.CalculateConnectionsForCellAndNeighbours(x, z);
                }
            }
        }
    }
}