using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    [SerializeField] GameObject[] blockPrefabs;

    public GameObject BlockParent;
    public static BoardScript instance;

    private const float CHAIN_EFFECT_WAIT_TIME = 0.1f;
    private const float WAIT_TIME_BEFORE_RESOLVING_DEADLOCK = 0.7f;
    private const float WAIT_BEFORE_SCAN = 0.2f;

    private float blocksDistance;
    private int width;
    private int height;
    private int maxColors;
    private float middleOfTheBoardX;
    private float middleOfTheBoardY;
    private bool deadLock;
    private bool solvingDeadlock = false;
    private Vector3 scaleOfBlocks;
    private Camera gameCamera;
    private NodeScript[,] nodesArray;
    private List<GameObject> allBlocksList = new List<GameObject>();
    private HashSet<int> columnsNeedingGravity = new HashSet<int>();

    private void Start()
    {
        instance = this;
        width = GameManager.instance.GetWidthOfTheBoard();
        height = GameManager.instance.GetHeightOfTheBoard();
        maxColors = GameManager.instance.GetMaxColors();
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
        if (maxColors > 6) maxColors = 6;
        if (maxColors < 0) maxColors = 0;
        if (width < 2) width = 2;
        if( width > 10) width = 10;
        if (height < 2) height = 2;
        if (height > 10) height = 10;

        nodesArray = new NodeScript[width, height];
        
        SmartFitCameraAndBlocks();
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
                block.transform.localScale = scaleOfBlocks;
                block.transform.SetParent(BlockParent.transform);
                block.GetComponent<BlockScript>().SetIndicies(x, y);
                nodesArray[x, y] = new NodeScript(block);
            }
        }
        ScanBoard();
    }
    private void SmartFitCameraAndBlocks()
    {
        gameCamera = Camera.main;
        float screenHeight = gameCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * gameCamera.aspect;

        float paddingX = GameManager.instance.GetPaddingX();
        float paddingY = GameManager.instance.GetPaddingY();

        float boardHeight = screenHeight - paddingX;
        float boardWidth = screenWidth - paddingY;

    blocksDistance = Mathf.Min(boardWidth / width, boardHeight / height);

        foreach (GameObject block in blockPrefabs)
        {
            float newScaleX = blocksDistance / 2;
            float newScaleY = blocksDistance / 2;
            float newScaleZ = 1;
            scaleOfBlocks = new Vector3(newScaleX, newScaleY, newScaleZ);
        }

        middleOfTheBoardX = (float)(width - 1) / 2;
        middleOfTheBoardY = (float)(height - 1) / 2;
    }

    private void ScanBoard()
    {
        deadLock = true;
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
        if (deadLock && !solvingDeadlock)
        {
            solvingDeadlock = true;
            GameManager.instance.currentState = GameManager.stateOfBoard.Deadlock;
            StartCoroutine(ResolveDeadLock());
        }
    }

    private void CheckNeighbour(NodeScript nodeToScan, BlockScript.BlockType searchingType, MatchedBlocks list)
    {
        int xIndex = nodeToScan.xIndex;
        int yIndex = nodeToScan.yIndex;

        if (nodeToScan.currentBlockType != searchingType || nodeToScan.visited || nodeToScan.block == null || searchingType == BlockScript.BlockType.None)
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
        if (list.countOfBlocks > 1)
            deadLock = false;
        return;
    }

    private void HandleInput(int xIndex, int yIndex)
    {
        int idToDestroyObjects = nodesArray[xIndex, yIndex].GetMatchedBlocksId();
        GameManager.destroyBlocks?.Invoke(idToDestroyObjects);
    }

    private void HandleColumnGravity(int xIndex)
    {

        if (!columnsNeedingGravity.Contains(xIndex))
        {
            columnsNeedingGravity.Add(xIndex);
            StartCoroutine(ApplyGravityOnColumn(xIndex));
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
                    BlockScript blockAboveScript = nodesArray[x, y + 1].block.GetComponent<BlockScript>();
                    if (blockAboveScript == null) continue;
                    nodesArray[x, y].block = blockAbove;
                    blockAboveScript.SetIndicies(x, y);
                    nodesArray[x, y].currentBlockType = blockAboveScript.GetBlockType();
                    nodesArray[x, y + 1].ResetNode();

                    Vector2 targetPos = new Vector2(x - middleOfTheBoardX, y - middleOfTheBoardY) * blocksDistance;
                    StartCoroutine(blockAboveScript.MoveTarget(targetPos));

                    moved = true;
                }
            }

            yield return new WaitForSeconds(CHAIN_EFFECT_WAIT_TIME);
        } while (moved);

        SpawnNewBlocksInColumn(x);

        columnsNeedingGravity.Remove(x);
        if (columnsNeedingGravity.Count == 0)
        {
            yield return null;
            ScanBoard();
        }
    }
    
    private void SpawnNewBlocksInColumn(int x)
    {
        for (int y = 0; y < height; y++)
        {
            if (nodesArray[x, y].block == null)
            {
                Vector2 spawnPos = new Vector2(x - middleOfTheBoardX, (height - 1 - middleOfTheBoardY) + 2);
                spawnPos *= blocksDistance;

                int randomIndex = Random.Range(0, maxColors);
                GameObject newBlock = Instantiate(blockPrefabs[randomIndex], spawnPos, Quaternion.identity);
                newBlock.transform.localScale = scaleOfBlocks;
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

    private IEnumerator ResolveDeadLock()
    {
        allBlocksList.Clear();
        yield return new WaitForSeconds(WAIT_TIME_BEFORE_RESOLVING_DEADLOCK);

        foreach (NodeScript node in nodesArray)
        {
            allBlocksList.Add(node.block);
            node.block = null;
        }

        //garanti yerlesecekleri sec
        
        GameObject secondGuarenteedBlock = null;
        GameObject firstGuarenteedBlock = null;
        BlockScript.BlockType firstBlockType = BlockScript.BlockType.None;
        int counter = 0;

        while (secondGuarenteedBlock == null && counter < allBlocksList.Count)
        {
            firstGuarenteedBlock = allBlocksList[counter];
            firstBlockType = firstGuarenteedBlock.GetComponent<BlockScript>().GetBlockType();
            foreach (GameObject block in allBlocksList)
            {
                if (block != null && block.GetComponent<BlockScript>().GetBlockType() == firstBlockType && block != firstGuarenteedBlock)
                {
                    secondGuarenteedBlock = block;
                    break;
                }
            }
            counter++;
        }
        if (secondGuarenteedBlock == null)
        {
            Debug.Log("IMPOSSIBLE TO SOLVE");
            yield break;
        }

        allBlocksList.Remove(firstGuarenteedBlock);
        allBlocksList.Remove(secondGuarenteedBlock);

        //garantileri yerlestir
        int guaranteedX = Random.Range(0, width - 1);
        int guaranteedY = Random.Range(0, height);

        nodesArray[guaranteedX, guaranteedY].block = firstGuarenteedBlock;
        nodesArray[guaranteedX, guaranteedY].currentBlockType = firstBlockType;

        nodesArray[guaranteedX + 1, guaranteedY].block = secondGuarenteedBlock;
        nodesArray[guaranteedX + 1, guaranteedY].currentBlockType = secondGuarenteedBlock.GetComponent<BlockScript>().GetBlockType();

        //geri kalani rastgele sirala
        ShuffleList(allBlocksList);
        
        //siraya gore yerlestir
        int blockIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!nodesArray[x,y].block && blockIndex < allBlocksList.Count)
                {   
                    GameObject block = allBlocksList[blockIndex];
                    nodesArray[x, y].block = block;
                    if (block != null)
                        nodesArray[x, y].currentBlockType = block.GetComponent<BlockScript>().GetBlockType();
                    blockIndex++;
                }
            }
        }

        //blocklari nodelara ilerlet
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (nodesArray[x, y].block != null)
                {
                    BlockScript blockScript = nodesArray[x, y].block.GetComponent<BlockScript>();
                    blockScript.SetIndicies(x, y);

                    Vector2 targetPos = new Vector2(x - middleOfTheBoardX, y - middleOfTheBoardY) * blocksDistance;
                    StartCoroutine(blockScript.MoveTarget(targetPos));
                }
            }
        }

        yield return new WaitForSeconds(WAIT_BEFORE_SCAN);
        ScanBoard();
        solvingDeadlock = false;
    }
    
    private void ShuffleList(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
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