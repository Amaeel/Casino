using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public bool buscarCamaraAutomaticamente = true;
    public Camera camaraPrincipal;

    [Header("Configuración de Seguimiento")]
    public Transform objetivoASeguir; // El jugador
    public Vector3 offsetPosicion = new Vector3(0f, 5f, -10f);
    public float suavidad = 0.125f;

    void Start()
    {
        // Intentar encontrar la cámara principal si no está asignada
        if (camaraPrincipal == null && buscarCamaraAutomaticamente)
        {
            camaraPrincipal = Camera.main;

            // Si aún no hay cámara, buscar cualquier cámara en la escena
            if (camaraPrincipal == null)
            {
                camaraPrincipal = FindObjectOfType<Camera>();
            }

            // Si todavía no hay cámara, crear una nueva
            if (camaraPrincipal == null)
            {
                Debug.LogWarning("No se encontró cámara. Creando una nueva...");
                CrearNuevaCamara();
            }
        }

        // Buscar el jugador si no está asignado
        if (objetivoASeguir == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");
            if (jugador != null)
            {
                objetivoASeguir = jugador.transform;
            }
        }
    }

    void LateUpdate()
    {
        // Solo seguir si tenemos un objetivo
        if (objetivoASeguir != null && camaraPrincipal != null)
        {
            Vector3 posicionDeseada = objetivoASeguir.position + offsetPosicion;
            Vector3 posicionSuavizada = Vector3.Lerp(camaraPrincipal.transform.position, posicionDeseada, suavidad);
            camaraPrincipal.transform.position = posicionSuavizada;

            // Hacer que la cámara mire al jugador
            camaraPrincipal.transform.LookAt(objetivoASeguir);
        }
    }

    void CrearNuevaCamara()
    {
        GameObject nuevaCamara = new GameObject("Main Camera");
        camaraPrincipal = nuevaCamara.AddComponent<Camera>();
        nuevaCamara.tag = "MainCamera";

        // Configuración básica de la cámara
        camaraPrincipal.clearFlags = CameraClearFlags.Skybox;
        camaraPrincipal.depth = -1;
        camaraPrincipal.fieldOfView = 60f;
        camaraPrincipal.nearClipPlane = 0.3f;
        camaraPrincipal.farClipPlane = 1000f;

        // Añadir AudioListener si no existe
        if (FindObjectOfType<AudioListener>() == null)
        {
            nuevaCamara.AddComponent<AudioListener>();
        }

        Debug.Log("Cámara principal creada exitosamente.");
    }

    // Método público para cambiar el objetivo a seguir
    public void EstablecerObjetivo(Transform nuevoObjetivo)
    {
        objetivoASeguir = nuevoObjetivo;
    }

    // Método público para cambiar el offset
    public void EstablecerOffset(Vector3 nuevoOffset)
    {
        offsetPosicion = nuevoOffset;
    }
}