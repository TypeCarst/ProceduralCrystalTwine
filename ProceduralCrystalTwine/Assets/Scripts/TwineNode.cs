using UnityEngine;

public class TwineNode : MonoBehaviour
{
    [SerializeField] private float baseRadius;

    public float BaseRadius => baseRadius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}