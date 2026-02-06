using UnityEngine;
using UnityEngine.InputSystem;

public class Moviment_Cub : MonoBehaviour
{
    public float f_sensibilitat_mouse = 1;
    [SerializeField] float f_altura_extra = 0.1f;

    [Header("Ajustes de Velocidad")]
    public float multiplicadorEsprintar = 2.0f;

    CharacterController cc_goku;
    Vector3 v3_posicio_inicial = Vector3.zero;

    void Start()
    {
        cc_goku = GetComponent<CharacterController>();
        v3_posicio_inicial = transform.position;

#if UNITY_EDITOR || UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    void Update()
    {
        float mouseX = 0f;

        // 1. ROTACIÓN PC (Ratón)
        if (Mouse.current != null)
        {
            mouseX = Mouse.current.delta.x.ReadValue() * f_sensibilitat_mouse * 0.1f;
        }

        // 2. ROTACIÓN MÓVIL (Touch)
        Input_Manager_Movil inputMovil = GetComponent<Input_Manager_Movil>();
        if (inputMovil != null)
        {
            mouseX += inputMovil.v2_rotacion_tactil.x * f_sensibilitat_mouse * 10f;
        }

        transform.Rotate(Vector3.up, mouseX);

        // --- Lógica de Movimiento y Gravedad ---
        Vector3 v3_g_velocitat_total = Vector3.zero;
        RaycastHit rchit;
        Physics.Raycast(transform.position, Vector3.down, out rchit, cc_goku.height / 2 + f_altura_extra);

        foreach (IInfo info in GetComponents<IInfo>())
        {
            MonoBehaviour monob = info as MonoBehaviour;
            if (monob != null && monob.enabled)
                info.Info_Impacte_Superficie(rchit);
        }

        // --- DETECCIÓN DE ESPRINTAR ---
        // Si presionamos Shift, multiplicamos la velocidad
        float multiActual = 1.0f;
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
        {
            multiActual = multiplicadorEsprintar;
        }

        foreach (IVelocitat ivelocitat in GetComponents<IVelocitat>())
        {
            MonoBehaviour monob = ivelocitat as MonoBehaviour;
            if (monob != null && monob.enabled)
            {
                // Aplicamos el multiplicador a la velocidad obtenida de los scripts
                v3_g_velocitat_total += ivelocitat.V3_g_velocitat() * multiActual;
            }
        }

        cc_goku.Move(v3_g_velocitat_total * Time.deltaTime);
    }
}