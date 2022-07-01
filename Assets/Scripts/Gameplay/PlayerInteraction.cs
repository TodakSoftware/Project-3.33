using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    public LayerMask interactableLayerMask;
    public Transform cam;
    public float interactMaxDistance;
    public GameObject crosshairUI;
    public GameObject interactProgressUI;

    // --------------------------------------------------------------

    Interactable interactable;
    public Image interactImage;
    public Sprite defaultInteractIcon;
    public Sprite defaultIcon;
    public Vector2 defaultInteractIconSize;
    public Vector2 defaultIconSize;


    void FixedUpdate(){
        RaycastHit hit;
       
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, interactMaxDistance, interactableLayerMask)){
            if(hit.collider.GetComponent<Interactable>() != false){
                
                if(interactable == null || interactable.id != hit.collider.GetComponent<Interactable>().id){
                    interactable = hit.collider.GetComponent<Interactable>();
                } // end interactable null

                if(interactable.interactIcon != null){
                    interactImage.sprite = interactable.interactIcon;
                }else{
                    interactImage.sprite = defaultInteractIcon;
                    interactImage.rectTransform.sizeDelta = defaultInteractIconSize;
                } // end interactable Icon != null

                if(Input.GetButtonDown("Interact")){
                    interactable.onInteract.Invoke();
                } // end input getbutton
            }
        }else{
            if(interactImage.sprite != defaultIcon){
                interactImage.sprite = defaultIcon;
                interactImage.rectTransform.sizeDelta = defaultIconSize;
            }
        }
    }
}
