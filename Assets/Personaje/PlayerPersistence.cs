using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    // Patrón Singleton
    public static PlayerPersistence instance;

    [Header("Estadísticas del Jugador")]
    public float coins = 100f;
    public float deudaTotal = 1000f;
    public bool deudaPagada = false;

    [Header("Referencias de Scripts")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour verticalMovementScript;
    public CameraManager cameraManager; // Gestor de cámara

    [Header("Referencias UI")]
    public GameObject pantallaGameOver;

    private GestionMuerte gestionMuerte;
    private bool juegoTerminado = false;
    private bool controlsEnabled = true;

    // ========== NUEVO: Variables para guardado automático ==========
    private float tiempoUltimoGuardado = 0f;
    private float intervaloGuardado = 5f; // Guardar cada 5 segundos
    // ================================================================

    void Awake()
    {
        // Implementar Singleton
        if (instance == null)
        {
            instance = this;
            // No destruir este objeto al cambiar de escena si es necesario
            // DontDestroyOnLoad(gameObject); // Descomenta si quieres que persista entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ========== NUEVO: Guardar escena actual ==========
        GuardarEscenaActual();
        // ==================================================

        // Cargar datos guardados
        CargarDatos();

        // Verificar si hay una posición de spawn guardada
        if (PlayerPrefs.GetInt("UseSpawnPosition", 0) == 1)
        {
            // Obtener la posición guardada
            float spawnX = PlayerPrefs.GetFloat("SpawnX", 0f);
            float spawnY = PlayerPrefs.GetFloat("SpawnY", 0f);
            float spawnZ = PlayerPrefs.GetFloat("SpawnZ", 0f);

            // Mover al jugador a esa posición
            transform.position = new Vector3(spawnX, spawnY, spawnZ);

            // Limpiar el flag para que no se use en la próxima escena
            PlayerPrefs.SetInt("UseSpawnPosition", 0);
            PlayerPrefs.Save();
        }

        // Verificar que haya una cámara en la escena
        VerificarCamara();

        // Obtener referencia al script de gestión de muerte
        if (pantallaGameOver != null)
        {
            gestionMuerte = pantallaGameOver.GetComponent<GestionMuerte>();
        }

        // Verificar condición de muerte al iniciar
        VerificarMuerte();
    }

    void VerificarCamara()
    {
        // Si no hay CameraManager asignado, intentar encontrarlo
        if (cameraManager == null)
        {
            cameraManager = FindObjectOfType<CameraManager>();
        }

        // Si no hay cámara principal, buscarla o advertir
        Camera camaraPrincipal = Camera.main;
        if (camaraPrincipal == null)
        {
            camaraPrincipal = FindObjectOfType<Camera>();

            if (camaraPrincipal == null)
            {
                Debug.LogWarning("No se encontró cámara en la escena. Considera añadir una cámara o el script CameraManager.");
            }
        }

        // Si tenemos CameraManager, establecer el jugador como objetivo
        if (cameraManager != null)
        {
            cameraManager.EstablecerObjetivo(transform);
        }
    }

    void Update()
    {
        // Verificar constantemente si el jugador debe morir
        if (!juegoTerminado)
        {
            VerificarMuerte();
        }

        // ========== NUEVO: Guardar posición cada 5 segundos ==========
        tiempoUltimoGuardado += Time.deltaTime;
        if (tiempoUltimoGuardado >= intervaloGuardado)
        {
            GuardarPosicionActual();
            tiempoUltimoGuardado = 0f;
        }
        // =============================================================
    }

    // ========== NUEVO: Métodos de guardado automático ==========
    void GuardarEscenaActual()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("UltimaEscena", escenaActual);
        PlayerPrefs.Save();
    }

    void GuardarPosicionActual()
    {
        PlayerPrefs.SetFloat("UltimaPosX", transform.position.x);
        PlayerPrefs.SetFloat("UltimaPosY", transform.position.y);
        PlayerPrefs.SetFloat("UltimaPosZ", transform.position.z);
        PlayerPrefs.Save();
    }
    // ===========================================================

    void CargarDatos()
    {
        // Cargar dinero
        if (PlayerPrefs.HasKey("Dinero"))
        {
            coins = PlayerPrefs.GetFloat("Dinero");
        }
        else
        {
            coins = 100f; // Valor inicial
            PlayerPrefs.SetFloat("Dinero", coins);
        }

        // Cargar deuda
        if (PlayerPrefs.HasKey("Deuda"))
        {
            deudaTotal = PlayerPrefs.GetFloat("Deuda");
        }
        else
        {
            // Generar deuda aleatoria entre 80,000 y 100,000 (en números enteros de miles)
            int deudaEnMiles = Random.Range(80, 101); // 80 a 100 inclusive
            deudaTotal = deudaEnMiles * 1000f; // Convertir a miles (80000, 81000, 82000... 100000)
            PlayerPrefs.SetFloat("Deuda", deudaTotal);
            Debug.Log("Nueva deuda generada: $" + deudaTotal);
        }

        // Cargar estado de deuda pagada
        if (PlayerPrefs.HasKey("DeudaPagada"))
        {
            deudaPagada = PlayerPrefs.GetInt("DeudaPagada") == 1;
        }
        else
        {
            deudaPagada = false;
            PlayerPrefs.SetInt("DeudaPagada", 0);
        }

        PlayerPrefs.Save();
    }

    public void GuardarDatos()
    {
        PlayerPrefs.SetFloat("Dinero", coins);
        PlayerPrefs.SetFloat("Deuda", deudaTotal);
        PlayerPrefs.SetInt("DeudaPagada", deudaPagada ? 1 : 0);
        PlayerPrefs.Save();
    }

    void VerificarMuerte()
    {
        // Condiciones de muerte:
        // 1. Dinero llega a 0 o menos
        // 2. Dinero es menor que 20
        if (coins <= 0 || coins < 20)
        {
            MorirJugador();
        }
    }

    void MorirJugador()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        // Desactivar el movimiento del jugador
        DisablePlayerControls();

        // Mostrar la pantalla de muerte
        if (gestionMuerte != null)
        {
            gestionMuerte.MostrarPantallaMuerte();
        }
        else if (pantallaGameOver != null)
        {
            pantallaGameOver.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // ============================================
    // MÉTODOS PARA TIENDA TRUMP Y OTROS SCRIPTS
    // ============================================

    /// <summary>
    /// Obtiene el dinero actual del jugador
    /// </summary>
    public float GetCoins()
    {
        return coins;
    }

    /// <summary>
    /// Verifica si el jugador tiene suficiente dinero
    /// </summary>
    public bool CanAfford(float amount)
    {
        return coins >= amount;
    }

    /// <summary>
    /// Resta dinero del jugador
    /// </summary>
    public void DeductCoins(float amount)
    {
        coins -= amount;
        if (coins < 0) coins = 0;
        GuardarDatos();
    }

    /// <summary>
    /// Añade dinero al jugador
    /// </summary>
    public void AddCoins(float amount)
    {
        coins += amount;
        GuardarDatos();
    }

    /// <summary>
    /// Desactiva los controles del jugador
    /// </summary>
    public void DisablePlayerControls()
    {
        controlsEnabled = false;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (verticalMovementScript != null)
        {
            verticalMovementScript.enabled = false;
        }
    }

    /// <summary>
    /// Activa los controles del jugador
    /// </summary>
    public void EnablePlayerControls()
    {
        if (juegoTerminado) return; // No reactivar si el juego terminó

        controlsEnabled = true;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        if (verticalMovementScript != null)
        {
            verticalMovementScript.enabled = true;
        }
    }

    // ============================================
    // MÉTODOS LEGACY (Compatibilidad)
    // ============================================

    /// <summary>
    /// Método público para añadir dinero (alias de AddCoins)
    /// </summary>
    public void AñadirDinero(float cantidad)
    {
        AddCoins(cantidad);
    }

    /// <summary>
    /// Método público para restar dinero (alias de DeductCoins)
    /// </summary>
    public void RestarDinero(float cantidad)
    {
        DeductCoins(cantidad);
    }

    /// <summary>
    /// Método público para añadir deuda
    /// </summary>
    public void AñadirDeuda(float cantidad)
    {
        deudaTotal += cantidad;
        GuardarDatos();
    }

    /// <summary>
    /// Método público para pagar deuda
    /// </summary>
    public void PagarDeuda(float cantidad)
    {
        deudaTotal -= cantidad;
        if (deudaTotal <= 0)
        {
            deudaTotal = 0;
            deudaPagada = true;
        }
        GuardarDatos();
    }

    /// <summary>
    /// Método para obtener el dinero actual
    /// </summary>
    public float ObtenerDinero()
    {
        return coins;
    }

    /// <summary>
    /// Método para obtener la deuda actual
    /// </summary>
    public float ObtenerDeuda()
    {
        return deudaTotal;
    }

    /// <summary>
    /// Método para establecer el dinero directamente
    /// </summary>
    public void EstablecerDinero(float cantidad)
    {
        coins = cantidad;
        GuardarDatos();
    }

    /// <summary>
    /// Método para establecer la deuda directamente
    /// </summary>
    public void EstablecerDeuda(float cantidad)
    {
        deudaTotal = cantidad;
        GuardarDatos();
    }

    /// <summary>
    /// Carga una nueva escena y guarda los datos antes de cambiar
    /// </summary>
    public void LoadNewScene(string sceneName)
    {
        // ========== NUEVO: Guardar escena como última escena ==========
        PlayerPrefs.SetString("UltimaEscena", sceneName);
        // ==============================================================

        // Guardar los datos antes de cambiar de escena
        GuardarDatos();

        // Reanudar el tiempo por si estaba pausado
        Time.timeScale = 1f;

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Carga una nueva escena y coloca al jugador en una posición específica
    /// </summary>
    public void LoadNewScene(string sceneName, Vector3 spawnPosition)
    {
        // ========== NUEVO: Guardar escena como última escena ==========
        PlayerPrefs.SetString("UltimaEscena", sceneName);
        // ==============================================================

        // Guardar la posición de spawn para usarla cuando se cargue la escena
        PlayerPrefs.SetFloat("SpawnX", spawnPosition.x);
        PlayerPrefs.SetFloat("SpawnY", spawnPosition.y);
        PlayerPrefs.SetFloat("SpawnZ", spawnPosition.z);
        PlayerPrefs.SetInt("UseSpawnPosition", 1);

        // ========== NUEVO: También guardar como última posición ==========
        PlayerPrefs.SetFloat("UltimaPosX", spawnPosition.x);
        PlayerPrefs.SetFloat("UltimaPosY", spawnPosition.y);
        PlayerPrefs.SetFloat("UltimaPosZ", spawnPosition.z);
        // =================================================================

        PlayerPrefs.Save();

        // Guardar los datos del jugador antes de cambiar de escena
        GuardarDatos();

        // Reanudar el tiempo por si estaba pausado
        Time.timeScale = 1f;

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Carga una nueva escena por índice
    /// </summary>
    public void LoadNewScene(int sceneIndex)
    {
        // Guardar los datos antes de cambiar de escena
        GuardarDatos();

        // Reanudar el tiempo por si estaba pausado
        Time.timeScale = 1f;

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneIndex);
    }

    void OnApplicationQuit()
    {
        GuardarDatos();
        // ========== NUEVO: Guardar posición al salir ==========
        GuardarPosicionActual();
        // ======================================================
    }

    void OnDestroy()
    {
        // Limpiar la instancia singleton cuando se destruya
        if (instance == this)
        {
            instance = null;
        }
    }
}