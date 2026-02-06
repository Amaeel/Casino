using UnityEngine;
using UnityEngine.InputSystem;

public class Rotacio_Cub : MonoBehaviour
{
 
    private Moviment_Cub scriptPare;
    private float rotacionVertical = 0f;

    void Start()
    {
        scriptPare = GetComponentInParent<Moviment_Cub>();

        // Si la referencia es nula, significa que la cámara no es hija del jugador
        if (scriptPare == null)
        {
            Debug.LogError("Error en Rotacio_Cub: El script Moviment_Cub no fue encontrado en el objeto padre.");
        }
    }

    void Update()
    {
        float sensibilidad = scriptPare != null ? scriptPare.f_sensibilitat_mouse : 1f;

        // ROTACIÓN VERTICAL (Mouse Y)
        float mouseY = 0f;
        if (Mouse.current != null)
        {
            mouseY = Mouse.current.delta.y.ReadValue() * sensibilidad * Time.deltaTime;
        }

        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -90f, 90f);

        // rotación LOCAL.
        transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
    }
}