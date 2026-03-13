using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    private Light2D myLight;
    void Start() => myLight = GetComponent<Light2D>();

    void Update()
    {
        // Làm ánh sáng nhấp nháy nhẹ nhàng
        myLight.intensity = Mathf.PingPong(Time.time * 2f, 2.8f) + 0.2f;
    }
}
