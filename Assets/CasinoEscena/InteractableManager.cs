using UnityEngine;
using UnityEngine.InputSystem;

public class InteractableManager : MonoBehaviour
{

    [Header("Configuración")]
    public float distanciaInteraccion = 3f;


    void IntentarInteractuar() {
        // Crear rayo desde el centro de la cámara
        Ray rayo = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, distanciaInteraccion))
        {
            // Verificar si el objeto que se toca tiene el Tag correcto
            if (hit.collider.CompareTag("Interactable"))
            {
                // Buscar si tiene el componente IInteractable
                IInteractable interactuable = hit.collider.GetComponent<IInteractable>();
                if (interactuable != null)
                {
                    // Ejecutar la accion
                    interactuable.Interact();
                }

            }
        }
    }

    private void Update()
    {
        // Detectar tecla 'E'
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarInteractuar();
        }
    }
}
