using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // Esta línea ya incluye el namespace UnityEngine.EventSystems

public class EventSystemChecker : MonoBehaviour
{
    //public GameObject eventSystem;

    // Use this for initialization
    void Awake()
    {

        if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");

            // Simplificamos la adición del componente EventSystem
            es.AddComponent<EventSystem>();

            es.AddComponent<StandaloneInputModule>();
        }
    }
}