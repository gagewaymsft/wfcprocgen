using UnityEngine;

/// <summary>
/// Used to switch from the brighter and easier to edit in global light to the <br/>
/// darker and moodier play light.
/// Develop how it's easy, test how it would be actually used.<br />
/// This doesn't flip the checkmark in the editor and throws a "no two global lights" exception. <br />
/// Ignore this. It works as intended.
/// </summary>
public class GlobalLightswitch : MonoBehaviour
{
    public bool Lightswitch;

    private readonly int DevGlobalLightChildIndex = 0;
    private readonly int GameGlobalLightChildIndex = 1;

    private UnityEngine.Rendering.Universal.Light2D DevGlobalLight;
    private UnityEngine.Rendering.Universal.Light2D GameGlobalLight;

    void Start()
    {
        DevGlobalLight = transform
            .GetChild(DevGlobalLightChildIndex)
            .GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        GameGlobalLight = transform
            .GetChild(GameGlobalLightChildIndex)
            .GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        
        SwitchLights();
        Lightswitch = false;
    }

    public void SwitchLights()
    {
        Lightswitch = !Lightswitch;
        DevGlobalLight.enabled = !DevGlobalLight.enabled;
        GameGlobalLight.enabled = !GameGlobalLight.enabled;
    }

    private void Update()
    {
        if (Lightswitch)
        {
            SwitchLights();
        }
    }
}
