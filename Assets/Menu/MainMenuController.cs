using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Botones del Menú")]
    public Button botonNuevaPartida;
    public Button botonSalir;

    void Start()
    {
        // Asignar funciones a los botones
        if (botonNuevaPartida != null)
        {
            botonNuevaPartida.onClick.AddListener(NuevaPartida);
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.AddListener(SalirJuego);
        }
    }

    public void NuevaPartida()
    {
        // Resetear los datos del jugador antes de empezar
        PlayerPrefs.DeleteKey("Dinero");
        PlayerPrefs.DeleteKey("Deuda");
        PlayerPrefs.DeleteKey("NotaLeida");
        PlayerPrefs.Save();

        // Cargar la escena de Casa
        SceneManager.LoadScene("Casa");
    }

    public void SalirJuego()
    {
        // Salir del juego
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
