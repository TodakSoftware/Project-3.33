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
    public GameObject thermalCameraGO;
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
    bool capturedAnimRun;
    public int currentDeadTimeout, deadTimeout = 60; // in seconds
    public bool isDead;
    Coroutine deadtimeoutCoroutine;

    [Header("Nearby Ghost Fear")]
    public bool ghostNearby;
    Collider[] ghostInRadiusList;
    bool fearIsIncrease;
    [SerializeField] int ghostNearbyIncreaseAmount = 1;
    [SerializeField] float nearbyDetectDistance = 2f;
    public LayerMask ghostLayermask;

    [Header("Wwise Related")]
    public SoundRTPC fearLevelRTPC;

    void Awake(){
        playerController = GetComponent<PlayerController>();
    }

    void Start(){
        
        if(!photonView.IsMine){ // If this script is not ours to control, end it
            
            return;
        }
        if(PhotonNetwork.IsConnected){
            photonView.RPC("SetupPhone", RpcTarget.All);
        }else{
            SetupPhone();
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
            //GetComponent<PlayerAbilities>().ToggleFlashlight("A001");
            GetComponent<PlayerAbilities>().photonView.RPC("ToggleFlashlight", RpcTarget.All, "A001");
        }

        // ---------------------------------------- NEARBY GHOST UPDATE RELATED START ---------------------------------------
        ghostInRadiusList = Physics.OverlapSphere(this.transform.position, nearbyDetectDistance, ghostLayermask);
        foreach(var filter in ghostInRadiusList){ // If Ghost inRange, true fear increase
            ghostNearby = true;

            if(!fearIsIncrease){
                photonView.RPC("EnableGhostNearbyIncrease", RpcTarget.All, true);
            }
        }

        if(ghostInRadiusList.Length == 0 && ghostNearby){ // If Ghost !inRange, false fear increase
            ghostNearby = false;

            if(fearIsIncrease){
                photonView.RPC("EnableGhostNearbyIncrease", RpcTarget.All, false);
            }
        }
        // ---------------------------------------- NEARBY GHOST UPDATE RELATED END ---------------------------------------
        if(Input.GetKeyDown(KeyCode.C)){
            StartCoroutine(Captured());
        }

        if(Input.GetKeyDown(KeyCode.M)){
           
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
    [PunRPC]
    void SetupPhone(){
        // Setup player custom skin / early game ability / etc.

        // Spawn Phone (Will replace with photon prefab)
        //instantiatedPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        if(PhotonNetwork.IsConnected){
            instantiatedPhone = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Props", "MobilePhone"), new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        }else{
            // If not online
            instantiatedPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        }
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
        if(instantiatedPhone != null){
            if(!phoneSwitchedPlaces){
                phoneSwitchedPlaces = true;
                instantiatedPhone.transform.SetParent(itemHolderLeft, false);
                
            }else{
                phoneSwitchedPlaces = false;
                instantiatedPhone.transform.SetParent(itemHolderRight, false);
                
                InteractZoomEffect(false);  // Zoom out
            }
        }
    } // end SwitchPhonePosition()

    public void InteractZoomEffect(bool zoomIn){
        if(cameraGO != null){
            if(zoomIn){
                instantiatedPhone.GetComponent<MobilePhone>().SwitchPhoneView(true);
                cameraGO.GetComponent<Camera>().DOFieldOfView(zoomInFOV, .8f);
                thermalCameraGO.GetComponent<Camera>().DOFieldOfView(zoomInFOV, .7f);
                cameraGO.GetComponent<Camera>().DONearClipPlane(zoomInNearClipping, .6f);
                thermalCameraGO.GetComponent<Camera>().DONearClipPlane(zoomInNearClipping, .5f);
                cameraGO.transform.DOLocalMove(zoomInCamPosition, .8f);
                thermalCameraGO.transform.DOLocalMove(zoomInCamPosition, .7f);
                Invoke("EnterUIMode", .9f);
            }else{
                cameraGO.GetComponent<Camera>().DOFieldOfView(defaultFOV, .6f);
                thermalCameraGO.GetComponent<Camera>().DOFieldOfView(defaultFOV, .5f);
                cameraGO.GetComponent<Camera>().DONearClipPlane(defaultNearClipping, .1f).SetDelay(.3f);
                thermalCameraGO.GetComponent<Camera>().DONearClipPlane(defaultNearClipping, .1f).SetDelay(.2f);
                cameraGO.transform.DOLocalMove(defaultCamPosition, .8f);
                thermalCameraGO.transform.DOLocalMove(defaultCamPosition, .7f);
                instantiatedPhone.GetComponent<MobilePhone>().SwitchPhoneView(false);
                Invoke("ExitUIMode", .5f);
            }
        } // end cameraGO
    } // end InteractZoomEffect()
/* --------------------------------------------  PHONE RELATED FUNCTIONS END -------------------------------------------------*/


// --------------------------------- HEART RATE FUNCTION START ----------------------------------
    private IEnumerator UpdateHeartRate(){
        while(fearLevel >= 0 && fearLevel <= 100){ 
            UpdateHeartUI();
            fearLevelRTPC.PlayFearLevelSound();
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
    public IEnumerator Scared(float duration, int fearAmount){ // Used by CaughtCollider.cs
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
        if(photonView.IsMine && !capturedAnimRun){
            capturedAnimRun = true;
            //isCaptured = true; // link with custom props
            photonView.RPC("SetIsCaptured", RpcTarget.All, true);
            PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Particles", "blackSmoke"), transform.position, Quaternion.Euler(-90, 0, 0));

            Invoke("DelayCapturedPopup", 1.2f);

            playerController.anim.SetBool("Captured", true);
            print("Play Captured Anim");
            
            yield return new WaitForSeconds(2.7f);
            // Transfer to prison
            int randomNmbr = Random.Range(0, GameManager.instance.spawnpoints_CapturedRoom.Count);
            playerController.canMove = false; // false to make player move to new position
            transform.position = GameManager.instance.spawnpoints_CapturedRoom[randomNmbr].position;
            
            GameManager.instance.spawnpoints_CapturedRoom[randomNmbr].gameObject.GetComponent<CapturedSpawnpoints>().prisonDoor.GetComponent<Interact_Door>().photonView.RPC("HumanInsideRoom", RpcTarget.All, photonView.ViewID);

            playerController.anim.SetBool("Captured", false);
            capturedAnimRun = false;

            yield return new WaitForSeconds(1f);
            playerController.UnstopMovement();
            
            // Update Properties
            Hashtable playerProps = new Hashtable();
            playerProps.Add("PlayerCaptured", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

            
        }
    } // end Captured

    void DelayCapturedPopup()
    {
        UIManager.instance.PopupCapturedUI();
    }

    [PunRPC]
    public void Released(){ // Used by Interact_Door.cs
        if(photonView.IsMine && !isDead){
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
            StartCoroutine(DeadCountdown());
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

    IEnumerator DeadCountdown(){
        currentDeadTimeout = 0;

        while(currentDeadTimeout < deadTimeout && isCaptured){ // add  && isCaptured
            yield return new WaitForSeconds(1f);
            currentDeadTimeout += 1;
        }

        if(currentDeadTimeout >= deadTimeout && !isDead){
            currentDeadTimeout = deadTimeout;
            photonView.RPC("SetIsDead", RpcTarget.All, true);
        }
    }

    [PunRPC]
    public void SetIsDead(bool dead){
        if(dead){
            isDead = true;
            // Play Animation
        }else{
            isDead = false;
        }
    } // end SetIsCaptured

}
