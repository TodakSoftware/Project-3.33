using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Interactable))]
public class RitualItem : MonoBehaviourPunCallbacks
{
    
    [Header("Item Info")]
    public SO_RitualItem ritualItemSO;
    public string code;

    public void Start(){
        foreach(var item in ritualItemSO.ritualItemLists){
            if(item.code == code){
                // Setup interaction first
                if(GetComponent<Interactable>() != null){
                    GetComponent<Interactable>().interactType = item.interactType;
                    GetComponent<Interactable>().buttonType = item.buttonType;
                    GetComponent<Interactable>().holdDuration = item.holdDuration;
                } // end GetCOmponent Interactable

                gameObject.name = item.displayName; // set item name
            } // end if(item.code == code)
        } // end foreach
    }// end Start

    [PunRPC]
    void DestroyItem(){
        if(photonView.IsMine){
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
