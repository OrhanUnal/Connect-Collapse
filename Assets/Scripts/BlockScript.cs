using System.Collections;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField] BlockType type;
    [SerializeField] Sprite[] sprites;
    public int xIndex;
    public int yIndex;
    private Vector2 _targetPos;
    private Vector2 _currentPos;
    private SpriteRenderer _spriteRenderer;
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

    public enum SpriteConditions
    {
        Default,
        ConditionA,
        ConditionB,
        ConditionC,
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetIndicies(int postionX, int postionY)
    {
        xIndex = postionX;
        yIndex = postionY;
    }

    public IEnumerator MoveTarget(Vector2 positionOfDestination)
    {
        float duration = 0.4f;
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(startPosition, positionOfDestination, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = positionOfDestination;
    }

    public BlockType GetBlockType()
    {
        return type;
    }

    public void ChangeSprites(SpriteConditions conditionThatMatched)
    {
        switch (conditionThatMatched)
        {
            case SpriteConditions.Default:
                _spriteRenderer.sprite = sprites[0];
                break;
            case SpriteConditions.ConditionA:
                _spriteRenderer.sprite = sprites[1];
                break;
            case SpriteConditions.ConditionB:
                _spriteRenderer.sprite= sprites[2];
                break;
            case SpriteConditions.ConditionC:
                _spriteRenderer.sprite = sprites[3];
                break;
            default:
                _spriteRenderer.sprite = sprites[0];
                break;
        }
    }
}
