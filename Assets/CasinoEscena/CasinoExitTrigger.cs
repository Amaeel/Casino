using UnityEngine;
using UnityEngine.SceneManagement;

public class CasinoExitTrigger : MonoBehaviour
{
    public string nombreDeLaCiudad = "SimplePoly City - Low Poly Assets __ Demo Scene";


    private bool jugadorCerca = false;

    // Cuando el jugador (Tag "Player") entra en el cubo invisible
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Pulsa E para volver a la ciudad.");
            // mostrar aquí un mensaje "Pulsa E para salir"
        }
    }

    // Cuando el jugador sale del cubo invisible
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    // Comprobar la tecla de acción
    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            // Carga la escena de la ciudad principal
            SceneManager.LoadScene(2);
        }
    }
}