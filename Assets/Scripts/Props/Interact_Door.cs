using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class Interact_Door : MonoBehaviourPunCallbacks
{
    List<GameObject> capturedHumanList = new List<GameObject>();
    public bool openOutward;
    bool isOpened;
    public bool offline;
    // Button Related
    E_ButtonType originalBtnType;
    float originalHoldDuration;


    public void OpenDoor(){
        if(!offline){
            photonView.RPC("HandleDoor", RpcTarget.All);
        }else{
            HandleDoor();
        }
    }

    [PunRPC]
    void HandleDoor(){
        if(!isOpened){
            if(!openOutward){
                transform.DORotateQuaternion(new Quaternion(-0.5f,-0.5f,-0.5f,0.5f), 1f).OnComplete(() => { isOpened = true; } );
            }else{
                transform.DORotateQuaternion(new Quaternion(-0.5f,0.5f,0.5f,0.5f), 1f).OnComplete(() => { isOpened = true; } );
            }

            if(capturedHumanList.Count > 0){
                // Set captured human to Uncaptured
                foreach(var human in capturedHumanList){
                    human.GetComponent<Human>().photonView.RPC("Released", human.GetPhotonView().Owner);
                }

                GetComponent<Interactable>().buttonType = originalBtnType;
                GetComponent<Interactable>().holdDuration = originalHoldDuration;
            }
        }else{
            transform.DORotateQuaternion(new Quaternion(-0.707106829f,0,0,0.707106829f), 1f).OnComplete(() => { isOpened = false; } );
        }
    }

    [PunRPC]
    public void HumanInsideRoom(int playerViewID){
        var player = PhotonView.Find(playerViewID).gameObject;
        capturedHumanList.Add(player);
        print("Added " + player + " inside prison");

        // Store original value
        originalBtnType = GetComponent<Interactable>().buttonType;
        originalHoldDuration = GetComponent<Interactable>().holdDuration;

        // Change to new value
        GetComponent<Interactable>().buttonType = E_ButtonType.HOLD;
        GetComponent<Interactable>().holdDuration = 3f;
        if(isOpened){
            isOpened = false;
            transform.DORotateQuaternion(new Quaternion(-0.707106829f,0,0,0.707106829f), 1f);
        }
    }

}
