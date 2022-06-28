using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Human : MonoBehaviour
{
    PlayerController playerController;

    [Header("ENABLER")]
    public bool canInteractPhone;
    // PHONE RELATED 
    [Header("PHONE")]
    public int playerMoney;
    [SerializeField] Transform itemHolderRight;
    [SerializeField] Transform itemHolderLeft;
    [SerializeField] GameObject phonePrefab; // <--- Will replaced by photon
    [HideInInspector] public GameObject instantiatedPhone; // <--- For references
    public bool isInteractPhone;
    bool phoneSwitchedPlaces;
    [HideInInspector] public bool interactAnimEnd;
    // ----------------------------------------------------------------------------------
    // CAMERA RELATED 
    [Header("CAMERA")]
    public GameObject cameraGO;
    float defaultFOV, defaultNearClipping;
    Vector3 defaultCamPosition;
    public float zoomInFOV;
    public float zoomInNearClipping;
    public Vector3 zoomInCamPosition;

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

        if(Input.GetButtonDown("Flashlight")){ // Tab key
            GetComponent<PlayerAbilities>().ToggleFlashlight();
        }
    }

    void SetupPhone(){
        // Setup player custom skin / early game ability / etc.

        // Spawn Phone (Will replace with photon prefab)
        instantiatedPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        instantiatedPhone.GetComponent<MobilePhone>().phoneOwner = this.gameObject;
        instantiatedPhone.transform.SetParent(itemHolderRight, false);

        // Store Transform Default Values
        defaultCamPosition = cameraGO.transform.localPosition;
        defaultNearClipping = cameraGO.GetComponent<Camera>().nearClipPlane;
        defaultFOV = cameraGO.GetComponent<Camera>().fieldOfView;
    }

    /* --------------------------------------------  PHONE RELATED FUNCTIONS START -------------------------------------------------*/
    public void HandleInteractPhone()
    {
        if(!isInteractPhone && !interactAnimEnd){
            // Disable mouse look & enable cursor
            playerController.camController.isEnable = false;
            playerController.camController.LockCursor(false);
            isInteractPhone = true;
            // Play interact phone animation
            playerController.anim.SetBool("InteractPhone", isInteractPhone);
            
            InteractZoomEffect(true); // Zoom In

        }else if(isInteractPhone && interactAnimEnd){
            // Enable Mouselook
            playerController.camController.isEnable = true;
            playerController.camController.LockCursor(true);
            isInteractPhone = false;
            // Play closed interact phone animation
            playerController.anim.SetBool("InteractPhone", isInteractPhone);
           
        }
    } // end HandleInteractPhone

    public void SwitchPhonePosition(){
        if(!phoneSwitchedPlaces){
            phoneSwitchedPlaces = true;
            instantiatedPhone.transform.SetParent(itemHolderLeft, false);
            
        }else{
            phoneSwitchedPlaces = false;
            instantiatedPhone.transform.SetParent(itemHolderRight, false);
            
            InteractZoomEffect(false);  // Zoom out
        }
    }

    public void InteractZoomEffect(bool zoomIn){
        if(zoomIn){
            instantiatedPhone.GetComponent<MobilePhone>().SwitchPhoneView(true);
            cameraGO.GetComponent<Camera>().DOFieldOfView(zoomInFOV, .8f);
            cameraGO.GetComponent<Camera>().DONearClipPlane(zoomInNearClipping, .6f);
            cameraGO.transform.DOLocalMove(zoomInCamPosition, .8f);
        }else{
            cameraGO.GetComponent<Camera>().DOFieldOfView(defaultFOV, .6f);
            cameraGO.GetComponent<Camera>().DONearClipPlane(defaultNearClipping, .1f).SetDelay(.3f);
            cameraGO.transform.DOLocalMove(defaultCamPosition, .8f);
            instantiatedPhone.GetComponent<MobilePhone>().SwitchPhoneView(false);
        }
    }
/* --------------------------------------------  PHONE RELATED FUNCTIONS END -------------------------------------------------*/
}
