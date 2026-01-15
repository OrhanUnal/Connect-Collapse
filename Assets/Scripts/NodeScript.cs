using System.Collections.Generic;
using UnityEngine;

public class NodeScript
{

    private int matchedBlocksId;
    private int blastableNeighbourCount;
    private int conditionA;
    private int conditionB;
    private int conditionC;

    public GameObject block;
    public int xIndex;
    public int yIndex;
    public bool visited;
    public BlockScript.BlockType currentBlockType;

    public NodeScript(GameObject block)
    {
        GameManager.destroyBlocks += DeleteBlock;
        this.block = block;
        blastableNeighbourCount = 0;
        currentBlockType = block.GetComponent<BlockScript>().GetBlockType();
        xIndex = block.GetComponent<BlockScript>().xIndex;
        yIndex = block.GetComponent<BlockScript>().yIndex;
        visited = false;
        conditionA = GameManager.instance.GetConditionA();
        conditionB = GameManager.instance.GetConditionB();
        conditionC = GameManager.instance.GetConditionC();
    }

    public void AssignNewMatchedBlocks(int count, int id)
    {
        BlockScript blockScript = block.GetComponent<BlockScript>();
        blastableNeighbourCount = count;
        matchedBlocksId = id;
        if (blastableNeighbourCount < conditionA)
        {
            blockScript.ChangeSprites(BlockScript.SpriteConditions.Default);
        }
        else if (blastableNeighbourCount < conditionB)
        {
            blockScript.ChangeSprites(BlockScript.SpriteConditions.ConditionA);
        }
        else if (blastableNeighbourCount < conditionC)
        {
            blockScript.ChangeSprites(BlockScript.SpriteConditions.ConditionB);
        }
        else
        {
            blockScript.ChangeSprites(BlockScript.SpriteConditions.ConditionC);
        }
    }

    public void DeleteBlock(int idToDestroy)
    {
        if (idToDestroy == matchedBlocksId && blastableNeighbourCount > 2)
        {
            UnityEngine.Object.Destroy(block);
            //destroy and cascading mechanism
        }
        return;
    }

    #region Getters
    public int GetMatchedBlocksId()
    { 
        return matchedBlocksId; 
    }
    #endregion

}
