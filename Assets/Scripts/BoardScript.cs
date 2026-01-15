using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    [SerializeField] float blocksDistance; 
    [SerializeField] GameObject[] blockPrefabs;

    private int width;
    private int height;
    private int maxColors;
    private float middleOfTheBoardX;
    private float middleOfTheBoardY;
    private NodeScript[,] nodesArray;

    private void Start()
    {
        width = GameManager.instance.GetWidthOfTheBoard();
        height = GameManager.instance.GetHeightOfTheBoard();
        maxColors = GameManager.instance.GetMaxColors();
        if (maxColors > 6) maxColors = 6;
        InitBoard();
    }

    private void InitBoard()
    {
        nodesArray = new NodeScript[width, height];
        middleOfTheBoardX = (float)(width - 1) / 2;
        middleOfTheBoardY = (float)(height - 1) / 2;
        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                Vector2 positionOfNode = new Vector2(x - middleOfTheBoardX, y - middleOfTheBoardY);
                positionOfNode *= blocksDistance;
                int randomIndex = Random.Range(0, maxColors);
                GameObject block = Instantiate(blockPrefabs[randomIndex], positionOfNode, Quaternion.identity);
                block.GetComponent<BlockScript>().SetPosition(x, y);
                nodesArray[x,y] = new NodeScript(block);
            }
        }
        ScanBoard();
    }

    private void ScanBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!nodesArray[x, y].visited)
                {
                    MatchedBlocks matched = new MatchedBlocks();
                    CheckNeighbour(nodesArray[x, y], nodesArray[x, y].currentBlockType, matched);
                    foreach (var node in matched.nodes)
                    {
                        node.AssignNewMatchedBlocks(matched.countOfBlocks, matched.idOfList);
                    }
                }
            }

        }
    }

    private void CheckNeighbour(NodeScript nodeToScan, BlockScript.BlockType searchingType, MatchedBlocks list)
    {
        int xIndex = nodeToScan.xIndex;
        int yIndex = nodeToScan.yIndex;
        if (nodeToScan.currentBlockType != searchingType || nodeToScan.visited)
            return;

        nodeToScan.visited = true;
        list.nodes.Add(nodeToScan);
        list.countOfBlocks++;
        if (xIndex > 0)
            CheckNeighbour(nodesArray[xIndex - 1, yIndex], searchingType, list);
        if (yIndex > 0)
            CheckNeighbour(nodesArray[xIndex, yIndex - 1], searchingType, list);
        if (xIndex < width - 1)
            CheckNeighbour(nodesArray[xIndex + 1, yIndex], searchingType, list);
        if (yIndex < height - 1)
            CheckNeighbour(nodesArray[xIndex, yIndex + 1], searchingType, list);
        return;
    }
}

public class MatchedBlocks
{
    public int countOfBlocks;
    public int idOfList;

    public List<NodeScript> nodes = new List<NodeScript>();

    static int nextIdOfList = 0;
    public MatchedBlocks()
    {
        idOfList = nextIdOfList;
        nextIdOfList++;
        countOfBlocks = 0;
    }
}