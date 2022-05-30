using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    // STATIC VARIABLE
    Vector3 velocity; 
    CharacterController characterController;
    Animator anim;
    [SerializeField] GameObject playerMesh; // player mesh reference
    // ----------------------------------------------------------------------------------
    // ENABLER RELATED
    [Header("----- ENABLER -----")]
    [SerializeField] bool canMove;
    [SerializeField] bool canRun, canJump, canMouselook, canInteractPhone;
    // ----------------------------------------------------------------------------------
    // MOVEMENT RELATED
    [Header("----- MOVEMENT -----")]
    [SerializeField] float movementSpeed = 2f; // Actual speed for movement
    float currentSpeed; // Realtime walk speed value (For debugging) Do Not Change this
    [SerializeField] float jumpSpeed = .5f; // Jump height
    Vector3 movementDir;
    float gravity = -9.81f; // Ground check (For debugging) Do Not Change this
    // ----------------------------------------------------------------------------------
    // SPRINT/RUN RELATED
    [SerializeField] float runMultiplier = 1; // Multiplier for running
    float tempRunMultiplier, runAnim = 0.3f; // Smooth multiplier for running (+animation)
    bool isRunning, walkForward;
    // ----------------------------------------------------------------------------------
    // ANIMATION RELATED
    float inputVertical, inputHorizontal, smoothVer, smoothHor;
    // ----------------------------------------------------------------------------------
    // CAMERA RELATED
    [Header("----- CAMERA -----")]
    [SerializeField] CameraController camController;
    // ----------------------------------------------------------------------------------
    // PHONE RELATED 
    [Header("----- PHONE -----")]
    [SerializeField] Transform itemHolderRight;
    [SerializeField] Transform itemHolderLeft;
    [SerializeField] GameObject phonePrefab; // <--- Will replaced by photon
    GameObject instantiatedPhone; // <--- For references
    bool isInteractPhone, phoneSwitchedPlaces;
    public bool interactAnimEnd;
    // ----------------------------------------------------------------------------------
    // PHOTON RELATED 
    [Header("----- PHOTON -----")]
    int playerID; 
    Player photonPlayer;
    // ----------------------------------------------------------------------------------

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = playerMesh.GetComponent<Animator>();
    }

    void Start(){
        SetupPlayer();
    }

    void Update()
    {
        HandleGravity();
        if(canJump){
            if(Input.GetButtonDown("Jump") && characterController.isGrounded){
                HandleJumping();
            }
        }
        if(canInteractPhone){
            if(Input.GetButtonDown("Interact Phone")){
                HandleInteractPhone();
            }
        }
        if(canMouselook){
            if(!camController.enabled){
                camController.enabled = true;
            }
        }else{
            if(camController.enabled){
                camController.enabled = false;
            }
        }

        // CAN ONLY DO SPRINTING WHEN WALK FORWARD (Do it in Update function because of Input processing)
        if(canRun && walkForward && inputHorizontal == 0 && !isInteractPhone){
            if(Input.GetButton("Sprint")){
                characterController.Move(movementDir * Time.fixedDeltaTime * tempRunMultiplier);
                //anim.speed = runMultiplier + 0.3f; // additional 0.3f to match with walk anim (speedup)
                anim.SetBool("Running", true);
                isRunning = true;
            }else if(Input.GetButtonUp("Sprint")){
                //anim.speed = 1; // revert back to normal speed animation
                anim.SetBool("Running", false);
                isRunning = false;
            }
        }else if(canRun && walkForward && inputHorizontal != 0){
            anim.SetBool("Running", false);
            walkForward = false;
            isRunning = false;
        }
    }

    void FixedUpdate() {
        if(canMove){
            HandleMovement();
        }
    }

    void SetupPlayer(){
        // Setup player custom skin / early game ability / etc.

        // Spawn Phone (Will replace with photon prefab)
        instantiatedPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        instantiatedPhone.transform.SetParent(itemHolderRight, false);
    }

/* -------------------------------------------- BASIC MOVEMENT HANDLER FUNCTIONS START -------------------------------------------------*/
    void HandleGravity(){
        if(characterController.isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
        
        // Handle Gravity
        velocity.y += gravity * Time.fixedDeltaTime;
        characterController.Move(velocity * Time.fixedDeltaTime);
    } // end HandleGravity

    void SmoothAnimation(){ // Smoothing animation movement value by inc/dec value by 0.09f per second instead snap
        // Smooth Vertical
        if(inputVertical > 0){
            smoothVer += 0.09f;
            if(smoothVer >= 1){
                smoothVer = 1;
            }
        }else if(inputVertical < 0){
            smoothVer -= 0.09f;
            if(smoothVer <= -1){
                smoothVer = -1;
            }
        }else{
            if(smoothVer > 0){
                smoothVer -= 0.09f;
                if(smoothVer <= 0){
                    smoothVer = 0;
                }
            }else if(smoothVer < 0){
                smoothVer += 0.09f;
                if(smoothVer >= 0){
                    smoothVer = 0;
                }
            }
        }

        // Smooth Horizontal
        if(inputHorizontal > 0){
            smoothHor += 0.4f;
            if(smoothHor >= 1){
                smoothHor = 1;
            }
        }else if(inputHorizontal < 0){
            smoothHor -= 0.4f;
            if(smoothHor <= -1){
                smoothHor = -1;
            }
        }else{
            if(smoothHor > 0){
                smoothHor -= 0.4f;
                if(smoothHor <= 0){
                    smoothHor = 0;
                }
            }else if(smoothHor < 0){
                smoothHor += 0.4f;
                if(smoothHor >= 0){
                    smoothHor = 0;
                }
            }
        }

        // Tweak Running animation
        if(isRunning){
            tempRunMultiplier += 0.08f;
            if(tempRunMultiplier >= runMultiplier){
                tempRunMultiplier = runMultiplier;
            }

            // Animation
            runAnim += 0.07f;
            if(runAnim >= 1f){
                anim.speed = 1f;
                runAnim = 1f;
            }else{
                anim.speed = runAnim;
            }
        }else if(tempRunMultiplier > 0 && !isRunning){
                tempRunMultiplier = 0;
                runAnim = 0.3f;
        }
    } // end smooth animation

    void HandleMovement(){
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        movementDir = transform.right * inputHorizontal + transform.forward * inputVertical;
        movementDir = Vector3.ClampMagnitude(movementDir, 1f);

        if(inputVertical < -0.5f){  // if walk backward
            currentSpeed = movementSpeed / 1.4f;
            walkForward = false;
        }else if(inputVertical > 0.5f){    // if walk forward
            currentSpeed = movementSpeed;
            if(inputVertical > 0){ // Detailing. Only set true if above 0
                walkForward = true;
            }
        }else if(inputHorizontal > 0.5f || inputHorizontal < -0.5f){   // if strafe
            currentSpeed = movementSpeed / 1.4f;
            walkForward = false;
        }

        characterController.Move(movementDir * currentSpeed * Time.fixedDeltaTime); // keeps gravity fall

        // ANIMATION RELATED
        SmoothAnimation();
        
        anim.SetFloat("Vertical", smoothVer);
        anim.SetFloat("Horizontal", smoothHor);
    } // end HandleMovement

    void HandleJumping(){
        velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
    } // end HandleJumping

    void HandleInteractPhone()
    {
        if(!isInteractPhone && !interactAnimEnd){
            // Disable mouse look & enable cursor
            camController.isEnable = false;
            camController.LockCursor(false);
            isInteractPhone = true;
            // Play interact phone animation
            anim.SetBool("InteractPhone", isInteractPhone);
            // Zoom In
        }else if(isInteractPhone && interactAnimEnd){
            // Enable Mouselook
            camController.isEnable = true;
            camController.LockCursor(true);
            isInteractPhone = false;
            // Play closed interact phone animation
            anim.SetBool("InteractPhone", isInteractPhone);
            // Zoom out
        }
    } // end HandleInteractPhone

/* -------------------------------------------- BASIC MOVEMENT HANDLER FUNCTIONS END -------------------------------------------------*/

/* --------------------------------------------  PHONE RELATED FUNCTIONS START -------------------------------------------------*/
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

/* --------------------------------------------  PHOTON RELATED FUNCTIONS START -------------------------------------------------*/
    [PunRPC]
    public void InitializePhotonPlayer(Player player){ // Called by GamaManager (SpawnPlayers())
        photonPlayer = player;
        playerID = player.ActorNumber;

        // Register self into playerControllerArray in GameManager
        GameManager.instance.playerControllerArray[playerID - 1] = this;

    }
/* --------------------------------------------  PHOTON RELATED FUNCTIONS END -------------------------------------------------*/
} // end monobehaviour
