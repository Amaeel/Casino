using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionMuerte : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelMuerte;

    void Start()
    {
        // Asegurarse de que el panel esté oculto al inicio
        if (panelMuerte != null)
        {
            panelMuerte.SetActive(false);
        }
    }

    public void MostrarPantallaMuerte()
    {
        if (panelMuerte != null)
        {
            panelMuerte.SetActive(true);
            Time.timeScale = 0f; // Pausar el juego
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void BotonReintentar()
    {
        // Reanudar el tiempo
        Time.timeScale = 1f;

        // Resetear el dinero y la deuda
        PlayerPrefs.SetFloat("Dinero", 100f); // Valor inicial del dinero
        PlayerPrefs.SetFloat("Deuda", 1000f); // Valor inicial de la deuda (ajusta según tu juego)
        PlayerPrefs.Save();

        // Recargar la escena de Casa
        SceneManager.LoadScene("Casa");
    }

    public void SalirAlMenu()
    {
        // Reanudar el tiempo
        Time.timeScale = 1f;

        // Cargar la escena del menú principal
        SceneManager.LoadScene("Menu");
    }
}
