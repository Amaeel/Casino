using UnityEngine;
using TMPro;

public class LoreNota : MonoBehaviour
{
    public static bool notaLeidaGlobal = false;

    [Header("Referencias de Interfaz")]
    public GameObject canvasNotaPrincipal;
    public GameObject objetoTexto;
    public GameObject promptFlotante;

    [Header("Configuracion de Bloqueo")]
    public GameObject muroPuerta;

    private bool jugadorEnRango = false;

    void Start()
    {
        if (canvasNotaPrincipal != null) canvasNotaPrincipal.SetActive(false);
        if (promptFlotante != null) promptFlotante.SetActive(false);
        notaLeidaGlobal = false;
    }

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.E))
        {
            if (!canvasNotaPrincipal.activeSelf)
                AbrirNota();
            else
                CerrarNota();
        }
    }

    public void AbrirNota()
    {
        if (canvasNotaPrincipal != null) canvasNotaPrincipal.SetActive(true);
        if (objetoTexto != null)
        {
            objetoTexto.SetActive(true);

            // CORRECCIÓN: Ahora lee de PlayerPersistence en lugar de DeudaManager
            if (PlayerPersistence.instance != null)
            {
                objetoTexto.GetComponent<TextMeshProUGUI>().text =
                    "Despiertas con una deuda de $" + PlayerPersistence.instance.deudaTotal.ToString("N0") +
                    " monedas con el Faraón. Si tu saldo llega a 0, no habrá mañana para ti. " +
                    "Encuentra la forma de pagar o quédate atrapado en su casino para siempre.";
            }
        }

        if (promptFlotante != null) promptFlotante.SetActive(false);

        Time.timeScale = 0f; // Pausa el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Desactivamos controles del jugador para que no se mueva mientras lee
        if (PlayerPersistence.instance != null) PlayerPersistence.instance.DisablePlayerControls();
    }

    public void CerrarNota()
    {
        Time.timeScale = 1f;
        notaLeidaGlobal = true;

        if (canvasNotaPrincipal != null) canvasNotaPrincipal.SetActive(false);
        if (muroPuerta != null) muroPuerta.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reactivamos controles del jugador
        if (PlayerPersistence.instance != null) PlayerPersistence.instance.EnablePlayerControls();

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = true;
            if (promptFlotante != null) promptFlotante.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            if (promptFlotante != null) promptFlotante.SetActive(false);

            if (canvasNotaPrincipal != null && canvasNotaPrincipal.activeSelf)
                CerrarNota();
        }
    }
}