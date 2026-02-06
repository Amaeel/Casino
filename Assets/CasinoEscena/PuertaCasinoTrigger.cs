using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PuertaCasinoTrigger : MonoBehaviour
{
    [Header("Configuración UI")]
    public GameObject mensajeUI;

    [Header("Configuración Escena")]
    public string nombreEscenaDestino = "CasinoEscena1";

    [Tooltip("Punto exacto (X, Y, Z) donde aparecerá el jugador en la escena de destino.")]
    public Vector3 puntoDeAparicion = new Vector3(0f, 1f, 0f);

    private bool jugadorCerca = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mensajeUI.SetActive(true);
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mensajeUI.SetActive(false);
            jugadorCerca = false;
        }
    }


    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            // Ocultar el mensaje inmediatamente
            mensajeUI.SetActive(false);

            PlayerPersistence playerPersistor = PlayerPersistence.instance;

            if (playerPersistor != null)
            {
                playerPersistor.LoadNewScene(nombreEscenaDestino, puntoDeAparicion);
            }
            else
            {
                Debug.LogError("PlayerPersistence no encontrado. Asegúrate de que tu Player tenga el script PlayerPersistence y la tag 'Player'.");
                SceneManager.LoadScene(nombreEscenaDestino);
            }
        }
    }
}