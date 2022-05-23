using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEvent : MonoBehaviour
{
    [SerializeField] GameObject meshOwner;

    void Start(){
        if(meshOwner == null){
            meshOwner = gameObject.transform.parent.gameObject;
        }
    }
    public void SwapPhonePosition(){ // ANIMATION HUMAN INTERACT PHONE (FRAME 42)
        // tell player to swipe phone position
        meshOwner.GetComponent<PlayerController>().SwitchPhonePosition();
    }

    public void NotifyInteractEnd(){ // ANIMATION HUMAN INTERACT PHONE (FRAME END)
        meshOwner.GetComponent<PlayerController>().interactAnimEnd = true;
    }

    public void NotifyInteractReset(){ // ANIMATION HUMAN INTERACT PHONE (FRAME START)
        meshOwner.GetComponent<PlayerController>().interactAnimEnd = false;
    }
}
