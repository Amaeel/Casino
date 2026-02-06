using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro

public class Dinero : MonoBehaviour
{
    // Asigna este TextMeshProUGUI en el Inspector
    public TextMeshProUGUI coinTextDisplay;

    private PlayerPersistence playerPersistor;
    private float refreshTimer = 0f;
    private const float RefreshRate = 0.5f; // Actualiza la UI dos veces por segundo

    void Start()
    {
        // Busca la instancia del script persistente
        playerPersistor = PlayerPersistence.instance;

        if (playerPersistor == null)
        {
            Debug.LogError("GlobalCoinDisplay no pudo encontrar PlayerPersistence. Asegúrate de que esté activo.");
            return;
        }

        // Asegúrate de tener la referencia al texto
        if (coinTextDisplay == null)
        {
            // Intenta obtenerlo si está en el mismo objeto
            coinTextDisplay = GetComponent<TextMeshProUGUI>();
        }

        // Realiza una actualización inicial
        UpdateCoinDisplay();
    }

    void Update()
    {
        // Actualiza el texto periódicamente para reflejar los cambios del Autoclicker
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= RefreshRate)
        {
            UpdateCoinDisplay();
            refreshTimer = 0f;
        }
    }

    private void UpdateCoinDisplay()
    {
        if (playerPersistor != null && coinTextDisplay != null)
        {
            // Obtiene las monedas y las muestra con formato de miles (1,000,000)
            coinTextDisplay.text = "$" + playerPersistor.GetCoins().ToString("N0");
        }
    }
}