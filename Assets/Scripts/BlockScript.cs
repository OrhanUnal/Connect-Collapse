using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField] BlockType type;
    public int xIndex;
    public int yIndex;
    private Vector2 _targetPos;
    private Vector2 _currentPos;
    public enum BlockType
    {
        Red,
        Blue, 
        Green,
        Yellow,
        Pink,
        Purple,
        None
    }

    public void SetPosition(int postionX, int postionY)
    {
        xIndex = postionX;
        yIndex = postionY;
    }

    public BlockType GetBlockType()
    {
        return type;
    }
}
