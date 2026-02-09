using UnityEngine;

public class AbuelaSpawner : MonoBehaviour
{
    public GameObject prefabAbuela;
    public int limiteAbuelas = 15;
    public float radioDeAparicion = 80f;
    public float frecuenciaSpawn = 4f;

    void Start()
    {
        InvokeRepeating("CrearAbuelaAleatoria", 2f, frecuenciaSpawn);
    }

    void CrearAbuelaAleatoria()
    {
        int contadorAbuelas = GameObject.FindGameObjectsWithTag("Abuela").Length;

        if (contadorAbuelas < limiteAbuelas)
        {
            // Simplemente calculamos la posición y la instanciamos
            // La Y la ponemos a una altura fija donde sepas que hay suelo (ej: 0.5f)
            Vector3 posAleatoria = transform.position + new Vector3(
                Random.Range(-radioDeAparicion, radioDeAparicion),
                0.5f,
                Random.Range(-radioDeAparicion, radioDeAparicion)
            );

            Instantiate(prefabAbuela, posAleatoria, Quaternion.identity);
        }
    }
}