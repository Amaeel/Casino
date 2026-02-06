using UnityEngine;
using UnityEngine.InputSystem;

public class Habilidad_Dash : MonoBehaviour
{
    // Variables ajustables en el Inspector
    [Header("Configuración Dash")]
    [SerializeField] float f_velocidad_dash = 20f;
    [SerializeField] float f_duracion_dash = 0.2f;
    [SerializeField] float f_cooldown_dash = 1f;

    float f_tiempo_dash;
    float f_tiempo_cooldown;

    CharacterController cc_goku;
    Moviment_teclat moviment_teclat; // obtener la direccion
    Vector3 v3_l_direccion_dash;

    void Start()
    {
        cc_goku = GetComponent<CharacterController>();
        moviment_teclat = GetComponent<Moviment_teclat>();
        f_tiempo_cooldown = 0f;
    }

    void Update()
    {
        // Cooldown
        if (f_tiempo_cooldown > 0)
        {
            f_tiempo_cooldown -= Time.deltaTime;
        }

        // Ejecutar Dash
        if (Keyboard.current.shiftKey.isPressed && f_tiempo_cooldown <= 0)
        {
            EjecutarDash();
        }

        // Aplicar movimiento del Dash
        if (f_tiempo_dash > 0)
        {
            cc_goku.Move(v3_l_direccion_dash * f_velocidad_dash * Time.deltaTime);
            f_tiempo_dash -= Time.deltaTime;

            // Bloquear otros movimientos mientras se hace el dash
            moviment_teclat.enabled = false;
        }
        else if (moviment_teclat.enabled == false)
        {
            // Re-habilitar movimiento cuando el dash termina
            moviment_teclat.enabled = true;
        }
    }

    void EjecutarDash()
    {
        // Obtener la dirección actual del movimiento
        v3_l_direccion_dash = moviment_teclat.transform.forward;

        // Si no se pulsa ninguna tecla, usar la dirección hacia adelante
        if (moviment_teclat.V3_g_velocitat().magnitude < 0.1f)
        {
            v3_l_direccion_dash = transform.forward;
        }
        else
        {
            // Usa la dirección global de la velocidad para que el dash siga el movimiento actual.
            v3_l_direccion_dash = moviment_teclat.V3_g_velocitat().normalized;
        }

        f_tiempo_dash = f_duracion_dash;
        f_tiempo_cooldown = f_cooldown_dash;
    }
}