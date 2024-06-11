using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileData : ScriptableObject
{
    public GameObject tilePrefab;
    public List<TileData> allowedNeighborsUp;
    public List<TileData> allowedNeighborsDown;
    public List<TileData> allowedNeighborsLeft;
    public List<TileData> allowedNeighborsRight;
}
