using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnumManager : MonoBehaviour
{
    
    private Dictionary<BlockType, BlockReactionType> blockTypeToReactionType = new Dictionary<BlockType, BlockReactionType>
    {
        { BlockType.None, BlockReactionType.None },
        { BlockType.Hit, BlockReactionType.Hit },
        { BlockType.Touch, BlockReactionType.Touch }
    };

    /*private Dictionary<BackType, BlockReactionType> backTypeToReactionType = new Dictionary<BackType, BlockReactionType>
    {
        { BackType.None, BlockReactionType.None},
        { BackType.Restore, BlockReactionType.Restore}
    };*/

    // Method to set the block reaction type
    public void SetBlockType(GameObject newBlock, BlockType blockType)
    {
        BlockReaction blockReaction = newBlock.GetComponent<BlockReaction>();
        if (blockReaction != null && blockTypeToReactionType.TryGetValue(blockType, out BlockReactionType reactionType))
        {
            blockReaction.type = reactionType;
        }
        else
        {
            Debug.LogWarning("BlockReaction component not found or BlockType not mapped.");
        }
    }

    
}
