using System;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    public class ParticleSystemMultiplier : MonoBehaviour
    {
        // a simple script to scale the size, speed and lifetime of a particle system

        public float multiplier = 1;


        private void Start()
        {
            var systems = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem system in systems)
            {
                // Accedemos al módulo principal (main) para modificar las propiedades
                var mainModule = system.main;

                // CORRECCIÓN: 'startSize' ahora es 'main.startSizeMultiplier'
                mainModule.startSizeMultiplier *= multiplier;

                // CORRECCIÓN: 'startSpeed' ahora es 'main.startSpeedMultiplier'
                mainModule.startSpeedMultiplier *= multiplier;

                // CORRECCIÓN: 'startLifetime' ahora es 'main.startLifetimeMultiplier'
                mainModule.startLifetimeMultiplier *= Mathf.Lerp(multiplier, 1, 0.5f);

                // Estas funciones no cambiaron
                system.Clear();
                system.Play();
            }
        }
    }
}