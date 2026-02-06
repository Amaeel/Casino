using UnityEngine;

// A LED node emitting light based on start/stop signals from the previous node in the chain.
public class LEDNode : MonoBehaviour
{
    // Previous linked LED node.
    public LEDNode prevNode = null;
    // Start/stop signals for the next linked LED node.
    public bool nextStart { get; private set; } = false;

    // Mark in GUI if first node in the chain.
    public bool isFirstNode = false;
    // Point light enable control.
    public bool isPointLightEn = false;
    // Slow light progress indicator.
    public bool slowProgress = false;


    // Attached components.
    private Light pointLight = null;
    private Renderer rend = null;

    // Emitted light intensity parameters.
    private float intensity = 0;
    private float minIntensity = 0;
    private float maxIntensity = 1.0f;
    private float onTime = 0;
    private float maxOnTime = 1.0f;
    private float onTimerSpeed = 2.0f;
    private float offTime = 0;
    private float maxOffTime = 20.0f;
    private float offTimerSpeed = 20.0f;
    // Emitted light increase/decrease controls.
    private enum LightState { INCR, DECR, IDLE }
    private LightState lightState = LightState.IDLE;

    void Start()
    {
        // Init the attached components.
        pointLight = this.GetComponent<Light>();
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        ResolveNodeState();
        UpdateColor();
    }

    // Decides on state of fading in/out based on the input parameters.
    private void ResolveNodeState()
    {
        switch (lightState)
        {
            case LightState.INCR:
                IntensityIncrease();
                break;
            case LightState.DECR:
                IntensityDecrease();
                break;
            case LightState.IDLE:
                LightIdle();
                break;
            default:
                break;
        }
    }

    // Gradually increase intensity level up to the maximum-level defined in GUI.
    private void IntensityIncrease()
    {
        if (!slowProgress)
        {
            if (nextStart == false)
                nextStart = true;
        }

        intensity = maxIntensity;

        // If point light is enabled, light it up.
        if (isPointLightEn)
            pointLight.enabled = true;

        // If ON timer handn't been reached yet
        if (onTime < maxOnTime)
            onTime += onTimerSpeed * Time.deltaTime;
        // ON timer had been reached.
        else
        {
            onTime = 0;
            // Start decreasing intensity.
            lightState = LightState.DECR;
        }
    }

    // Gradually decrease intensity level down to 0.
    private void IntensityDecrease()
    {
        intensity = minIntensity;

        // If there's a point light component enabled, disable the point light.
        if (isPointLightEn)
            pointLight.enabled = false;

        if (slowProgress)
        {
            if (nextStart == false)
                nextStart = true;
        }
        else
        {
            // Stop sending the start signal to the next node.
            if (nextStart == true)
                nextStart = false;
        }

        // Move to the idle (no light) state.
        lightState = LightState.IDLE;
    }

    // Light idles until signaled to do otherwise.
    private void LightIdle()
    {
        if (slowProgress)
        {
            if (nextStart == true)
                nextStart = false;
        }

        // If there is a previous linked node
        if (prevNode != null)
        {
            if (prevNode.nextStart)
            {
                // Start increasing light intensity.
                lightState = LightState.INCR;
            }
        }
        // If first node in the chain
        else if (isFirstNode)
        {
            // If max Idle time wasn't reached yet
            if (offTime < maxOffTime)
                offTime += offTimerSpeed * Time.deltaTime;
            else
            {
                offTime = 0;
                // Start increasing light intensity.
                lightState = LightState.INCR;
                // Light-up the next node in chain.
                if (nextStart == false)
                    nextStart = true;
            }
        }
    }

    // Updates the calculated LED color and intensity level.
    private void UpdateColor()
    {
        Material mat = rend.material;
        Color baseColor;

        // CORRECCIÓN: Intentamos obtener el color de la propiedad de Emisión.
        // El error indica que el shader no tiene la propiedad "_Color" (que es el atajo 'mat.color').
        // Asumimos que el color base del LED se define en su propiedad de Emisión.
        if (mat.HasProperty("_EmissionColor"))
        {
            baseColor = mat.GetColor("_EmissionColor");
        }
        else
        {
            // Fallback: Si no tiene _EmissionColor, intentamos con _BaseColor (común en URP/HDRP) 
            // o _Color, pero si falla, usamos blanco para que no rompa.
            if (mat.HasProperty("_BaseColor"))
            {
                baseColor = mat.GetColor("_BaseColor");
            }
            else if (mat.HasProperty("_Color"))
            {
                baseColor = mat.GetColor("_Color");
            }
            else
            {
                baseColor = Color.white;
            }
        }

        // Calculate the resulting color based on the intensity.
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(intensity);

        // Establecemos el color de emisión con la nueva intensidad
        mat.SetColor("_EmissionColor", finalColor);
    }
}