using UnityEngine;

public class BoardScript : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int maxColors;
    [SerializeField] float blocksDistance; 
    [SerializeField] GameObject[] blockPrefabs;
    
    private float middleOfTheBoardX;
    private float middleOfTheBoardY;
    private NodeScript[,] nodesArray;

    private void Start()
    {
        InitBoard();
    }

    private void InitBoard()
    {
        nodesArray = new NodeScript[width, height];
        middleOfTheBoardX = (float)(width - 1) / 2;
        middleOfTheBoardY = (float)(height - 1) / 2;
        if (maxColors > 6)
            maxColors = 6;
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
                Debug.Log(positionOfNode);
            }
        }
    }
}
