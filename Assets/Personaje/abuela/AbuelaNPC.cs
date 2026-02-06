using UnityEngine;

public class AbuelaNPC : MonoBehaviour
{
    public float velocidadHuida = 4f;
    public float distanciaDeteccion = 8f;
    [Range(0, 100)] public int probabilidadSusto = 20;

    private Transform jugador;
    private bool haSidoInteractuada = false;
    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) jugador = p.transform;
    }

    void Update()
    {
        if (haSidoInteractuada || jugador == null || cc == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia < distanciaDeteccion)
        {
            Vector3 dir = (transform.position - jugador.position).normalized;
            dir.y = 0;
            // Movemos con el CharacterController propio de la abuela
            cc.Move(dir * velocidadHuida * Time.deltaTime);
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 0.2f);
        }
    }

    // Usamos OnTriggerEnter porque la abuela es un Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Forzamos la detección por Tag
        if (other.CompareTag("Player") && !haSidoInteractuada)
        {
            Debug.Log("¡ELVIS DETECTADO!");
            ResolverEncuentro();
        }
    }

    void ResolverEncuentro()
    {
        haSidoInteractuada = true;
        if (Random.Range(0, 100) < probabilidadSusto)
        {
            EjecutarSusto();
        }
        else
        {
            PlayerPersistence.instance.AddCoins(Random.Range(100, 300));
        }
        Destroy(gameObject, 0.5f);
    }

    void EjecutarSusto()
    {
        GameObject susto = GameObject.Find("AbuelaSusto");
        if (susto != null)
        {
            susto.GetComponent<Canvas>().enabled = true;
            AudioSource audio = susto.GetComponent<AudioSource>();
            if (audio != null) audio.Play();
            Invoke("OcultarSusto", 0.5f);
        }
        PlayerPersistence.instance.DeductCoins((int)(PlayerPersistence.instance.GetCoins() * 0.4f));
    }

    void OcultarSusto()
    {
        GameObject susto = GameObject.Find("AbuelaSusto");
        if (susto != null) susto.GetComponent<Canvas>().enabled = false;
    }
}