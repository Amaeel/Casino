using UnityEngine;

public class AbuelaSpawner : MonoBehaviour
{
    [Header("Configuración del Generador")]
    public GameObject prefabAbuela; // Arrastra aquí el Prefab azul de la abuela
    public int limiteAbuelas = 15;
    public float radioDeAparicion = 80f; // Radio de la ciudad donde aparecerán
    public float frecuenciaSpawn = 4f;   // Cada cuántos segundos aparece una

    void Start()
    {
        // Llama a la función de crear cada X segundos de forma infinita
        InvokeRepeating("CrearAbuelaAleatoria", 2f, frecuenciaSpawn);
    }

    void CrearAbuelaAleatoria()
    {
        // Buscamos cuántas abuelas hay ahora mismo en la calle
        // Nota: Asegúrate de ponerle el Tag "Abuela" al Prefab de la abuela
        int contadorAbuelas = GameObject.FindGameObjectsWithTag("Abuela").Length;

        if (contadorAbuelas < limiteAbuelas)
        {
            // Calculamos una posición al azar en el mapa
            Vector3 posAleatoria = transform.position + new Vector3(
                Random.Range(-radioDeAparicion, radioDeAparicion),
                15f, // Aparece un poco alto para que el Raycast funcione bien
                Random.Range(-radioDeAparicion, radioDeAparicion)
            );

            // Lanzamos un rayo hacia abajo para encontrar el suelo exacto
            RaycastHit hit;
            if (Physics.Raycast(posAleatoria, Vector3.down, out hit, 50f))
            {
                // Comprobamos que el suelo sea plano o transitable
                Instantiate(prefabAbuela, hit.point + Vector3.up * 0.5f, Quaternion.identity);
            }
        }
    }
}