using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInteraction : MonoBehaviourPunCallbacks
{
    public bool enableInteract = true;
    public LayerMask interactableLayerMask;
    public Transform cam;
    public float interactMaxDistance;
    public GameObject interactionUI;
    public Image keyImage;
    public TextMeshProUGUI keyCodeText;
    public TextMeshProUGUI interactText;
    Interactable interactable;
    
    [Header("Hold Related")]
    float holdDur, holdTimer;
    public Image holdSliderImage;

    void Update(){
        if(enableInteract){
            if(interactable != null){

                if(interactable.GetComponent<Interactable>().buttonType == E_ButtonType.HOLD){
                    if(Input.GetButton("Interact")){
                        if(holdTimer < holdDur){
                            holdTimer += Time.deltaTime;
                            holdSliderImage.fillAmount = holdTimer / holdDur;
                        }else{
                            holdTimer = holdDur;
                            // Added to inventory
                            if(interactable.onInteract != null){
                                photonView.RPC("AddRitualToInventory", RpcTarget.All);
                                photonView.RPC("PutRitualOnAltar", RpcTarget.All);

                                interactable.onInteract.Invoke();
                                ClearInteraction();
                            }else{
                                print("Cannot proceed");
                            } // end if ritualitem != null
                        }
                    }else{
                        holdTimer = 0;
                        holdSliderImage.fillAmount = 0;
                    } // end input getbutton
                }else{
                    if(Input.GetButtonDown("Interact")){
                        // Added to inventory
                        if(interactable.onInteract != null){
                            photonView.RPC("AddRitualToInventory", RpcTarget.All);
                            photonView.RPC("PutRitualOnAltar", RpcTarget.All);

                            interactable.onInteract.Invoke();
                            ClearInteraction();
                        }else{
                            print("Cannot proceed");
                        }

                        
                        
                    } // end input getbutton
                }

            } // end interacble != null

        } // end enableInteract
    } // end Update()

    [PunRPC]
    void AddRitualToInventory(){
        if(interactable != null && interactable.CompareTag("RitualItem") && !GetComponent<PlayerInventory>().IsInventoryFull()){
            print("Added ritual item " + interactable.GetComponent<RitualItem>().code);
            GetComponent<PlayerInventory>().photonView.RPC("AddRitualItem", RpcTarget.All, interactable.GetComponent<RitualItem>().code);
            interactable.GetComponent<RitualItem>().photonView.RPC("DestroyItem", RpcTarget.All); // Destroy item
        }
    }

    [PunRPC]
    void PutRitualOnAltar(){
        if(interactable != null && interactable.CompareTag("Altar") && GetComponent<PlayerInventory>().ritualItemLists.Count > 0){
            foreach(var item in GetComponent<PlayerInventory>().ritualItemLists){
                if(interactable.GetComponent<Altar>().itemLists.Count < 5){
                    interactable.GetComponent<Altar>().photonView.RPC("addRitualItems", RpcTarget.All, item);
                }
            } // end foreach

            GetComponent<PlayerInventory>().photonView.RPC("ClearRitualItems", RpcTarget.All);
            interactionUI.SetActive(false);
        }
    }

    void FixedUpdate(){
        if(enableInteract){
            RaycastHit hit;
        
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactMaxDistance, interactableLayerMask)){
                if(hit.collider.GetComponent<Interactable>() != false){
                    
                    if(interactable == null || interactable.id != hit.collider.GetComponent<Interactable>().id){
                        if(gameObject.CompareTag("Ghost") && hit.collider.gameObject.GetComponent<Interact_Door>().humanInside){ // if we are ghost & hitted object is door with human inside, do nothing
                            print("Cannot open, human inside");
                        }else{
                            interactable = hit.collider.GetComponent<Interactable>();

                            holdDur = hit.collider.GetComponent<Interactable>().holdDuration;

                            // UI Related
                            if(!interactionUI.activeSelf){
                                interactionUI.SetActive(true);
                            }
                            PopupInteractInfo();

                            // Altar Interaction start
                            if(interactable.CompareTag("Altar") && GetComponent<PlayerInventory>().ritualItemLists.Count <= 0 && photonView.IsMine){
                                // UI Related
                                if(interactionUI.activeSelf){
                                    interactionUI.SetActive(false);
                                }
                            }// Altar Interaction end
                        }
                    } // end interactable null
                }
            }else{
                interactable = null;
                ClearInteraction();
            }
        } // end enableInteract
    } // end update()

    public void ClearInteraction(){
        if(interactable == null && interactionUI.activeSelf){
            holdTimer = 0;
            holdDur = 0;
            holdSliderImage.fillAmount = 0;

            interactionUI.SetActive(false);
        }
    } // end ClearInteraction

    void PopupInteractInfo(){
        if(interactable != null){
            switch(interactable.interactType){
                case E_InteractType.PICKUP:
                    if(interactable.buttonType == E_ButtonType.HOLD){
                        interactText.text = "Hold to pickup [" + interactable.gameObject.name.ToLower() + "]";
                    }else{
                        interactText.text = "To pickup [" + interactable.gameObject.name.ToLower() + "]";
                    }
                break;

                case E_InteractType.CHARGE:
                    if(interactable.buttonType == E_ButtonType.HOLD){
                        interactText.text = "Hold to charge phone";
                    }else{
                        interactText.text = "To charge phone";
                    }
                break;

                case E_InteractType.OPENDOOR:
                    if(interactable.buttonType == E_ButtonType.HOLD){
                        interactText.text = "Hold to open/close door";
                    }else{
                        interactText.text = "To open/close door";
                    }
                break;

                case E_InteractType.INTERACT:
                    if(interactable.buttonType == E_ButtonType.HOLD){
                        interactText.text = "Hold to interact";
                    }else{
                        interactText.text = "To interact";
                    }
                break;

                case E_InteractType.TRANSFER:
                    if(interactable.buttonType == E_ButtonType.HOLD){
                        interactText.text = "Hold to transfer";
                    }else{
                        interactText.text = "To transfer";
                    }
                break;

                case E_InteractType.SCAN:
                    if(interactable.buttonType == E_ButtonType.HOLD){
                        interactText.text = "Hold to scan";
                    }else{
                        interactText.text = "To scan";
                    }
                break;

                default:
                break;
            }
        }
    } // end PopupInfo
}
