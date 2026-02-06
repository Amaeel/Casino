using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence instance;

    [Header("Economía y Deuda")]
    public long coins = 0;
    private const long MAX_COINS = 99999999;
    public long deudaTotal;
    private float coinsPerSecond = 0f;
    private float passiveUpdateTimer = 0f;

    [Header("Scripts de Movimiento")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour verticalMovementScript;

    private Vector3 nextSpawnPosition;
    private bool shouldTeleport = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CargarDatos(); // CARGA LOS DATOS AL INICIAR EL JUEGO
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
        // Si después de cargar la deuda sigue en 0, generamos una nueva
        if (deudaTotal <= 0)
        {
            deudaTotal = Random.Range(80, 101) * 1000;
            GuardarDatos();
        }
    }

    // --- SISTEMA DE PERSISTENCIA (GUARDADO LOCAL) ---
    public void GuardarDatos()
    {
        // Guardamos como String porque PlayerPrefs no soporta el tipo 'long'
        PlayerPrefs.SetString("Monedas_Guardadas", coins.ToString());
        PlayerPrefs.SetString("Deuda_Guardada", deudaTotal.ToString());
        PlayerPrefs.Save();
        Debug.Log("Datos guardados: $" + coins + " | Deuda: " + deudaTotal);
    }

    public void CargarDatos()
    {
        // Cargamos los strings y los convertimos de vuelta a números
        string monedasStr = PlayerPrefs.GetString("Monedas_Guardadas", "0");
        string deudaStr = PlayerPrefs.GetString("Deuda_Guardada", "0");

        coins = long.Parse(monedasStr);
        deudaTotal = long.Parse(deudaStr);
    }

    private void ConectarScripts()
    {
        if (playerMovementScript == null) playerMovementScript = GetComponent("Moviment_Cub") as MonoBehaviour;
        if (verticalMovementScript == null) verticalMovementScript = GetComponent("Moviment_vertical") as MonoBehaviour;
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

    public long GetCoins() => coins;

    public void AddCoins(int amount)
    {
        coins += amount;
        if (coins > MAX_COINS) coins = MAX_COINS;
        if (coins < 0) coins = 0;
        GuardarDatos(); // Guardado automático al ganar
    }

    public bool CanAfford(int cost) => coins >= (long)cost;

    public void DeductCoins(int cost)
    {
        if (CanAfford(cost))
        {
            coins -= cost;
            GuardarDatos(); // Guardado automático al gastar
        }
    }

    public void DisablePlayerControls()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (verticalMovementScript != null) verticalMovementScript.enabled = false;
    }

    public void EnablePlayerControls()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (verticalMovementScript != null) verticalMovementScript.enabled = true;
    }

    public void LoadNewScene(string sceneName, Vector3 spawnPoint)
    {
        nextSpawnPosition = spawnPoint;
        shouldTeleport = true;
        GuardarDatos(); // Aseguramos guardado antes de cambiar de escena
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (shouldTeleport)
        {
            transform.position = nextSpawnPosition;
            shouldTeleport = false;
        }
        ConectarScripts();
    }
}