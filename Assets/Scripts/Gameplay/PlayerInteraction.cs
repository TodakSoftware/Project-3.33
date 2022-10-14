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
    public float holdDur, holdTimer;
    public Image holdSliderImage;
    [Header("Wwise Related")]
    public AK.Wwise.Event itemPickupWwiseSound;
    public AK.Wwise.Event ritualItemPlaceWwiseSound;

    //[Header("Highlight Related")]
    //public bool isHighlighting;

    void Update(){
        if(enableInteract){
            if(interactable != null){

                if(interactable.GetComponent<Interactable>().buttonType == E_ButtonType.HOLD){
                    if(Input.GetButton("Interact")){

                        //wall charger related
                        if(interactable != null && interactable.CompareTag("WallCharger")){
                            
                            holdDur = interactable.GetComponent<Interactable>().holdDuration;
                           
                            if(holdTimer < holdDur && interactable.GetComponent<WallCharger>().isBatteryAvailable && GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().currentBattery < 103f){
                                
                                holdTimer += Time.deltaTime;
                                
                                holdSliderImage.fillAmount = holdTimer / holdDur;
                            
                                interactable.GetComponent<WallCharger>().AdjustCurrentCharger(-interactable.GetComponent<WallCharger>().currentCharge/100f); //DEDUCT WALL CHARGER BATTERY (holdDur*holdDur)
                                //interactable.GetComponent<WallCharger>().AdjustCurrentCharger((-interactable.GetComponent<WallCharger>().currentCharge/103f)*holdSliderImage.fillAmount);
                                
                                GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().chargeBattery(interactable.GetComponent<WallCharger>().currentCharge/100f); // Charge Battery
                                //GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().chargeBattery((interactable.GetComponent<WallCharger>().currentCharge/103f)*holdSliderImage.fillAmount);


                            }else{
                                holdTimer = holdDur;
                                interactable.GetComponent<WallCharger>().SetNewHoldDuration();
                                ClearInteraction();
                                print("Tutup UI & Habis Battery");
                                interactionUI.SetActive(false);
                                GetComponent<Human>().instantiatedPhone.GetComponent<MobilePhone>().isCharging = false;
                            }//wall charger related END

                        }else{//macam biasa

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
                            }// end holdtimer < holddur

                        }//end mcm biasa
                    }else if(Input.GetButtonUp("Interact")){
                        if(interactable != null && interactable.CompareTag("WallCharger")){
                             interactable.GetComponent<WallCharger>().SetNewHoldDuration();
                             print("Reset");
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
            
            UI_Simple_Notification_Spawner.instance.CreateNotification(NotificationType.PICKUPITEM, "Player"+photonView.OwnerActorNr , interactable.name); // Notification

            itemPickupWwiseSound.Post(gameObject); // play sound
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
            ritualItemPlaceWwiseSound.Post(gameObject);

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

                            // Highlight Ritual Item
                            /* if(hit.collider.CompareTag("RitualItem") && hit.collider.GetComponent<RitualItem>() != null && !isHighlighting){
                                hit.collider.GetComponent<RitualItem>().HighlightItem();
                                isHighlighting = true;
                            } */

                            // UI Related
                            if(!interactionUI.activeSelf){
                                interactionUI.SetActive(true);
                            }

                            if(interactable.CompareTag("WallCharger") && !interactable.GetComponent<WallCharger>().isBatteryAvailable){
                                if(interactionUI.activeSelf){
                                    interactionUI.SetActive(false);
                                }
                            }else{
                                PopupInteractInfo();
                            }

                            

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
                /* if(isHighlighting && interactable != null){
                    interactable.gameObject.GetComponent<RitualItem>().UnhighlightItem();
                    isHighlighting = false;
                } */
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
