using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerAbilities : MonoBehaviour
{
    public static PlayerAbilities instance;
    PlayerMovement playerMvmt;
    [Title("Flashlight")]
    public KeyCode flashlightKey;
    public bool flashlightOn = true;

    [Title("Thermal Vision")]
    //public KeyCode thermalVisionKey;
    public GameObject thermalCam;
    public bool thermalOn;

    [Title("Night Vision")]
    //public KeyCode nightVisionKey;
    public GameObject nightCam;
    public bool nightOn;

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }

        playerMvmt = GetComponent<PlayerMovement>();
    }

    void Update(){
        if(Input.GetKeyDown(flashlightKey)){
            ToggleFlashlight();
        }
        //ToggleThermalVision();
        //ToggleNightVision();
    }

    // Flashlight
    public void ToggleFlashlight(){
        if(playerMvmt.mobilePhone != null){
            if(flashlightOn){
                playerMvmt.mobilePhone.flashLight.SetActive(false);
                MobilePhone.instance.drainBattery(false, "A001");
                flashlightOn = false;
            }else{
                playerMvmt.mobilePhone.flashLight.SetActive(true);
                MobilePhone.instance.drainBattery(true, "A001");
                flashlightOn = true;
            }
        }
    }

    // Thermal Vision
    public void ToggleThermalVision(){
        if(thermalOn){
            thermalCam.SetActive(false);
            thermalOn = false;
        }else{
            thermalCam.SetActive(true);
            thermalOn = true;

            if(nightOn){
                nightCam.SetActive(false);
                nightOn = false;
            }
        }
    }

    // Night Vision
    public void ToggleNightVision(){
        if(nightOn){
            nightCam.SetActive(false);
            nightOn = false;
        }else{
            nightCam.SetActive(true);
            nightOn = true;

            if(thermalOn){
                thermalCam.SetActive(false);
                thermalOn = false;
            }
        }
    }
}
