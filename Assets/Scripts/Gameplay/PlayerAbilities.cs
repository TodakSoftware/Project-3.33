using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [Header("Apps Status")]
    public bool flashlightOn = true;
    public GameObject thermalCamGO;
    public bool thermalVisionOn = false;
    public GameObject nightCamGO;
    public bool nightVisionOn = false;

    public void ToggleFlashlight(){
        if(!flashlightOn){
            flashlightOn = true;
            GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneLight.SetActive(true);
        }else{
            flashlightOn = false;
            GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneLight.SetActive(false);
        }
    } // end ToggleFlashlight()

    public void ToggleThermalVision(){
        if(!thermalVisionOn){
            thermalVisionOn = true;
            thermalCamGO.SetActive(true);
        }else{
            thermalVisionOn = false;
            thermalCamGO.SetActive(false);
        }
    } // end ToggleThermalVision()

    public void ToggleNightVision(){
        if(!nightVisionOn){
            nightVisionOn = true;
            nightCamGO.SetActive(true);
        }else{
            nightVisionOn = false;
            nightCamGO.SetActive(false);
        }
    } // end ToggleNightVision()


}
