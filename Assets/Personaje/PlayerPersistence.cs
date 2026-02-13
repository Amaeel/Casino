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

    [Header("Referencias UI")]
    public GameObject pantallaGameOver;

    private GestionMuerte gestionMuerte;
    private bool juegoTerminado = false;
    private bool controlsEnabled = true;

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
        // Cargar datos guardados
        CargarDatos();

        // Obtener referencia al script de gestión de muerte
        if (pantallaGameOver != null)
        {
            gestionMuerte = pantallaGameOver.GetComponent<GestionMuerte>();
        }

        // Verificar condición de muerte al iniciar
        VerificarMuerte();
    }

    void Update()
    {
        // Verificar constantemente si el jugador debe morir
        if (!juegoTerminado)
        {
            VerificarMuerte();
        }
    }

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
            deudaTotal = 1000f; // Valor inicial
            PlayerPrefs.SetFloat("Deuda", deudaTotal);
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

    void OnApplicationQuit()
    {
        GuardarDatos();
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
