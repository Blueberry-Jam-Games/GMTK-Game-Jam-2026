using UnityEngine;

public class BasicGizmo : MonoBehaviour
{
    [SerializeField]
    private GameObject other;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color (0.8f, 0.4f, 0.0f, 0.75f);
        Gizmos.DrawSphere(transform.position, 0.05f);

        if (other != null)
        {
            Gizmos.DrawLine(transform.position, other.transform.position);
        }
    }
#endif
}
