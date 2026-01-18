using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int activeAnimation;
    public static event Action<int, int> handleInput;
    public delegate void DestroyBlocks(int id);
    public static GameManager instance;
    public static DestroyBlocks destroyBlocks;
    public stateOfBoard currentState;
    public enum stateOfBoard
    {
        Idle,
        Falling,
        Deadlock
    }

    [SerializeField] int conditionA;
    [SerializeField] int conditionB;
    [SerializeField] int conditionC;
    [SerializeField] int widthOfTheBoard;
    [SerializeField] int heightOfTheBoard;
    [SerializeField] float paddingToYAxis;
    [SerializeField] float paddingToXAxis;
    [SerializeField] int maxColors;

    private void Awake()
    {
        activeAnimation = 0;
        instance = this;
        currentState = stateOfBoard.Idle;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentState == stateOfBoard.Idle)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider == null) return;
            BlockScript hitBlock = hit.collider.GetComponent<BlockScript>();
            if (hitBlock)
            {
                handleInput?.Invoke(hitBlock.xIndex, hitBlock.yIndex);
            }
        }
    }

    #region Getters
    public int GetConditionA()
    {
        return conditionA;
    }
    public int GetConditionB()
    {
        return conditionB;
    }
    public int GetConditionC()
    {
        return conditionC;
    }
    public int GetWidthOfTheBoard()
    {
        return widthOfTheBoard;
    }
    public int GetHeightOfTheBoard()
    {
        return heightOfTheBoard;
    }
    public int GetMaxColors()
    {
        return maxColors;
    }

    public float GetPaddingX()
    {
        return paddingToYAxis;
    }
    public float GetPaddingY()
    {
        return paddingToXAxis;
    }
    #endregion
}
