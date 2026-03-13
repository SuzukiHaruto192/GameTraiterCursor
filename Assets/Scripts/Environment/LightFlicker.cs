using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D myLight;
    void Start() => myLight = GetComponent<Light2D>();

    [SerializeField] float minIntensity = 1.8f;
    [SerializeField] float maxIntensity = 2.6f;

    void Update()
    {
        myLight.intensity = Random.Range(minIntensity, maxIntensity);
    }
}
