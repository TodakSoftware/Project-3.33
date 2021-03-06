// Handle player(Human/Ghost) movement
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
    [HideInInspector] public Animator anim;
    public GameObject playerMesh; // player mesh reference
    public E_Team team;
    // ----------------------------------------------------------------------------------
    // ENABLER RELATED
    [Header("ENABLER")]
    public bool canMove;
    public bool canRun, canJump, canMouselook;
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
    public CameraController camController;
    public CameraControlledIK camControllerIK;
    // ----------------------------------------------------------------------------------

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = playerMesh.GetComponent<Animator>();
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

    } // end Update

    public void StopMovement(){
        inputHorizontal = 0f;
        inputVertical = 0f;
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        anim.SetBool("Running", false);

        canMove = false;
        canRun = false;
    }

    public void UnstopMovement(){
        canMove = true;
        canRun = true;
    }

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

    void HandleJumping(){
        velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
    } // end HandleJumping

/* -------------------------------------------- BASIC MOVEMENT HANDLER FUNCTIONS END -------------------------------------------------*/

} // end monobehaviour
