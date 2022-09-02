// Handle player(Human/Ghost) movement
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks
{
    // STATIC VARIABLE
    Vector3 velocity; 
    CharacterController characterController;
    [HideInInspector] public Animator anim;
    public GameObject playerMesh; // player mesh reference
    public E_Team team;
    // ----------------------------------------------------------------------------------
    // ENABLER RELATED
    [Header("ENABLER")]
    public bool canMove;
    public bool canRun, canJump, canMouselook, enableFootstep;
    // ----------------------------------------------------------------------------------
    // MOVEMENT RELATED
    [Header("MOVEMENT")]
    [SerializeField] float movementSpeed = 2f; // Actual speed for movement
    float currentSpeed; // Realtime walk speed value (For debugging) Do Not Change this
    [SerializeField] float jumpSpeed = .5f; // Jump height
    Vector3 movementDir;
    float gravity = -9.81f; // Ground check (For debugging) Do Not Change this
    // ----------------------------------------------------------------------------------
    // SPRINT/RUN RELATED
    [Header("SPRINT / RUN")]
    [SerializeField] float runMultiplier = 1; // Multiplier for running
    float tempRunMultiplier, runAnim = 0.3f; // Smooth multiplier for running (+animation)
    bool isRunning, walkForward;
    [SerializeField] bool enableStaminaDrain = true;
    [SerializeField] float staminaAmount = 100f;
    [SerializeField] float staminaDrainPerSecond = 20f;
    [SerializeField] float staminaRegenPerSecond = 10f;
    // ----------------------------------------------------------------------------------
    // ANIMATION RELATED
    float inputVertical, inputHorizontal, smoothVer, smoothHor;
    // ----------------------------------------------------------------------------------
    // CAMERA RELATED
    [Header("CAMERA")]
    public CameraController camController;
    public CameraControlledIK camControllerIK;
    // ----------------------------------------------------------------------------------
    public TextMeshProUGUI debugText;
    // ----------------------------------------------------------------------------------
    // AUDIO RELATED
    [Header("AUDIO")]
    [SerializeField] SO_FootstepList footstepSO;
    [SerializeField] float baseStepSpeed = .6f;
    [SerializeField] float sprintStepMultiplier = .7f;
    AudioSource footStepAudioSource = default;
    float footstepTimer = 0;
    public float GetCurrentOffset => isRunning ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = playerMesh.GetComponent<Animator>();
        if(footStepAudioSource == null){
            footStepAudioSource = GetComponent<AudioSource>();
        }

        StartCoroutine(UpdateDebugText());
    } // end Awake

    void Update()
    {
        if(!photonView.IsMine){ // If this script is not ours to control, end it
            return;
        }

        if(photonView.Owner != photonView.Controller){ // if player disconnect, prevent host from controlling
            return;
        }

        HandleGravity();

        if(canJump){
            if(Input.GetButtonDown("Jump") && characterController.isGrounded){
                HandleJumping();
            }
        } // end canJump
        
        if(canMouselook){
            if(!camController.enabled){
                camController.enabled = true;
            }
        }else{
            if(camController.enabled){
                camController.enabled = false;
            }
        } // end canMouseLook

        if(enableFootstep){ // Toggle if we want to enable footstep sound
            HandleFootstep();
        } // end enableFootstep

        // CAN ONLY DO SPRINTING WHEN WALK FORWARD (Do it in Update function because of Input processing)
        if(canMove && canRun && walkForward && inputHorizontal == 0){ //  && !isInteractPhone
            if(Input.GetButton("Sprint")){
                characterController.Move(movementDir * Time.deltaTime * tempRunMultiplier);
                //anim.speed = runMultiplier + 0.3f; // additional 0.3f to match with walk anim (speedup)
                anim.SetBool("Running", true);
                isRunning = true;
            }else if(Input.GetButtonUp("Sprint")){
                //anim.speed = 1; // revert back to normal speed animation
                anim.SetBool("Running", false);
                isRunning = false;
            }
        }else if(canMove && canRun && walkForward && inputHorizontal != 0){
            anim.SetBool("Running", false);
            walkForward = false;
            isRunning = false;
        } // end canRun && walkForward && inputHorizontal == 0 

        if(isRunning){
            if(enableStaminaDrain && isRunning && staminaAmount > 0){ // Handle Stamina Drain
                staminaAmount -= staminaDrainPerSecond * Time.deltaTime;

                if(staminaAmount <= 0){
                    staminaAmount = 0;
                    canRun = false;

                    anim.SetBool("Running", false);
                    walkForward = false;
                    isRunning = false;
                }
            }
        }else{
            if(enableStaminaDrain && !isRunning && staminaAmount < 100f){ // Handle Stamina Regen
                staminaAmount += staminaRegenPerSecond * Time.deltaTime;

                if(staminaAmount > 50f){ // Minimum value for player can run again
                    canRun = true;
                }
            }
        }

        // ANIMATION RELATED
        SmoothAnimation();
        
        anim.SetFloat("Vertical", smoothVer);
        anim.SetFloat("Horizontal", smoothHor);

        if(canMove){
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");

            movementDir = transform.right * inputHorizontal + transform.forward * inputVertical;
            movementDir = Vector3.ClampMagnitude(movementDir, 1f);

            characterController.Move(movementDir * currentSpeed * Time.deltaTime); // keeps gravity fall
        }

        // DEBUG
        if(Input.GetKeyDown(KeyCode.K)){
            movementSpeed += 1f;
        }

        if(Input.GetKeyDown(KeyCode.L)){
            movementSpeed -= 1f;
        }

    } // end Update

    IEnumerator UpdateDebugText(){
        debugText.text = "Human " + GameManager.GetAllPlayersHuman().Count + " | Ghost " + GameManager.GetAllPlayersGhost().Count;
        yield return new WaitForSeconds(1f);
        StartCoroutine(UpdateDebugText());
    } // end UpdateDebugText

    public void StopMovement(){
        inputHorizontal = 0f;
        inputVertical = 0f;
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        anim.SetBool("Running", false);

        canMove = false;
        canRun = false;
    } // end StopMovement

    public void UnstopMovement(){
        canMove = true;
        canRun = true;
    } // UnstopMovement

    void FixedUpdate() {
        if(!photonView.IsMine){ // If this script is not ours to control, end it
            return;
        }

        if(photonView.Owner != photonView.Controller){ // if player disconnect, prevent host from controlling
            return;
        }

        if(canMove){
            HandleMovement();
        }
    } // end FixedUpdate

/* -------------------------------------------- BASIC MOVEMENT HANDLER FUNCTIONS START -------------------------------------------------*/
    void HandleGravity(){
        if(characterController.isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
        
        // Handle Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
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
    } // end HandleMovement

    void HandleFootstep(){ // Footstep Audio function
        if(!characterController.isGrounded) return; // if we are not grounded, return / prevent from proceed
        if(movementDir == Vector3.zero) return; // same as we haven't received any input

        footstepTimer -= Time.deltaTime; // always deduct footstep timer value > 0

        if(footstepTimer <= 0){
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f)){ // Check raycast below our player
                switch(hit.collider.tag){
                    case "Floor/Cement":
                        foreach(var sound in footstepSO.list){ // filter out sound list
                            if(sound.name == "Cement"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;

                    case "Floor/Grass":
                        foreach(var sound in footstepSO.list){ // filter out sound list
                            if(sound.name == "Grass"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;

                    case "Floor/Tiles":
                        foreach(var sound in footstepSO.list){ // filter out sound list
                            if(sound.name == "Tiles"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;

                    case "Floor/Wood":
                        foreach(var sound in footstepSO.list){ // filter out sound list
                            if(sound.name == "Wood"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;

                    case "Floor/Broken Glass":
                        foreach(var sound in footstepSO.list){ // filter out sound list
                            if(sound.name == "Broken Glass"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;

                    case "Floor/Metal":
                        foreach(var sound in footstepSO.list){ // filter out sound list
                            if(sound.name == "Metal"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;

                    default:
                        foreach(var sound in footstepSO.list){
                            if(sound.name == "Cement"){ 
                                footStepAudioSource.PlayOneShot(sound.audioClipLists[UnityEngine.Random.Range(0, sound.audioClipLists.Count)]);
                            }
                        } // end foreach
                    break;
                } // end switch
            } // end if pyhsics

            footstepTimer = GetCurrentOffset; // set footstepTimer > 0, to prevent audio playing instantly
        } // end if timer <= 0
    } // end HandleFootstep

    void HandleJumping(){
        velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
    } // end HandleJumping

/* -------------------------------------------- BASIC MOVEMENT HANDLER FUNCTIONS END -------------------------------------------------*/

} // end monobehaviour
