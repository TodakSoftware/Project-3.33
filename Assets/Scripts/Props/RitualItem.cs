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
    public string itemName;
    [Header("Material Highlight")]
    public List<MeshRenderer> meshRendererList = new List<MeshRenderer>();
    List<Material> materialList = new List<Material>();
    public bool isHighlighted;

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
                itemName = item.displayName;
                UI_TaskMain.instance.GenerateTask(itemName);

            } // end if(item.code == code)
        } // end foreach

        if(meshRendererList.Count > 0){
            foreach(var mat in meshRendererList){
                materialList.Add(mat.sharedMaterial);
            }
        }

        HighlightItem();
    }// end Start

    void Update(){
        /* if(Input.GetKeyDown(KeyCode.H)){
            HighlightItem();
        } */
    }

    [PunRPC]
    void DestroyItem(){
        if(photonView.IsMine){
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void HighlightItem(){
        if(!isHighlighted){
            foreach(var a in materialList){
                a.DOVector(Color.white * 12f, "_EmissiveColor", .8f).SetLoops(-1, LoopType.Yoyo);
            }
            isHighlighted = true;
            print("Item Highlighted");
        }
    }

    public void UnhighlightItem(){
        if(isHighlighted){
            DOTween.KillAll();
            foreach(var a in materialList){
                a.SetColor("_EmissiveColor", Color.white * .5f);
            }
            isHighlighted = false;
            print("Item Unhighlighted");
        }
    }

    private void OnApplicationQuit() {
        UnhighlightItem();
    }
}
