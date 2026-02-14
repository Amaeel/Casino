using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Botones del Menú")]
    public Button botonNuevaPartida;
    public Button botonCargarPartida; // NUEVO
    public Button botonSalir;

    void Start()
    {
        // Asignar funciones a los botones
        if (botonNuevaPartida != null)
        {
            botonNuevaPartida.onClick.AddListener(NuevaPartida);
        }

        if (botonCargarPartida != null)
        {
            botonCargarPartida.onClick.AddListener(CargarPartida);

            // Verificar si hay partida guardada para activar/desactivar el botón
            bool hayPartidaGuardada = PlayerPrefs.HasKey("Dinero");
            botonCargarPartida.interactable = hayPartidaGuardada;

            if (!hayPartidaGuardada)
            {
                Debug.Log("No hay partida guardada. Botón 'Cargar Partida' desactivado.");
            }
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.AddListener(SalirJuego);
        }
    }

    public void NuevaPartida()
    {
        // IMPORTANTE: Borrar TODOS los datos para empezar desde cero
        PlayerPrefs.DeleteAll();

        // Establecer valores iniciales
        PlayerPrefs.SetFloat("Dinero", 100f);
        // La deuda se generará aleatoriamente en PlayerPersistence al cargar

        // Establecer la posición inicial del jugador en la casa (SIEMPRE)
        PlayerPrefs.SetFloat("SpawnX", -2f);
        PlayerPrefs.SetFloat("SpawnY", 1.3f);
        PlayerPrefs.SetFloat("SpawnZ", 24f);
        PlayerPrefs.SetInt("UseSpawnPosition", 1);

        // Marcar que es una partida nueva (no continuación)
        PlayerPrefs.SetInt("PartidaNueva", 1);

        PlayerPrefs.Save();

        Debug.Log("Nueva partida iniciada. Spawn en casa: (-2, 1.3, 24)");

        // Cargar la escena de Casa
        SceneManager.LoadScene("Casa");
    }

    public void CargarPartida()
    {
        // Verificar si hay una partida guardada
        if (!PlayerPrefs.HasKey("Dinero"))
        {
            Debug.LogWarning("No hay partida guardada. Iniciando nueva partida...");
            NuevaPartida();
            return;
        }

        // Verificar si hay una escena guardada
        string ultimaEscena = "Casa"; // Por defecto Casa

        if (PlayerPrefs.HasKey("UltimaEscena"))
        {
            ultimaEscena = PlayerPrefs.GetString("UltimaEscena");
        }

        // Verificar si hay posición guardada
        if (PlayerPrefs.HasKey("UltimaPosX"))
        {
            float posX = PlayerPrefs.GetFloat("UltimaPosX");
            float posY = PlayerPrefs.GetFloat("UltimaPosY");
            float posZ = PlayerPrefs.GetFloat("UltimaPosZ");

            // Establecer la posición de spawn
            PlayerPrefs.SetFloat("SpawnX", posX);
            PlayerPrefs.SetFloat("SpawnY", posY);
            PlayerPrefs.SetFloat("SpawnZ", posZ);
            PlayerPrefs.SetInt("UseSpawnPosition", 1);
            PlayerPrefs.Save();

            Debug.Log("Cargando partida en " + ultimaEscena + " en posición: (" + posX + ", " + posY + ", " + posZ + ")");
        }
        else
        {
            Debug.Log("No hay posición guardada. Usando posición por defecto en " + ultimaEscena);
        }

        // Cargar la escena
        SceneManager.LoadScene(ultimaEscena);
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