using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    [SerializeField] float blocksDistance;
    [SerializeField] GameObject[] blockPrefabs;

    public GameObject BlockParent;
    public static BoardScript instance;

    private HashSet<int> columnsNeedingGravity = new HashSet<int>();
    private int width;
    private int height;
    private int maxColors;
    private float middleOfTheBoardX;
    private float middleOfTheBoardY;
    private NodeScript[,] nodesArray;

    private void Start()
    {
        instance = this;
        width = GameManager.instance.GetWidthOfTheBoard();
        height = GameManager.instance.GetHeightOfTheBoard();
        maxColors = GameManager.instance.GetMaxColors();
        if (maxColors > 6) maxColors = 6;
        InitBoard();
    }

    private void OnEnable()
    {
        GameManager.handleInput += HandleInput;
        NodeScript.OnColumnNeedsGravity += HandleColumnGravity;
    }

    private void OnDisable()
    {
        NodeScript.OnColumnNeedsGravity -= HandleColumnGravity;
        GameManager.handleInput -= HandleInput;
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
                block.transform.SetParent(BlockParent.transform);
                block.GetComponent<BlockScript>().SetIndicies(x, y);
                nodesArray[x, y] = new NodeScript(block);
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
                nodesArray[x, y].visited = false;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!nodesArray[x, y].visited && nodesArray[x, y].block != null)
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

        if (nodeToScan.currentBlockType != searchingType ||
            nodeToScan.visited ||
            nodeToScan.block == null ||
            searchingType == BlockScript.BlockType.None)
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

    private void HandleInput(int xIndex, int yIndex)
    {
        int idToDestroyObjects = nodesArray[xIndex, yIndex].GetMatchedBlocksId();
        GameManager.destroyBlocks?.Invoke(idToDestroyObjects);
    }

    private void HandleColumnGravity(int x)
    {
        if (!columnsNeedingGravity.Contains(x))
        {
            columnsNeedingGravity.Add(x);
            StartCoroutine(ApplyGravityOnColumn(x));
        }
    }

    private IEnumerator ApplyGravityOnColumn(int x)
    {
        yield return null;
        bool moved;
        do
        {
            moved = false;
            for (int y = 0; y < height - 1; y++)
            {
                if (nodesArray[x, y].block == null && nodesArray[x, y + 1].block != null)
                {
                    nodesArray[x, y].ResetNode();
                    GameObject blockAbove = nodesArray[x, y + 1].block;
                    if (blockAbove == null) continue;
                    BlockScript blockScript = blockAbove.GetComponent<BlockScript>();
                    if (blockScript == null) continue;
                    nodesArray[x, y].block = blockAbove;
                    blockScript.SetIndicies(x, y);
                    nodesArray[x, y].currentBlockType = blockScript.GetBlockType();
                    nodesArray[x, y + 1].ResetNode();

                    Vector2 targetPos = new Vector2(x - middleOfTheBoardX, y - middleOfTheBoardY) * blocksDistance;
                    StartCoroutine(blockScript.MoveTarget(targetPos));

                    moved = true;
                }
            }

            yield return new WaitForSeconds(0.1f);
        } while (moved);

        SpawnNewBlocksInColumn(x);

        columnsNeedingGravity.Remove(x);

        yield return new WaitForSeconds(0.3f);
        ScanBoard();
    }
    private void SpawnNewBlocksInColumn(int x)
    {
        for (int y = height - 1; y >= 0; y--)
        {
            if (nodesArray[x, y].block == null)
            {
                Vector2 spawnPos = new Vector2(x - middleOfTheBoardX, (height - 1 - middleOfTheBoardY) + 2);
                spawnPos *= blocksDistance;

                int randomIndex = Random.Range(0, maxColors);
                GameObject newBlock = Instantiate(blockPrefabs[randomIndex], spawnPos, Quaternion.identity);
                newBlock.transform.SetParent(BlockParent.transform);

                BlockScript blockScript = newBlock.GetComponent<BlockScript>();
                blockScript.SetIndicies(x, y);

                nodesArray[x, y].block = newBlock;
                nodesArray[x, y].currentBlockType = blockScript.GetBlockType();

                Vector2 targetPos = new Vector2(x - middleOfTheBoardX, y - middleOfTheBoardY) * blocksDistance;
                StartCoroutine(blockScript.MoveTarget(targetPos));
            }
        }
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