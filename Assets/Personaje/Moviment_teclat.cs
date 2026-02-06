using UnityEngine;
using UnityEngine.InputSystem;

public class Moviment_teclat : MonoBehaviour, IVelocitat, IInfo
{
    CharacterController cc_goku;
    [SerializeField] float f_velocitat = 5f;
    Animator animator;
    RaycastHit raycastHit;
    Vector3 v3_g_velocitat_total = Vector3.zero;

    public float F_velocitat
    {
        get { return f_velocitat; }
        set { f_velocitat = Mathf.Max(value, 0f); }
    }

    void Start()
    {
        cc_goku = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    public Vector3 V3_g_velocitat()
    {
        Vector3 v3_l_velocitat_total = Vector3.zero;

        // 1. INPUT TECLADO
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v3_l_velocitat_total += Vector3.forward;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v3_l_velocitat_total += Vector3.back;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) v3_l_velocitat_total += Vector3.left;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) v3_l_velocitat_total += Vector3.right;
        }

        // 2. INPUT MÓVIL (Joystick)
        Input_Manager_Movil inputMovil = GetComponent<Input_Manager_Movil>();
        if (inputMovil != null && inputMovil.v2_movimiento_joystick != Vector2.zero)
        {
            v3_l_velocitat_total += new Vector3(inputMovil.v2_movimiento_joystick.x, 0, inputMovil.v2_movimiento_joystick.y);
        }

        if (animator != null)
            animator.SetFloat("Avant", v3_l_velocitat_total.magnitude);

        Vector3 v3_direccion_global = transform.TransformVector(v3_l_velocitat_total);

        // Ajuste de pendiente / suelo
        if (raycastHit.collider != null)
        {
            v3_g_velocitat_total = Vector3.ProjectOnPlane(v3_direccion_global.normalized, raycastHit.normal) * f_velocitat;
        }
        else
        {
            v3_g_velocitat_total = v3_direccion_global.normalized * f_velocitat;
        }

        return v3_g_velocitat_total;
    }

    public void Reinicia() => v3_g_velocitat_total = Vector3.zero;
    public void Info_Impacte_Superficie(RaycastHit hitInfo) => raycastHit = hitInfo;
}