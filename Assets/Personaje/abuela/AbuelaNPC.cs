using UnityEngine;

public class AbuelaNPC : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadPaseo = 1.5f;
    public float velocidadHuida = 5f;
    public float distanciaDeteccion = 8f;

    private Transform jugador;
    private bool haSidoInteractuada = false;
    private Vector3 direccionPaseo;
    private float cronometroCambio;

    private CharacterController controller;
    private bool puedoRobar = false;
    private GameObject visualPrompt;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;

        visualPrompt = GameObject.Find("TextoRobar");
        if (visualPrompt != null) visualPrompt.SetActive(false);

        ElegirNuevaDireccion();
    }

    void Update()
    {
        if (haSidoInteractuada || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        // Movimiento con CharacterController (Detecta colisiones con edificios)
        if (distancia < distanciaDeteccion)
            Mover((transform.position - jugador.position).normalized, velocidadHuida);
        else
            Pasear();

        // Lógica de robo con tecla E
        if (puedoRobar && Input.GetKeyDown(KeyCode.E))
        {
            IniciarRobo();
        }
    }

    void Pasear()
    {
        cronometroCambio += Time.deltaTime;
        if (cronometroCambio >= 3f) ElegirNuevaDireccion();
        Mover(direccionPaseo, velocidadPaseo);
    }

    void Mover(Vector3 dir, float vel)
    {
        if (controller != null && controller.enabled)
        {
            // Aplicamos gravedad para que no floten
            Vector3 movimiento = dir * vel;
            movimiento.y = -9.81f;

            controller.Move(movimiento * Time.deltaTime);

            // Rotación suave
            Vector3 dirMirar = new Vector3(dir.x, 0, dir.z);
            if (dirMirar != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirMirar), 0.15f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            puedoRobar = true;
            if (visualPrompt != null) visualPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            puedoRobar = false;
            if (visualPrompt != null) visualPrompt.SetActive(false);
        }
    }

    void IniciarRobo()
    {
        haSidoInteractuada = true; // Bloquea más robos a esta abuela
        if (visualPrompt != null) visualPrompt.SetActive(false);

        int probabilidad = Random.Range(0, 100);

        if (probabilidad < 30) // 30% de probabilidad de susto
        {
            EjecutarSusto();
        }
        else
        {
            PlayerPersistence.instance.AddCoins(Random.Range(100, 301));
            FinalizarAbuela();
        }
    }

    void EjecutarSusto()
    {
        GameObject sustoObj = GameObject.Find("AbuelaSusto");
        if (sustoObj != null)
        {
            // Activamos el Canvas del susto
            Canvas canvasSusto = sustoObj.GetComponent<Canvas>();
            if (canvasSusto != null) canvasSusto.enabled = true;

            // Reproducimos el sonido
            AudioSource audio = sustoObj.GetComponent<AudioSource>();
            if (audio != null) audio.Play();

            // Quitamos el 40% del dinero
            long perdida = (long)(PlayerPersistence.instance.GetCoins() * 0.4f);
            PlayerPersistence.instance.DeductCoins((int)perdida);

            // Programamos el cierre del susto y destruir la abuela después
            Invoke("OcultarSustoYMorir", 0.7f);
        }
        else
        {
            Debug.LogError("No se encontró el objeto AbuelaSusto en la escena.");
            FinalizarAbuela();
        }
    }

    void OcultarSustoYMorir()
    {
        GameObject sustoObj = GameObject.Find("AbuelaSusto");
        if (sustoObj != null) sustoObj.GetComponent<Canvas>().enabled = false;
        FinalizarAbuela();
    }

    void FinalizarAbuela()
    {
        Destroy(gameObject);
    }

    void ElegirNuevaDireccion()
    {
        float angulo = Random.Range(0f, 360f);
        direccionPaseo = new Vector3(Mathf.Sin(angulo), 0, Mathf.Cos(angulo)).normalized;
        cronometroCambio = 0;
    }
}