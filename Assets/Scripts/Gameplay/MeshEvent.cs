using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeshEvent : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject meshOwner;

    void Start(){
        if(meshOwner == null){
            meshOwner = gameObject.transform.parent.gameObject;
        }
    }
    public void SwapPhonePosition(){ // ANIMATION HUMAN INTERACT PHONE (FRAME 42)
        // tell player to swipe phone position
        meshOwner.GetComponent<Human>().SwitchPhonePosition();
    }

    public void NotifyInteractEnd(){ // ANIMATION HUMAN INTERACT PHONE (FRAME END)
        meshOwner.GetComponent<Human>().interactAnimEnd = true;
    }

    public void NotifyInteractReset(){ // ANIMATION HUMAN INTERACT PHONE (FRAME START)
        meshOwner.GetComponent<Human>().interactAnimEnd = false;
    }

    public void NotifyGhostCaughtColliderStart(){
        meshOwner.GetComponent<Ghost>().EnableCaughtCollider();
    }

    public void NotifyGhostCaughtColliderEnd(){
        meshOwner.GetComponent<Ghost>().DisableCaughtCollider();
    }
}
