using UnityEngine;
using UnityEngine.Rendering;

public class NightController : MonoBehaviour
{
    public Color defaultLightColor;
    public Color boostedLightColor;
    Volume volume;
    bool isEnabled;

    void Start(){
        if(volume == null){
            volume = GetComponent<Volume>();
            volume.weight = 0;

            RenderSettings.ambientLight = defaultLightColor;
        }
    }

}