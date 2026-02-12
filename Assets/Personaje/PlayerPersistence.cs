using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence instance;

    [Header("Economía y Deuda")]
    public long coins = 20;
    private const long MAX_COINS = 99999999;
    public long deudaTotal;
    private float coinsPerSecond = 0f;
    private float passiveUpdateTimer = 0f;

    [Header("Scripts de Movimiento")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour verticalMovementScript;

    [Header("Game Over UI")]
    public GameObject pantallaGameOver;

    private Vector3 nextSpawnPosition;
    private bool shouldTeleport = false;

    // ESTA VARIABLE ES OBLIGATORIA PARA QUE NO SE CONGELE EL BOTÓN
    private bool yaMurio = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CargarDatos();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        ConectarScripts();
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("Monedas_Guardadas"))
        {
            coins = 20;
            deudaTotal = Random.Range(80, 101) * 1000;
            GuardarDatos();
        }
        if (pantallaGameOver != null) pantallaGameOver.SetActive(false);
    }

    // --- AQUÍ ESTÁN LOS BOTONES QUE FALTABAN ---
    // TIENES QUE ARRASTRAR "PlayerPersistence" AL ONCLICK DEL BOTÓN EN UNITY
    public void BotonReintentar()
    {
        yaMurio = false; // Reseteamos el bloqueo
        Time.timeScale = 1f; // Descongelamos el juego
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BotonSalirAlMenu()
    {
        yaMurio = false; // Reseteamos el bloqueo
        Time.timeScale = 1f; // IMPRESCINDIBLE: Descongelar para poder cambiar de escena
        SceneManager.LoadScene("Menu"); // Asegúrate de que tu escena se llama "Menu"
    }
    // -------------------------------------------

    public long GetCoins() => coins;

    public bool CanAfford(int cost) => coins >= (long)cost;

    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins > MAX_COINS) coins = MAX_COINS;

        // MODIFICADO: Solo ejecuta muerte si no ha muerto ya
        if (coins <= 0 && !yaMurio) { coins = 0; EjecutarGameOver(); }

        GuardarDatos();
    }

    public void DeductCoins(int cost)
    {
        coins -= cost;

        // MODIFICADO: Solo ejecuta muerte si no ha muerto ya
        if (coins <= 0 && !yaMurio) { coins = 0; EjecutarGameOver(); }

        GuardarDatos();
    }

    public void LoadNewScene(string sceneName, Vector3 spawnPoint)
    {
        nextSpawnPosition = spawnPoint;
        shouldTeleport = true;
        GuardarDatos();
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f; // Asegurar que el tiempo corre al cambiar de escena
        yaMurio = false; // Resetear el estado de muerte

        if (shouldTeleport)
        {
            transform.position = nextSpawnPosition;
            shouldTeleport = false;
        }
        ConectarScripts();

        // Parche de seguridad: Si la pantalla de muerte se pierde al cambiar de escena, la busca
        if (pantallaGameOver == null) pantallaGameOver = GameObject.Find("PantallaMuerte");
        if (pantallaGameOver != null) pantallaGameOver.SetActive(false);
    }

    public void EnablePlayerControls()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (verticalMovementScript != null) verticalMovementScript.enabled = true;
    }

    public void DisablePlayerControls()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (verticalMovementScript != null) verticalMovementScript.enabled = false;
    }

    private void ConectarScripts()
    {
        if (playerMovementScript == null) playerMovementScript = GetComponent("Moviment_Cub") as MonoBehaviour;
        if (verticalMovementScript == null) verticalMovementScript = GetComponent("Moviment_vertical") as MonoBehaviour;
    }

    private void EjecutarGameOver()
    {
        if (pantallaGameOver != null)
        {
            yaMurio = true; // ACTIVAMOS EL BLOQUEO para que no se repita el bucle
            pantallaGameOver.SetActive(true);

            Time.timeScale = 0f; // Aquí se pausa el juego

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            DisablePlayerControls();

            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }

    public void GuardarDatos()
    {
        PlayerPrefs.SetString("Monedas_Guardadas", coins.ToString());
        PlayerPrefs.SetString("Deuda_Guardada", deudaTotal.ToString());
        PlayerPrefs.Save();
    }

    public void CargarDatos()
    {
        string monedasStr = PlayerPrefs.GetString("Monedas_Guardadas", "300");
        string deudaStr = PlayerPrefs.GetString("Deuda_Guardada", "0");
        long.TryParse(monedasStr, out coins);
        long.TryParse(deudaStr, out deudaTotal);
    }

    void Update()
    {
        passiveUpdateTimer += Time.deltaTime;
        if (passiveUpdateTimer >= 1f)
        {
            int passiveGain = Mathf.RoundToInt(coinsPerSecond);
            if (passiveGain > 0) AddCoins(passiveGain);
            passiveUpdateTimer -= 1f;
        }
    }
}