using UnityEngine;

public class NodeScript : MonoBehaviour
{
    [SerializeField] GameObject block;
    public NodeScript(GameObject block)
    {
        this.block = block;
    }
}
