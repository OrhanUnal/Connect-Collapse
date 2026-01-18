using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static event Action<int, int> handleInput;
    public delegate void DestroyBlocks(int id);
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
        instance = this;
        currentState = stateOfBoard.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider && hit.collider.gameObject.GetComponent<BlockScript>() && currentState == stateOfBoard.Idle)
            {
                handleInput?.Invoke(hit.collider.gameObject.GetComponent<BlockScript>().xIndex, hit.collider.GetComponent<BlockScript>().yIndex);
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
