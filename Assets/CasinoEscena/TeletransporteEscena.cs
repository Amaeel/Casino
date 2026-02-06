using UnityEngine;
using UnityEngine.SceneManagement;

public class TeletransporteEscena : MonoBehaviour, IInteractable
{

    [Header("Configuración de Viaje")]
    [Tooltip("Escribe el nombre exacto de la escena a cargar (Casino)")]
    public string nombreEscenaDestino;

    public void Interact()
    {
        Debug.Log("Viajando a " +  nombreEscenaDestino);
        // if (GameManager.instance != null) GameManager.instance.saveData();

        // Verificar si la escena es válida
        if (Application.CanStreamedLevelBeLoaded(nombreEscenaDestino))
        {

            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else {
            Debug.LogError("¡Error! La escena '" +  nombreEscenaDestino + "' no existe.");
        }
    }
}
