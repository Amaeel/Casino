using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionMuerte : MonoBehaviour
{
    public GameObject panelMuerte; // Arrastra aquí tu objeto PantallaMuerte

    public void EntrarEnBancarrota()
    {
        panelMuerte.SetActive(true); // Esto encenderá el panel de golpe
        Time.timeScale = 0f;        // Pausa el juego
        Cursor.lockState = CursorLockMode.None; // Libera el ratón para poder clicar
        Cursor.visible = true;
    }

    public void BotonReintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SalirAlMenu()
    {
        Debug.Log("Forzando carga de escena 0...");
        Time.timeScale = 1f;
        AudioListener.pause = false; // Por si acaso el audio bloquea algo

        // Cambiamos el nombre por el índice 0 (que suele ser el Menú)
        SceneManager.LoadScene(0);
    }
}