using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class MeshProperty : MonoBehaviour
{
    public GameObject itemHandlerGO;  // Right
    public GameObject itemHandlerGO2; // left
    [HideInInspector] public GameObject toLookAt;
    bool hpOnLeft, zoomIn;
    
    [Title("Procedural Animations")]
    public bool enableProcedural = true;
    public GameObject chestTarget;


    void Update(){
        if(enableProcedural){
            if(toLookAt != null){
                chestTarget.transform.position = new Vector3(chestTarget.transform.position.x, toLookAt.transform.position.y,chestTarget.transform.position.z);
            }
        }
    }

    public void SwitchPhoneView(){
        if(!hpOnLeft){
            PlayerMovement.instance.SwitchPhoneLandscape(true);
            hpOnLeft = true;
        }else{
            PlayerMovement.instance.SwitchPhoneLandscape(false);
            hpOnLeft = false;
        }
    }

    public void CameraZoomIn(){
        if(!zoomIn){
            zoomIn = true;
            PlayerMovement.instance.PhoneZoomIn(true);
        }else{
            zoomIn = false;
            PlayerMovement.instance.PhoneZoomIn(false);
        }
    }
}
