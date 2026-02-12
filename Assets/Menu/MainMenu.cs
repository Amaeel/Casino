using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreMapaJuego = "Casa";

    [Header("Paneles")]
    public GameObject panelAjustes;
    public GameObject panelControles;

    // 1. NUEVA PARTIDA: Borra todo y empieza de cero
    public void NuevaPartida()
    {
        PlayerPrefs.DeleteAll(); // Adiós deudas y monedas viejas
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreMapaJuego);
    }

    // 2. CARGAR PARTIDA: Entra al mapa respetando los PlayerPrefs
    public void CargarPartida()
    {
        if (PlayerPrefs.HasKey("Monedas_Guardadas"))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(nombreMapaJuego);
        }
        else
        {
            Debug.Log("No hay partida guardada, empieza una Nueva Partida.");
            // Opcional: podrías hacer que el botón se vea gris si no hay datos
        }
    }

    // 3. AJUSTES Y CONTROLES (Sencillos)
    public void AbrirAjustes() => panelAjustes.SetActive(true);
    public void CerrarAjustes() => panelAjustes.SetActive(false);

    public void AbrirControles() => panelControles.SetActive(true);
    public void CerrarControles() => panelControles.SetActive(false);

    // 4. SALIR
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo de DebtOrDie...");
        Application.Quit();
    }
}