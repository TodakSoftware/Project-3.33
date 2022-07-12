using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Human : MonoBehaviourPunCallbacks
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
    [Header("FEAR LEVEL")]
    // ----------------------------------------------------------------------------------
    // FEAR LEVEL
    [Range(0,100)] public int fearLevel;
    bool hpGreen, hpYellow, hpRed;
    public bool isScared;
    public bool isCaptured;

    [Header("Nearby Ghost Fear")]
    public bool ghostNearby;
    Collider[] ghostInRadiusList;
    bool fearIsIncrease;
    [SerializeField] int ghostNearbyIncreaseAmount = 1;
    [SerializeField] float nearbyDetectDistance = 2f;
    public LayerMask ghostLayermask;

    void Awake(){
        playerController = GetComponent<PlayerController>();
    }

    void Start(){
        SetupPhone();
        if(!photonView.IsMine){ // If this script is not ours to control, end it
            return;
        }

        StartCoroutine(UpdateHeartRate());
    }

    void Update(){
        if(!photonView.IsMine){ // If this script is not ours to control, end it
            return;
        }

        if(canInteractPhone){
            if(Input.GetButtonDown("Interact Phone")){ // Tab key
                HandleInteractPhone();
            }
        }

        if(Input.GetButtonDown("Flashlight") && !instantiatedPhone.GetComponent<MobilePhone>().phoneIsDead){ // Tab key & phone is not dead
            GetComponent<PlayerAbilities>().ToggleFlashlight("A001");
        }

        // ---------------------------------------- NEARBY GHOST UPDATE RELATED START ---------------------------------------
        ghostInRadiusList = Physics.OverlapSphere(this.transform.position, nearbyDetectDistance, ghostLayermask);
        foreach(var filter in ghostInRadiusList){ // If Ghost inRange, true fear increase
            ghostNearby = true;

            if(!fearIsIncrease){
                //StartCoroutine(GhostNearbyIncrease());
                photonView.RPC("EnableGhostNearbyIncrease", RpcTarget.All, true);
            }
        }

        if(ghostInRadiusList.Length == 0 && ghostNearby){ // If Ghost !inRange, false fear increase
            ghostNearby = false;

            if(fearIsIncrease){
                //fearIsIncrease = false;
                photonView.RPC("EnableGhostNearbyIncrease", RpcTarget.All, false);
            }
        }
        // ---------------------------------------- NEARBY GHOST UPDATE RELATED END ---------------------------------------

        if(Input.GetKeyDown(KeyCode.L)){
            //photonView.RPC("Captured", RpcTarget.All);
            //photonView.RPC("EnableGhostNearbyIncrease", RpcTarget.All, true);
        }

        if(Input.GetKeyDown(KeyCode.M)){
            //photonView.RPC("Captured", RpcTarget.All);
           // photonView.RPC("EnableGhostNearbyIncrease", RpcTarget.All, false);
        }
    }

    // ---------------------------------------- NEARBY GHOST FUNCTION RELATED START ---------------------------------------
    IEnumerator GhostNearbyIncrease(){
        fearIsIncrease = true;
        while(fearLevel < 100 && ghostNearby){
            yield return new WaitForSeconds(1f);
            photonView.RPC("AdjustFearLevel", RpcTarget.All, ghostNearbyIncreaseAmount);// AdjustFearLevel(fearAmount);
        }
    }

    [PunRPC]
    public void EnableGhostNearbyIncrease(bool increase){
        if(photonView.IsMine){
            if(increase){
                photonView.RPC("SetGhostNearby", RpcTarget.All, true);
                StartCoroutine(GhostNearbyIncrease());
            }else{
                photonView.RPC("SetGhostNearby", RpcTarget.All, false);
                fearIsIncrease = false;
            }
        }
    }

    [PunRPC]
    public void SetGhostNearby(bool isNear){
        if(isNear){
            ghostNearby = true;
        }else{
            ghostNearby = false;
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position, nearbyDetectDistance);
    }
    // ---------------------------------------- NEARBY GHOST FUNCTION RELATED START ---------------------------------------


/* --------------------------------------------  PHONE RELATED FUNCTIONS START -------------------------------------------------*/
    void SetupPhone(){
        // Setup player custom skin / early game ability / etc.

        // Spawn Phone (Will replace with photon prefab)
        instantiatedPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        instantiatedPhone.GetComponent<MobilePhone>().phoneOwner = this.gameObject;
        instantiatedPhone.transform.SetParent(itemHolderRight, false);

        if(GetComponent<PlayerAbilities>().flashlightOn){
            instantiatedPhone.GetComponent<MobilePhone>().drainBattery(true, "A001"); // Drain battery for flashlight
        }

        // Store Transform Default Values
        defaultCamPosition = cameraGO.transform.localPosition;
        defaultNearClipping = cameraGO.GetComponent<Camera>().nearClipPlane;
        defaultFOV = cameraGO.GetComponent<Camera>().fieldOfView;
    } // end SetupPhone()

    public void HandleInteractPhone()
    {
        if(!isInteractPhone && !interactAnimEnd){
            playerController.StopMovement();
            playerController.Invoke("UnstopMovement", 1.5f); // value same as EnterUIMode delay
            isInteractPhone = true;
            // Play interact phone animation
            playerController.anim.SetBool("InteractPhone", isInteractPhone);
            
            InteractZoomEffect(true); // Zoom In

        }else if(isInteractPhone && interactAnimEnd){
            isInteractPhone = false;
            // Play closed interact phone animation
            playerController.anim.SetBool("InteractPhone", isInteractPhone);
           
        }
    } // end HandleInteractPhone

    public void EnterUIMode(){
        playerController.camController.LockCursor(false);
        playerController.canMouselook = false;
    }

    public void ExitUIMode(){
        playerController.camController.LockCursor(true);
        playerController.canMouselook = true;
    }

    public void SwitchPhonePosition(){
        if(!phoneSwitchedPlaces){
            phoneSwitchedPlaces = true;
            instantiatedPhone.transform.SetParent(itemHolderLeft, false);
            
        }else{
            phoneSwitchedPlaces = false;
            instantiatedPhone.transform.SetParent(itemHolderRight, false);
            
            InteractZoomEffect(false);  // Zoom out
        }
    } // end SwitchPhonePosition()

    public void InteractZoomEffect(bool zoomIn){
        if(zoomIn){
            instantiatedPhone.GetComponent<MobilePhone>().SwitchPhoneView(true);
            cameraGO.GetComponent<Camera>().DOFieldOfView(zoomInFOV, .8f);
            cameraGO.GetComponent<Camera>().DONearClipPlane(zoomInNearClipping, .6f);
            cameraGO.transform.DOLocalMove(zoomInCamPosition, .8f);
            Invoke("EnterUIMode", 1.5f);
        }else{
            cameraGO.GetComponent<Camera>().DOFieldOfView(defaultFOV, .6f);
            cameraGO.GetComponent<Camera>().DONearClipPlane(defaultNearClipping, .1f).SetDelay(.3f);
            cameraGO.transform.DOLocalMove(defaultCamPosition, .8f);
            instantiatedPhone.GetComponent<MobilePhone>().SwitchPhoneView(false);
            Invoke("ExitUIMode", .5f);
        }
    } // end InteractZoomEffect()
/* --------------------------------------------  PHONE RELATED FUNCTIONS END -------------------------------------------------*/


// --------------------------------- HEART RATE FUNCTION START ----------------------------------
    private IEnumerator UpdateHeartRate(){
        while(fearLevel >= 0 && fearLevel <= 100){ 
            UpdateHeartUI();
            yield return new WaitForSeconds(1f);
        }
        EndHeartRate();
    } // end UpdateHeartRate()

    private void UpdateHeartUI(){
        if(fearLevel < 50){
            if(!hpGreen){
                instantiatedPhone.GetComponent<MobilePhone>().heartrateUI.GetComponent<Animator>().SetTrigger("Healthy");
                hpGreen = true;
                hpYellow = false;
                hpRed = false;
            }
        }else if(fearLevel >= 50 && fearLevel < 80){
            if(!hpYellow){
                instantiatedPhone.GetComponent<MobilePhone>().heartrateUI.GetComponent<Animator>().SetTrigger("Panic");
                hpGreen = false;
                hpYellow = true;
                hpRed = false;
            }
        }else if(fearLevel >= 80 && fearLevel <= 100){
            if(!hpRed){
                instantiatedPhone.GetComponent<MobilePhone>().heartrateUI.GetComponent<Animator>().SetTrigger("Dying");
                hpGreen = false;
                hpYellow = false;
                hpRed = true;
            }
        }
    } // end UpdateHeartUI()

    public void EndHeartRate(){
       
    } // end EndHeartRate()

    [PunRPC]
    public void AdjustFearLevel(int amount){
        fearLevel += amount;
        
        if(fearLevel <= 0){
            fearLevel = 0;
        }else if(fearLevel >= 100){
            fearLevel = 100;
        }
    } // end AdjustFearLevel()

// --------------------------------- HEART RATE FUNCTION END ----------------------------------
    [PunRPC]
    public IEnumerator Scared(float duration, int fearAmount){
        if(photonView.IsMine){
            photonView.RPC("SetIsScared", RpcTarget.All, true);
            UIManager.instance.PopupJumpscareUI();

            photonView.RPC("AdjustFearLevel", RpcTarget.All, fearAmount);// AdjustFearLevel(fearAmount);
            playerController.StopMovement();
            playerController.canMouselook = false;
            yield return new WaitForSeconds(duration);

            if(!isCaptured){
                playerController.UnstopMovement();
                playerController.canMouselook = true;
            }

            photonView.RPC("SetIsScared", RpcTarget.All, false);
        }
    } // end Scared

    [PunRPC]
    public IEnumerator Captured(){
        if(photonView.IsMine){
            //isCaptured = true; // link with custom props
            photonView.RPC("SetIsCaptured", RpcTarget.All, true);

            UIManager.instance.PopupCapturedUI();
            
            yield return new WaitForSeconds(2f);
            // Transfer to prison
            int randomNmbr = Random.Range(0, GameManager.instance.spawnpoints_CapturedRoom.Count);
            playerController.canMove = false; // false to make player move to new position
            transform.position = GameManager.instance.spawnpoints_CapturedRoom[randomNmbr].position;
            GameManager.instance.spawnpoints_CapturedRoom[randomNmbr].gameObject.GetComponent<CapturedSpawnpoints>().prisonDoor.GetComponent<Interact_Door>().photonView.RPC("HumanInsideRoom", RpcTarget.All, photonView.ViewID);

            yield return new WaitForSeconds(1f);
            playerController.UnstopMovement();
            
            // Update Properties
            Hashtable playerProps = new Hashtable();
            playerProps.Add("PlayerCaptured", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
    } // end Captured

    [PunRPC]
    public void Released(){
        if(photonView.IsMine){
            //isCaptured = true; // link with custom props
            photonView.RPC("SetIsCaptured", RpcTarget.All, false);
            
            // Update Properties
            Hashtable playerProps = new Hashtable();
            playerProps.Add("PlayerCaptured", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        }
    } // end Released

    [PunRPC]
    public void SetIsCaptured(bool captured){
        if(captured){
            isCaptured = true;
            GetComponent<PlayerInteraction>().enableInteract = false;
        }else{
            isCaptured = false;
            GetComponent<PlayerInteraction>().enableInteract = true;
        }
    } // end SetIsCaptured

    [PunRPC]
    public void SetIsScared(bool scared){
        if(scared){
            isScared = true;
        }else{
            isScared = false;
        }
    } // end SetIsCaptured

}
