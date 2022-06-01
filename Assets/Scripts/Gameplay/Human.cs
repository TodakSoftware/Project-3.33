using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    PlayerController playerController;
    [Header("ENABLER")]
    public bool canInteractPhone;
    // PHONE RELATED 
    [Header("PHONE")]
    [SerializeField] Transform itemHolderRight;
    [SerializeField] Transform itemHolderLeft;
    [SerializeField] GameObject phonePrefab; // <--- Will replaced by photon
    GameObject instantiatedPhone; // <--- For references
    public bool isInteractPhone;
    bool phoneSwitchedPlaces;
    [HideInInspector] public bool interactAnimEnd;
    // ----------------------------------------------------------------------------------

    void Awake(){
        playerController = GetComponent<PlayerController>();
    }

    void Start(){
        SetupPhone();
    }

    void Update(){
        if(canInteractPhone){
            if(Input.GetButtonDown("Interact Phone")){ // Tab key
                HandleInteractPhone();
            }
        }
    }

    void SetupPhone(){
        // Setup player custom skin / early game ability / etc.

        // Spawn Phone (Will replace with photon prefab)
        instantiatedPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        instantiatedPhone.transform.SetParent(itemHolderRight, false);
    }

    /* --------------------------------------------  PHONE RELATED FUNCTIONS START -------------------------------------------------*/
    void HandleInteractPhone()
    {
        if(!isInteractPhone && !interactAnimEnd){
            // Disable mouse look & enable cursor
            playerController.camController.isEnable = false;
            playerController.camController.LockCursor(false);
            isInteractPhone = true;
            // Play interact phone animation
            playerController.anim.SetBool("InteractPhone", isInteractPhone);
            // Zoom In
        }else if(isInteractPhone && interactAnimEnd){
            // Enable Mouselook
            playerController.camController.isEnable = true;
            playerController.camController.LockCursor(true);
            isInteractPhone = false;
            // Play closed interact phone animation
            playerController.anim.SetBool("InteractPhone", isInteractPhone);
            // Zoom out
        }
    } // end HandleInteractPhone

    public void SwitchPhonePosition(){
        if(!phoneSwitchedPlaces){
            phoneSwitchedPlaces = true;
            instantiatedPhone.transform.SetParent(itemHolderLeft, false);
        }else{
            phoneSwitchedPlaces = false;
            instantiatedPhone.transform.SetParent(itemHolderRight, false);
        }
    }
/* --------------------------------------------  PHONE RELATED FUNCTIONS END -------------------------------------------------*/
}
