using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAbilities : MonoBehaviourPunCallbacks
{
    [Header("Apps Status")]
    public bool flashlightOn = true;
    public AK.Wwise.Event flashlightOnSound, flashlightOffSound;
    public GameObject thermalCamGO;
    public bool thermalVisionOn = false;
    public AK.Wwise.Event thermalOnSound, thermalOffSound;
    public GameObject nightVisionEffect;
    public bool nightVisionOn = false;
    public AK.Wwise.Event nightVisionOnSound, nightVisionOffSound;

    [PunRPC]
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

    [PunRPC]
    public void ToggleFlashlight(string appCode){
        if(!flashlightOn){
            flashlightOn = true;
            flashlightOnSound.Post(gameObject);
            GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneLight.SetActive(true);
        }else{
            flashlightOn = false;
            flashlightOffSound.Post(gameObject);
            GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().phoneLight.SetActive(false);
        }

        GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().drainBattery(flashlightOn, appCode); // Drain battery
    } // end ToggleFlashlight()

    [PunRPC]
    public void ToggleThermalVision(string appCode){
        thermalVisionOn = !thermalVisionOn;

        if(thermalVisionOn){
            thermalVisionOn = true;
            thermalCamGO.SetActive(true);
            thermalOnSound.Post(gameObject);

            if(nightVisionOn){
                ToggleNightVision("A005");
            }
        }else{
            thermalVisionOn = false;
            thermalCamGO.SetActive(false);
            thermalOffSound.Post(gameObject);
        }

        GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().drainBattery(thermalVisionOn, appCode); // Drain battery
    } // end ToggleThermalVision()

    [PunRPC]
    public void ToggleNightVision(string appCode){
        nightVisionOn = !nightVisionOn;
        
        if(nightVisionOn){
            nightVisionOn = true;
            nightVisionEffect.SetActive(true);
            nightVisionOnSound.Post(gameObject);

            if(thermalVisionOn){
                ToggleThermalVision("A004");
            }
        }else{
            nightVisionOn = false;
            nightVisionEffect.SetActive(false);
            nightVisionOffSound.Post(gameObject);
        }

        GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().drainBattery(nightVisionOn, appCode); // Drain battery
    } // end ToggleNightVision()


}
