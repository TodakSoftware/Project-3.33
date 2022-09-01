using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

[RequireComponent(typeof(Interactable))]
public class RitualItem : MonoBehaviourPunCallbacks
{
    
    [Header("Item Info")]
    public SO_RitualItem ritualItemSO;
    public string code;
    [Header("Material Highlight")]
    public List<MeshRenderer> meshRendererList = new List<MeshRenderer>();
    List<Material> materialList = new List<Material>();
    bool isHighlighted;

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

        if(meshRendererList.Count > 0){
            foreach(var mat in meshRendererList){
                materialList.Add(mat.sharedMaterial);
            }
        }
    }// end Start

    void Update(){
        if(Input.GetKeyDown(KeyCode.H)){
            HighlightItem();
        }
    }

    [PunRPC]
    void DestroyItem(){
        if(photonView.IsMine){
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void HighlightItem(){
        foreach(var a in materialList){
            print(a.color);
            
            //a.SetColor("_EmissiveColor", Color.white * 12f);
            //a.DOFloat(12,"_EmissiveColor",1f);
            a.DOVector(Color.white * 12f, "_EmissiveColor", 1f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnApplicationQuit() {
        foreach(var a in materialList){
            print(a.color);
            
            a.SetColor("_EmissiveColor", Color.white * 1f);
        }
    }
}
