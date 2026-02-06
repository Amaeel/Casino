using UnityEngine;
using TMPro;

public class ActualizadorHUD : MonoBehaviour
{
    [Header("Referencias de la UI")]
    public TextMeshProUGUI textoDineroHUD; // Arrastra aquí el objeto "Dinero"
    public TextMeshProUGUI textoDeudaHUD;   // Arrastra aquí el objeto "Deuda"

    void Update()
    {
        // Verificamos que el sistema persistente exista
        if (PlayerPersistence.instance != null)
        {
            // 1. Actualiza el Dinero
            if (textoDineroHUD != null)
            {
                textoDineroHUD.text = "$" + PlayerPersistence.instance.GetCoins().ToString("N0");
            }

            // 2. Actualiza la Deuda (La misma que usa Trump)
            if (textoDeudaHUD != null)
            {
                // Mostramos la deuda del PlayerPersistence para que sea idéntica
                textoDeudaHUD.text = "Deuda: $" + PlayerPersistence.instance.deudaTotal.ToString("N0");
            }
        }
    }
}