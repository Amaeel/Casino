using UnityEngine;
using UnityEngine.EventSystems;

public class Input_Manager_Movil : MonoBehaviour
{
    [Header("Valores de Salida")]
    public Vector2 v2_movimiento_joystick = Vector2.zero;
    public Vector2 v2_rotacion_tactil = Vector2.zero;

    [HideInInspector] public bool boton_saltar_pulsado = false;
    [HideInInspector] public bool boton_interactuar_pulsado = false;

    private int id_rotacion_dedo = -1;
    private Vector2 posicion_dedo_anterior;

    private void Update()
    {
        // Reiniciar valores de rotación cada frame
        v2_rotacion_tactil = Vector2.zero;

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                // SOLUCIÓN AL ERROR: Comprobar si el toque es sobre la UI (botones/joystick)
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    continue;
                }

                // Rotación de la cámara (mitad derecha de la pantalla)
                if (touch.position.x > Screen.width / 2)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        id_rotacion_dedo = touch.fingerId;
                        posicion_dedo_anterior = touch.position;
                    }
                    else if (touch.fingerId == id_rotacion_dedo && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary))
                    {
                        Vector2 delta = touch.position - posicion_dedo_anterior;
                        v2_rotacion_tactil = delta * 0.1f; // Ajusta la sensibilidad aquí
                        posicion_dedo_anterior = touch.position;
                    }
                }

                // Resetear el ID cuando el dedo se levanta
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (touch.fingerId == id_rotacion_dedo)
                    {
                        id_rotacion_dedo = -1;
                    }
                }
            }
        }
    }

    public void SetSaltarPulsado(bool estado) => boton_saltar_pulsado = estado;
    public void SetInteractuarPulsado(bool estado) => boton_interactuar_pulsado = estado;
}