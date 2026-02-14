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

        // Resetear el dinero
        PlayerPrefs.SetFloat("Dinero", 100f);

        // Generar nueva deuda aleatoria entre 80,000 y 100,000
        int deudaEnMiles = Random.Range(80, 101); // 80 a 100 inclusive
        float nuevaDeuda = deudaEnMiles * 1000f;
        PlayerPrefs.SetFloat("Deuda", nuevaDeuda);

        // Establecer la posición inicial del jugador en la casa
        PlayerPrefs.SetFloat("SpawnX", -2f);
        PlayerPrefs.SetFloat("SpawnY", 1.3f);
        PlayerPrefs.SetFloat("SpawnZ", 24f);
        PlayerPrefs.SetInt("UseSpawnPosition", 1);

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