using UnityEngine;

public class TwineNode : MonoBehaviour
{
    public float BaseRadius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position, 0.05f);
    }
}