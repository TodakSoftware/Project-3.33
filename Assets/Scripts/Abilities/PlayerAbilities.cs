using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerAbilities : MonoBehaviour
{
    PlayerMovement playerMovement;

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
    }

    // Flashlight
    void ToggleFlashlight(){
        
        if(Input.GetKeyDown(flashlightKey)){
            // Fullbody
            if(playerMovement.mobilePhone != null){
                if(playerMovement.mobilePhone.flashLight.activeSelf){
                    playerMovement.mobilePhone.flashLight.SetActive(false);
                }else{
                    playerMovement.mobilePhone.flashLight.SetActive(true);
                }
            }
            
            // Hand Only
            if(playerMovement.mobilePhone2 != null){
                if(playerMovement.mobilePhone2.flashLight.activeSelf){
                    playerMovement.mobilePhone2.flashLight.SetActive(false);
                }else{
                    playerMovement.mobilePhone2.flashLight.SetActive(true);
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
