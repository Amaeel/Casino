using UnityEngine;

public class PuertaSalidaCasino : MonoBehaviour
{
    [Header("Configuración de Destino")]
    public string nombreEscenaCiudad = "SimplePoly City - Low Poly Assets";
    public Vector3 posicionFrenteAlCasino = new Vector3(25f, 0f, 15f);

    private void OnTriggerEnter(Collider other)
    {
        // Verificamos que sea el jugador
        if (other.CompareTag("Player") && PlayerPersistence.instance != null)
        {
            // Usamos la función de teletransporte que ya tienes
            PlayerPersistence.instance.LoadNewScene(nombreEscenaCiudad, posicionFrenteAlCasino);
        }
    }
}