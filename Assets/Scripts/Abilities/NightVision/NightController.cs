using UnityEngine;
using UnityEngine.Rendering;

public class NightController : MonoBehaviour
{
    private float defaultAmbientVal;
    [SerializeField] float highAmbientVal = 8f;
    Volume volume;


    void OnEnable(){
        if(volume == null){
            volume = GetComponent<Volume>();
        }
        defaultAmbientVal = RenderSettings.ambientIntensity;
        EnableHighAmbient();
    }

    void OnDisable(){
        ResetHighAmbient();
    }

    private void EnableHighAmbient()
    {
        RenderSettings.ambientIntensity = highAmbientVal;
    }

    private void ResetHighAmbient()
    {
        RenderSettings.ambientIntensity = defaultAmbientVal;
    }
}