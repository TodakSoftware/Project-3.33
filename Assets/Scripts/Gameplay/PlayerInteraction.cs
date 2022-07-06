using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
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

    [Header("Altar Related")]
    public bool canContributeItem;
    public Altar altarRef;

    void Update(){
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
                            if(interactable.gameObject.GetComponent<RitualItem>() != null && !GetComponent<PlayerInventory>().IsInventoryFull()){
                                GetComponent<PlayerInventory>().AddRitualItem(interactable.gameObject.GetComponent<RitualItem>().code); // Add ritual items to player slot
                                interactable.onInteract.Invoke();
                            }else{
                                interactable.onInteract.Invoke();
                            }
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
                        if(interactable.gameObject.GetComponent<RitualItem>() != null && !GetComponent<PlayerInventory>().IsInventoryFull()){
                            GetComponent<PlayerInventory>().AddRitualItem(interactable.gameObject.GetComponent<RitualItem>().code);
                            interactable.onInteract.Invoke();
                        }else{
                            interactable.onInteract.Invoke();
                        }
                        ClearInteraction();
                    }else{
                        print("Cannot proceed");
                    }
                    
                } // end input getbutton
            }

        } // end interacble

// ----------------------------------- RITUAL ALTAR RELATED START -------------------------------------------
        if(canContributeItem && GetComponent<PlayerInventory>().ritualItemLists.Count > 0){
            if(!interactionUI.activeSelf){
                interactionUI.SetActive(true);
            }

            if(altarRef.gameObject.GetComponent<Interactable>().buttonType == E_ButtonType.HOLD){
                interactText.text = "Hold to transfer";
                if(Input.GetButton("Interact")){
                    if(holdTimer < altarRef.gameObject.GetComponent<Interactable>().holdDuration){
                        holdTimer += Time.deltaTime;
                        holdSliderImage.fillAmount = holdTimer / altarRef.gameObject.GetComponent<Interactable>().holdDuration;
                    }else{
                        holdTimer = altarRef.gameObject.GetComponent<Interactable>().holdDuration;
                        TransferRitualItems();
                    }
                }else{
                    holdTimer = 0;
                    holdSliderImage.fillAmount = 0;
                } // end input getbutton
            }else{
                interactText.text = "To transfer";

                if(Input.GetButtonDown("Interact")){
                    TransferRitualItems();
                }
            }
        }
// ----------------------------------- RITUAL ALTAR RELATED END -------------------------------------------            
    } // end Update()

    void FixedUpdate(){
        RaycastHit hit;
       
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactMaxDistance, interactableLayerMask)){
            if(hit.collider.GetComponent<Interactable>() != false){
                
                if(interactable == null || interactable.id != hit.collider.GetComponent<Interactable>().id){
                    interactable = hit.collider.GetComponent<Interactable>();

                    holdDur = hit.collider.GetComponent<Interactable>().holdDuration;
                    
                    if(!interactionUI.activeSelf){
                        interactionUI.SetActive(true);
                    }
                    PopupInteractInfo();
                } // end interactable null
            }
        }else{
            interactable = null;

            ClearInteraction();
        }
    } // end update()

    void ClearInteraction(){
        if(altarRef == null && !canContributeItem && interactable == null && interactionUI.activeSelf){
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
                        interactText.text = "Hold to open door";
                    }else{
                        interactText.text = "To open door";
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

    public void TransferRitualItems(){
        if(altarRef != null){
            altarRef.ContributeRitualItem(GetComponent<PlayerInventory>().ritualItemLists);
            GetComponent<PlayerInventory>().ritualItemLists.Clear();

            // Clear UI Interaction
            holdTimer = 0;
            holdDur = 0;
            holdSliderImage.fillAmount = 0;
            if(interactionUI.activeSelf){
                interactionUI.SetActive(false);
            }
        }
    }
}
