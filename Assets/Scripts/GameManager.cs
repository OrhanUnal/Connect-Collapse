using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] int conditionA;
    [SerializeField] int conditionB;
    [SerializeField] int conditionC;
    [SerializeField] int widthOfTheBoard;
    [SerializeField] int heightOfTheBoard;
    [SerializeField] int maxColors;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
    #endregion
}
