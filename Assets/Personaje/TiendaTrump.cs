using UnityEngine;
using TMPro;

public class TiendaTrump : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    public GameObject panelInterfazTienda;
    public GameObject hudNormalJugador;

    [Header("Textos de la Tienda")]
    public TextMeshProUGUI textoDineroEnTienda;
    public TextMeshProUGUI textoDeudaEnTienda;

    private bool jugadorCerca = false;

    void Start()
    {
        if (panelInterfazTienda != null) panelInterfazTienda.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (!panelInterfazTienda.activeSelf) AbrirInterfaz();
            else Cerrar();
        }
    }

    public void AbrirInterfaz()
    {
        panelInterfazTienda.SetActive(true);
        if (hudNormalJugador != null) hudNormalJugador.SetActive(false);

        if (PlayerPersistence.instance != null)
        {
            PlayerPersistence.instance.DisablePlayerControls();
            ActualizarUI();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Cerrar()
    {
        panelInterfazTienda.SetActive(false);
        if (hudNormalJugador != null) hudNormalJugador.SetActive(true);

        if (PlayerPersistence.instance != null)
            PlayerPersistence.instance.EnablePlayerControls();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ActualizarUI()
    {
        if (PlayerPersistence.instance != null)
        {
            if (textoDineroEnTienda != null)
                textoDineroEnTienda.text = "Dinero: $" + PlayerPersistence.instance.GetCoins();

            if (textoDeudaEnTienda != null)
                textoDeudaEnTienda.text = "Deuda Restante: $" + PlayerPersistence.instance.deudaTotal;

            if (PlayerPersistence.instance.deudaTotal <= 0)
            {
                textoDeudaEnTienda.text = "¡DEUDA PAGADA! ERES LIBRE";
                textoDeudaEnTienda.color = Color.green;
            }
        }
    }

    public void BotonPagarMil()
    {
        if (PlayerPersistence.instance != null)
        {
            if (PlayerPersistence.instance.CanAfford(1000) && PlayerPersistence.instance.deudaTotal > 0)
            {
                PlayerPersistence.instance.DeductCoins(1000);
                PlayerPersistence.instance.deudaTotal -= 1000;

                if (PlayerPersistence.instance.deudaTotal < 0)
                    PlayerPersistence.instance.deudaTotal = 0;

                ActualizarUI();
            }
        }
    }

    // ELIMINADO EL INCREASECPS PARA EVITAR EL ERROR
    public void BotonComprarBoost()
    {
        if (PlayerPersistence.instance != null)
        {
            int costeBoost = 5000;

            if (PlayerPersistence.instance.CanAfford(costeBoost))
            {
                PlayerPersistence.instance.DeductCoins(costeBoost);

                // Aquí podrías poner otra cosa en el futuro, 
                // por ahora solo resta el dinero para que no dé error.
                Debug.Log("Boost comprado (Dinero descontado).");

                ActualizarUI();
            }
            else
            {
                Debug.Log("Dinero insuficiente para Boost.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { jugadorCerca = false; Cerrar(); }
    }
}