using UnityEngine;
using UnityEngine.SceneManagement;

public class TeletransporteUniversal : MonoBehaviour
{
    public static string escenaDeOrigen;

    [Header("Configuracion de Destino")]
    public string nombreEscenaDestino;
    public Vector3 puntoDeAparicion;

    [Header("Condicion de Historia")]
    public bool requiereNotaLeida = true;

    [Header("Interfaz Visual")]
    public GameObject promptVisual;

    private bool jugadorCerca = false;

    void Start()
    {
        if (promptVisual != null) promptVisual.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Viajar();
        }
    }

    private void Viajar()
    {
        if (requiereNotaLeida && !LoreNota.notaLeidaGlobal)
        {
            Debug.Log("Bloqueado: Lee la nota primero.");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(nombreEscenaDestino))
        {
            escenaDeOrigen = SceneManager.GetActiveScene().name;
            if (PlayerPersistence.instance != null)
                PlayerPersistence.instance.LoadNewScene(nombreEscenaDestino, puntoDeAparicion);
            else
                SceneManager.LoadScene(nombreEscenaDestino);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Solo mostramos el texto si la nota ya fue leída globalmente
            if (LoreNota.notaLeidaGlobal || !requiereNotaLeida)
            {
                jugadorCerca = true;
                if (promptVisual != null) promptVisual.SetActive(true); // Activa el texto "Pulsa E para salir"
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (promptVisual != null) promptVisual.SetActive(false); // Lo oculta al alejarse
        }
    }
}