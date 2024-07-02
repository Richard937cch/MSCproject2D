using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "ScriptableObjects/MapSettings", order = 2)]
public class MapSettings : ScriptableObject
{
    public int width;
    public int height;
    public int seed;
    public int scoreTokenAmount;
    public int perkTokenAmount;
    public MapType mapType;
    public BlockType blockType;
    public BackType backType;
    public int rotationSpeed;
    public int jump;
    public int life;
    public int hp;

    public float w;
    public float h;
    public string s;
    public float sc;
    public float p;
    public int map;
    public int block;
    public int back;
    public float r;
    public float j;
    public float l;
    public float hpi;

}


