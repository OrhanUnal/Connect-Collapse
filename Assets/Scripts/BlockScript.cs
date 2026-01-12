using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField] BlockType type;
    [SerializeField] int xIndex;
    [SerializeField] int yIndex;
    private Vector2 _targetPos;
    private Vector2 _currentPos;
    public enum BlockType
    {
        Red,
        Blue, 
        Green,
        Yellow,
        Pink,
        Purple
    }

    public BlockScript(int postionX, int postionY)
    {
        xIndex = postionX;
        yIndex = postionY;
    }

    public void SetPosition(int postionX, int postionY)
    {
        xIndex = postionX;
        yIndex = postionY;
    }
}
