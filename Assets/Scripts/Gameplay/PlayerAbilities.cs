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

    public void TerminateAllApps(){ // Terminate all apps if phone is dead
        if(GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneIsDead){
            if(flashlightOn){
                ToggleFlashlight("A001");
            }

            if(thermalVisionOn){
                ToggleThermalVision("A004");
            }

            if(nightVisionOn){
                ToggleNightVision("A005");
            }
        } // end phone is Dead
    } // end TerminateAllApps()

    public void ToggleFlashlight(string appCode){
        if(!flashlightOn){
            flashlightOn = true;
            GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneLight.SetActive(true);
        }else{
            flashlightOn = false;
            GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneLight.SetActive(false);
        }

        GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().drainBattery(flashlightOn, appCode); // Drain battery
    } // end ToggleFlashlight()

    public void ToggleThermalVision(string appCode){
        if(!thermalVisionOn){
            thermalVisionOn = true;
            //thermalCamGO.SetActive(true);
        }else{
            thermalVisionOn = false;
            //thermalCamGO.SetActive(false);
        }

        GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().drainBattery(thermalVisionOn, appCode); // Drain battery
    } // end ToggleThermalVision()

    public void ToggleNightVision(string appCode){
        if(!nightVisionOn){
            nightVisionOn = true;
            //nightCamGO.SetActive(true);
        }else{
            nightVisionOn = false;
            //nightCamGO.SetActive(false);
        }

        GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().drainBattery(nightVisionOn, appCode); // Drain battery
    } // end ToggleNightVision()


}
