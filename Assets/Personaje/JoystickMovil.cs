using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickMovil : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public RectTransform fondoJoystick; // Arrastra aquí el cuadrado rojo
    public float radio = 100f; // El límite de movimiento

    private Input_Manager_Movil inputManager;
    private Vector2 posicionInicialLocal;

    void Start()
    {
        posicionInicialLocal = transform.localPosition;
        inputManager = FindFirstObjectByType<Input_Manager_Movil>();

        if (inputManager == null)
            Debug.LogError("¡No se encontró el script Input_Manager_Movil en el personaje!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("¡Joystick tocado!");
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 posicionLocal;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(fondoJoystick, eventData.position, eventData.pressEventCamera, out posicionLocal))
        {
            // Limitar el movimiento del cuadrado blanco dentro del radio del rojo
            Vector2 posicionLimitada = Vector2.ClampMagnitude(posicionLocal, radio);
            transform.localPosition = posicionLimitada;

            // Enviar datos al personaje (valores entre -1 y 1)
            if (inputManager != null)
            {
                inputManager.v2_movimiento_joystick = posicionLimitada / radio;
                Debug.Log("Enviando movimiento: " + inputManager.v2_movimiento_joystick);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Volver al centro al soltar
        transform.localPosition = posicionInicialLocal;
        if (inputManager != null)
        {
            inputManager.v2_movimiento_joystick = Vector2.zero;
            Debug.Log("Joystick soltado.");
        }
    }
}