using System.Collections.Generic;
using UnityEngine;

public class NodeScript
{

    private int matchedBlocksId;
    private int blastableNeighbourCount;

    public GameObject block;
    public int xIndex;
    public int yIndex;
    public bool visited;
    public BlockScript.BlockType currentBlockType;

    public NodeScript(GameObject block)
    {
        this.block = block;
        blastableNeighbourCount = 0;
        currentBlockType = block.GetComponent<BlockScript>().GetBlockType();
        xIndex = block.GetComponent<BlockScript>().xIndex;
        yIndex = block.GetComponent<BlockScript>().yIndex;
        visited = false;
    }

    public void AssignNewMatchedBlocks(int count, int id)
    {
        blastableNeighbourCount = count;
        matchedBlocksId = id;
        switch (blastableNeighbourCount)
        {
            case 3:
                Debug.Log("3CONNECT");
                break;
            case 4:
                Debug.Log("4CONNECT");
                break;
            case 5:
                Debug.Log("OHABEABI");
                break;
        }
                //switch case that checks a b c from gamemanagaer for sprite
    }

        public void DeleteBlock(NodeScript neighbour)
        {
            //deletion and cascading alghorithm
            return;
        }
    
}
