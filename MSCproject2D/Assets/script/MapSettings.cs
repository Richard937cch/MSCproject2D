using UnityEngine;

[CreateAssetMenu(fileName = "MapSettings", menuName = "ScriptableObjects/MapSettings", order = 2)]
public class MapSettings : ScriptableObject
{
    public int width;
    public int height;
    public int seed;
    public int scoreTokenAmount;
    public int hidenScoreTokenAmount;
    public int perkTokenAmount;
    public int hidenPerkTokenAmount;
    public MapType mapType;
    public BlockType blockType;
    public BackType backType;
    public bool lavaMode;
    public bool enemyEnable;
    public int rotationSpeed;
    public int jump;
    public int life;
    public int hp;

    public float w = 0.5114591f;
    public float h = 0.5042072f;
    public string s = "1234";
    public float sc = 0.09167421f;
    public float hisc = 0.09167421f;
    public float p = 0.04583697f;
    public float hip = 0.04583697f;
    public int map = 0;
    public int block = 0;
    public int back = 0;
    public bool lava = false;
    public bool enemy = true;
    public float r = 0.09167393f;
    public float j = 0.1260518f;
    public float l = 0.02291848f;
    public float hpi = 0.09167393f;

    
    /*public int width = 50;
    public int height = 50;
    public int seed = 1234;
    public int scoreTokenAmount = 10;
    public int perkTokenAmount = 5;
    public MapType mapType = MapType.RandomNCA;
    public BlockType blockType = BlockType.None;
    public BackType backType = BackType.None;
    public int rotationSpeed = 10;
    public int jump = 5;
    public int life = 3;
    public int hp = 100;

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
    public float hpi;*/

}


