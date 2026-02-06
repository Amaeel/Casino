using UnityEngine;
using UnityEngine.InputSystem;

public class Gestos_Pantalla : MonoBehaviour
{
    public delegate void Botar();
    public event Botar A_Botar;

    Vector2 v2_pos_ini_touch = Vector2.zero;

    private void Update()
    {
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.press.wasPressedThisFrame)
            {
                Debug.Log("Tocat " + Touchscreen.current.primaryTouch.position.ReadValue().ToString());
                v2_pos_ini_touch = Touchscreen.current.primaryTouch.position.ReadValue();
            }

            if (Touchscreen.current.press.wasReleasedThisFrame)
            {
                Debug.Log("Soltar " + Touchscreen.current.primaryTouch.ReadValue().ToString());
                Vector2 v2_pos_final_touch = Touchscreen.current.primaryTouch.position.ReadValue();

                if (v2_pos_ini_touch.y < v2_pos_final_touch.y)
                {
                    A_Botar?.Invoke();
                    Debug.Log("detectat Bot");
                }
            }
        }
    }

}
