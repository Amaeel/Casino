using UnityEngine;
using UnityEngine.InputSystem;

public class Moviment_vertical : MonoBehaviour, IVelocitat, IInfo
{
    CharacterController cc_goku;
    Animator animator;

    [Header("Configuración del Salto")]
    [SerializeField] float f_impuls_bot = 12f; // He subido este valor para que salte más
    public float F_impuls_bot { get { return f_impuls_bot; } set { f_impuls_bot = Mathf.Max(value, 0f); } }

    [SerializeField] float f_gravedad_personalizada = -30f; // Gravedad más fuerte para un control más preciso

    Vector3 v3_l_velocitat_total = Vector3.zero;
    RaycastHit raycastHit;

    void Start()
    {
        cc_goku = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Suscripción al evento de salto móvil
        Gestos_Pantalla gestos_Pantalla = FindAnyObjectByType<Gestos_Pantalla>();
        if (gestos_Pantalla != null)
            gestos_Pantalla.A_Botar += Bota;
    }

    private void OnDestroy()
    {
        Gestos_Pantalla gestos_Pantalla = FindAnyObjectByType<Gestos_Pantalla>();
        if (gestos_Pantalla != null)
            gestos_Pantalla.A_Botar -= Bota;
    }

    public Vector3 V3_g_velocitat()
    {
        // 1. DETECTAR SUELO
        // Usamos el CharacterController y tu RaycastHit para estar seguros
        bool estaEnSuelo = cc_goku.isGrounded || raycastHit.collider != null;

        if (estaEnSuelo)
        {
            if (animator != null) animator.SetBool("Bot", false);

            // Si estamos en el suelo y cayendo, reseteamos la velocidad vertical
            if (v3_l_velocitat_total.y < 0)
            {
                v3_l_velocitat_total.y = -2f;
            }

            // 2. SALTO (TECLADO)
            // Cambiado 'isPressed' por 'wasPressedThisFrame' para evitar el vuelo infinito
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                RealizarSalto();
            }
        }
        else
        {
            // Aplicar gravedad solo si estamos en el aire
            v3_l_velocitat_total.y += f_gravedad_personalizada * Time.deltaTime;
        }

        // Si chocamos con el techo, empezamos a caer inmediatamente
        if ((cc_goku.collisionFlags & CollisionFlags.Above) != 0 && v3_l_velocitat_total.y > 0)
        {
            v3_l_velocitat_total.y = 0;
        }

        // Devolvemos la velocidad en coordenadas globales
        return v3_l_velocitat_total;
    }

    void Bota()
    {
        // Salto para móvil
        if (cc_goku.isGrounded || raycastHit.collider != null)
        {
            RealizarSalto();
            Debug.Log("¡Salto ejecutado en móvil!");
        }
    }

    private void RealizarSalto()
    {
        v3_l_velocitat_total.y = f_impuls_bot;
        if (animator != null) animator.SetBool("Bot", true);
    }

    public void Info_Impacte_Superficie(RaycastHit hitInfo)
    {
        raycastHit = hitInfo;
    }

    public void Reinicia()
    {
        v3_l_velocitat_total = Vector3.zero;
    }
}