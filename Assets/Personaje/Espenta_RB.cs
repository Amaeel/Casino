using UnityEngine;

public class Espenta_RB : MonoBehaviour
{
    public float f_força = 5f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = (hit.transform.position - transform.position).normalized * f_força;
        }
    }
}
