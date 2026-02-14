using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TiendaTrump : MonoBehaviour
{
    [Header("Paneles de Interfaz")]
    public GameObject panelInterfazTienda;
    public GameObject hudNormalJugador;
    public GameObject promptHablarConTrump; // Nuevo: "Pulsa E para hablar con Trump"

    [Header("Textos de la Tienda")]
    public TextMeshProUGUI textoDineroEnTienda;
    public TextMeshProUGUI textoDeudaEnTienda;
    public TextMeshProUGUI textoBoostActivo; // Nuevo: Muestra si el boost está activo

    [Header("Botones")]
    public Button botonPagar1000;
    public Button botonPagar5000;
    public Button botonPagar10000;
    public Button botonComprarBoost;
    public Button botonCerrar;

    [Header("Sistema de Boost")]
    public int costeBoost = 5000;
    public float multiplicadorBoost = 2f; // Multiplica las probabilidades x2
    public int duracionBoostEnJugadas = 10; // Dura 10 jugadas
    private int jugadasRestantesBoost = 0;

    [Header("Audio y Efectos")]
    public AudioClip sonidoAbrirTienda;
    public AudioClip sonidoComprar;
    public AudioClip sonidoPagarDeuda;
    public AudioClip sonidoBoostActivado;
    public AudioClip[] sonidosTrumpHablando; // Array de frases de Trump
    private AudioSource audioSource;

    [Header("Animación Trump (Opcional)")]
    public Animator animadorTrump; // Si quieres animar a Trump cuando hablas

    private bool jugadorCerca = false;
    private bool tiendaAbierta = false;

    void Start()
    {
        if (panelInterfazTienda != null) panelInterfazTienda.SetActive(false);
        if (promptHablarConTrump != null) promptHablarConTrump.SetActive(false);

        // Obtener o crear AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configurar botones
        ConfigurarBotones();

        // Cargar estado del boost
        CargarEstadoBoost();
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (!tiendaAbierta) AbrirInterfaz();
            else Cerrar();
        }
    }

    void ConfigurarBotones()
    {
        if (botonPagar1000 != null)
            botonPagar1000.onClick.AddListener(() => PagarDeuda(1000));

        if (botonPagar5000 != null)
            botonPagar5000.onClick.AddListener(() => PagarDeuda(5000));

        if (botonPagar10000 != null)
            botonPagar10000.onClick.AddListener(() => PagarDeuda(10000));

        if (botonComprarBoost != null)
            botonComprarBoost.onClick.AddListener(ComprarBoost);

        if (botonCerrar != null)
            botonCerrar.onClick.AddListener(Cerrar);
    }

    public void AbrirInterfaz()
    {
        tiendaAbierta = true;
        panelInterfazTienda.SetActive(true);
        if (hudNormalJugador != null) hudNormalJugador.SetActive(false);

        if (PlayerPersistence.instance != null)
        {
            PlayerPersistence.instance.DisablePlayerControls();
            ActualizarUI();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Reproducir sonido de abrir tienda
        ReproducirSonido(sonidoAbrirTienda);

        // Reproducir frase aleatoria de Trump
        ReproducirFraseTrump();

        // Animar a Trump si tiene animador
        if (animadorTrump != null)
        {
            animadorTrump.SetTrigger("Hablar");
        }
    }

    public void Cerrar()
    {
        tiendaAbierta = false;
        panelInterfazTienda.SetActive(false);
        if (hudNormalJugador != null) hudNormalJugador.SetActive(true);

        if (PlayerPersistence.instance != null)
            PlayerPersistence.instance.EnablePlayerControls();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Guardar estado del boost
        GuardarEstadoBoost();
    }

    public void ActualizarUI()
    {
        if (PlayerPersistence.instance != null)
        {
            // Actualizar dinero
            if (textoDineroEnTienda != null)
                textoDineroEnTienda.text = "Dinero: $" + PlayerPersistence.instance.GetCoins().ToString("N0");

            // Actualizar deuda
            if (textoDeudaEnTienda != null)
            {
                float deudaActual = PlayerPersistence.instance.deudaTotal;

                if (deudaActual <= 0)
                {
                    textoDeudaEnTienda.text = "¡DEUDA PAGADA! ERES LIBRE";
                    textoDeudaEnTienda.color = Color.green;
                }
                else
                {
                    textoDeudaEnTienda.text = "Deuda Restante: $" + deudaActual.ToString("N0");
                    textoDeudaEnTienda.color = Color.red;
                }
            }

            // Actualizar estado del boost
            if (textoBoostActivo != null)
            {
                if (jugadasRestantesBoost > 0)
                {
                    textoBoostActivo.text = "BOOST ACTIVO: " + jugadasRestantesBoost + " jugadas restantes";
                    textoBoostActivo.color = Color.yellow;
                }
                else
                {
                    textoBoostActivo.text = "Boost: Compra para mejores probabilidades";
                    textoBoostActivo.color = Color.white;
                }
            }
        }

        // Actualizar interactividad de botones
        ActualizarBotones();
    }

    void ActualizarBotones()
    {
        if (PlayerPersistence.instance == null) return;

        float dinero = PlayerPersistence.instance.GetCoins();
        float deuda = PlayerPersistence.instance.deudaTotal;

        // Botones de pagar deuda
        if (botonPagar1000 != null)
            botonPagar1000.interactable = dinero >= 1000 && deuda > 0;

        if (botonPagar5000 != null)
            botonPagar5000.interactable = dinero >= 5000 && deuda > 0;

        if (botonPagar10000 != null)
            botonPagar10000.interactable = dinero >= 10000 && deuda > 0;

        // Botón de boost
        if (botonComprarBoost != null)
            botonComprarBoost.interactable = dinero >= costeBoost && jugadasRestantesBoost == 0;
    }

    public void PagarDeuda(int cantidad)
    {
        if (PlayerPersistence.instance != null)
        {
            if (PlayerPersistence.instance.CanAfford(cantidad) && PlayerPersistence.instance.deudaTotal > 0)
            {
                PlayerPersistence.instance.DeductCoins(cantidad);
                PlayerPersistence.instance.PagarDeuda(cantidad);

                ReproducirSonido(sonidoPagarDeuda);
                ActualizarUI();

                Debug.Log("Pagado $" + cantidad + " de deuda.");
            }
        }
    }

    public void ComprarBoost()
    {
        if (PlayerPersistence.instance != null)
        {
            if (PlayerPersistence.instance.CanAfford(costeBoost) && jugadasRestantesBoost == 0)
            {
                PlayerPersistence.instance.DeductCoins(costeBoost);
                jugadasRestantesBoost = duracionBoostEnJugadas;

                ReproducirSonido(sonidoBoostActivado);
                ActualizarUI();

                Debug.Log("¡Boost activado! Duración: " + duracionBoostEnJugadas + " jugadas.");
            }
        }
    }

    // Método público para que las máquinas tragaperras lo usen
    public float ObtenerMultiplicadorBoost()
    {
        if (jugadasRestantesBoost > 0)
        {
            return multiplicadorBoost;
        }
        return 1f; // Sin boost
    }

    // Método para que las máquinas tragaperras lo llamen después de cada jugada
    public void ConsumeBoostJugada()
    {
        if (jugadasRestantesBoost > 0)
        {
            jugadasRestantesBoost--;
            Debug.Log("Boost usado. Jugadas restantes: " + jugadasRestantesBoost);
            GuardarEstadoBoost();
        }
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void ReproducirFraseTrump()
    {
        if (sonidosTrumpHablando != null && sonidosTrumpHablando.Length > 0)
        {
            int indiceAleatorio = Random.Range(0, sonidosTrumpHablando.Length);
            ReproducirSonido(sonidosTrumpHablando[indiceAleatorio]);
        }
    }

    void CargarEstadoBoost()
    {
        if (PlayerPrefs.HasKey("BoostJugadasRestantes"))
        {
            jugadasRestantesBoost = PlayerPrefs.GetInt("BoostJugadasRestantes");
        }
    }

    void GuardarEstadoBoost()
    {
        PlayerPrefs.SetInt("BoostJugadasRestantes", jugadasRestantesBoost);
        PlayerPrefs.Save();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (promptHablarConTrump != null)
                promptHablarConTrump.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (promptHablarConTrump != null)
                promptHablarConTrump.SetActive(false);

            if (tiendaAbierta)
                Cerrar();
        }
    }
}