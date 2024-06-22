// GlobalEnum.cs

    public enum TileType //what kind of tile (For BlockReaction& Gridgen initiation)
    {
        BackGrounds, Blocks
    }

    public enum MapType //Map generating method (Gridgen setup)
    {
        RandomNCA, Perlin, WFC
    };

    public enum BlockType //Blocks reaction type (Gridgen setup and initiation)
    {
        None, Hit, Touch
    };

    public enum BackType //BackGround tile restore note (Gridgen setup & Tile Manager)
    {
        None, Restore
    };

    public enum BlockReactionType //Block Reaction note (For BlockReaction)
    {
        None, Hit, Touch, Restore
    };

    public enum ChangeType //Tile changing method (for TileManager)
    {
        None, Block2Back, Back2Block
    };

    public enum MapRotation //Map rotation method (for scrollControl)
    {
        None, ScrollTransform, ScrollForce, Gravity
    };



