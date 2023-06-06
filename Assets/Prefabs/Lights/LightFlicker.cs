using System.Collections;
using UnityEngine;


public class LightFlicker : MonoBehaviour
{
    Transform mainLight;
    Transform flickerLight;
    UnityEngine.Rendering.Universal.Light2D mainLightComponent;
    UnityEngine.Rendering.Universal.Light2D flickerLightComponent;


    // Start is called before the first frame update
    private void Start()
    {
        mainLight = transform.GetChild(0);
        flickerLight = transform.GetChild(1);
        mainLightComponent = mainLight.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        flickerLightComponent = flickerLight.GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        for (; ; ) //this is while(true), but doesn't lock the thread like while(true) would
        {
            float randomIntensity = Random.Range(1.5f, 3.5f);
            flickerLightComponent.intensity = randomIntensity;

            float randomTime = Random.Range(0f, 0.1f);
            yield return new WaitForSeconds(randomTime);
        }
    }
}
