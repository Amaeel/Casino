using UnityEngine;

public class Moviment_Superficie : MonoBehaviour, IVelocitat,IInfo
{
    Vector3 v3_vel_superficie = Vector3.zero;

    RaycastHit raycastHit;

    public void Info_Impacte_Superficie(RaycastHit hitInfo)
    {
        raycastHit = hitInfo;
    }

    public void Reinicia()
    {
        v3_vel_superficie = Vector3.zero;
    }

    public Vector3 V3_g_velocitat()
    {        
        if( raycastHit.collider != null)
        {
            IVelocitat velocitat = raycastHit.collider.gameObject.GetComponent<IVelocitat>();
            if (velocitat != null)
            {
                v3_vel_superficie = velocitat.V3_g_velocitat();
            }
            else
            {
                v3_vel_superficie = Vector3.zero;
            }
        }

        return v3_vel_superficie;
    }

}
