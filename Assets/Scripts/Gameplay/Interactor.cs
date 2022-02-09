using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Interactor : MonoBehaviourPunCallbacks
{
    public Camera cam;
    public LayerMask interactableLayerMask;
    public Ritual_Item interactable;
    private bool isHighlight;

    [Title("Others")]
    public GameObject crosshair;
    public TextMeshProUGUI pickupText;

    void Start(){
        if(photonView.IsMine){
            crosshair = GameObject.Find("Crosshair");
            if(crosshair.gameObject != null){
                pickupText = crosshair.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine){
        if(cam != null){
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.5f, interactableLayerMask)){
                //print(hit.collider.name);
                
                    if(hit.collider.GetComponent<Ritual_Item>() != false){
                        if(interactable == null){
                            interactable = hit.collider.GetComponent<Ritual_Item>();
                            crosshair.GetComponent<Image>().color = Color.red;
                            isHighlight = true;
                            pickupText.text = "[E] Pickup "+ interactable.itemName;
                        }

                        if(Input.GetKeyDown(KeyCode.E)){
                            // Interact
                            if(PlayerManager.instance.itemSlot == ""){
                                interactable.onInteract.Invoke();
                            }else{
                                print("Item Slot full");
                            }
                        }
                    }

            }else{
                if(isHighlight){
                    isHighlight = false;
                    crosshair.GetComponent<Image>().color = Color.white;
                    if(interactable != null){
                        interactable = null;
                    }
                    pickupText.text = "";
                }
            } // end raycast
        } // end cam != null
    } // end ismine
    }
}
