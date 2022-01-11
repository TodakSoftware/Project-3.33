using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerAbilities : MonoBehaviour
{
    PlayerMovement playerMovement;

    [Title("VHS Screen")]
    public KeyCode vhsKey;
    public GameObject vhsCam;

    [Title("Flashlight")]
    public KeyCode flashlightKey;

    [Title("Thermal Vision")]
    public KeyCode thermalVisionKey;
    public GameObject thermalCam;
    public bool thermalOn;

    [Title("Night Vision")]
    public KeyCode nightVisionKey;
    public GameObject nightCam;
    public bool nightOn;

    void Start(){
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update(){
        ToggleFlashlight();
        ToggleThermalVision();
        ToggleNightVision();
        ToggleVHSScreen();
    }

    // VHS Screen
    void ToggleVHSScreen(){
        if(Input.GetKeyDown(vhsKey)){
            if(vhsCam.activeSelf){
                vhsCam.SetActive(false);
            }else{
                vhsCam.SetActive(true);
            }
        }
    }

    // Flashlight
    void ToggleFlashlight(){
        
        if(Input.GetKeyDown(flashlightKey)){
            if(playerMovement.mobilePhone != null){
                if(playerMovement.mobilePhone.flashLight.activeSelf){
                    playerMovement.mobilePhone.flashLight.SetActive(false);
                }else{
                    playerMovement.mobilePhone.flashLight.SetActive(true);
                }
            }
        }
    }

    // Thermal Vision
    void ToggleThermalVision(){
        if(Input.GetKeyDown(thermalVisionKey)){
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
    }

    // Night Vision
    void ToggleNightVision(){
        if(Input.GetKeyDown(nightVisionKey)){
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
}
