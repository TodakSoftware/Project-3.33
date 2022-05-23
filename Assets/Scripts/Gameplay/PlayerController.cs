using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // STATIC VARIABLE
    Vector3 velocity; 
    CharacterController characterController;
    Animator anim;
    [SerializeField] GameObject playerMesh; // player mesh reference
    // ----------------------------------------------------------------------------------
    // ENABLER RELATED
    [SerializeField] bool canMove, canRun, canJump, canInteractPhone;
    // ----------------------------------------------------------------------------------
    // MOVEMENT RELATED
    float currentSpeed; // Realtime walk speed value (For debugging) Do Not Change this
    [SerializeField] float movementSpeed = 2f; // Actual speed for movement
    [SerializeField] float jumpSpeed = .5f; // Jump height
    float gravity = -9.81f; // Ground check (For debugging) Do Not Change this
    // ----------------------------------------------------------------------------------
    // SPRINT/RUN RELATED
    [SerializeField] float runMultiplier = 1; // Multiplier for running
    // ----------------------------------------------------------------------------------
    // ANIMATION RELATED
    float inputVertical, inputHorizontal, smoothVer, smoothHor;
    // ----------------------------------------------------------------------------------
    // PHONE RELATED 
    [SerializeField] Transform itemHolderRight, itemHolderLeft;
    public GameObject phonePrefab; // <--- Will replaced by photon
    bool isInteractPhone;
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
            HandleJumping();
        }
        if(canInteractPhone){
            HandleInteractPhone();
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
        var tPhone = Instantiate(phonePrefab, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 90f)));
        tPhone.transform.SetParent(itemHolderRight, false);
    }

    void HandleGravity(){
        if(characterController.isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
        
        // Handle Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

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
    } // end smooth animation

    void HandleMovement(){
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * inputHorizontal + transform.forward * inputVertical;
        movement = Vector3.ClampMagnitude(movement, 1f);

        if(inputVertical < -0.5f){  // if walk backward
            currentSpeed = movementSpeed / 1.4f;
        }else if(inputVertical > 0.5f){    // if walk forward
            currentSpeed = movementSpeed; 

            // CAN ONLY DO SPRINTING WHEN WALK FORWARD
            if(canRun){
                if(Input.GetButton("Sprint")){
                    characterController.Move(movement * Time.deltaTime * runMultiplier);
                    anim.speed = runMultiplier + 0.3f; // additional 0.3f to match with walk anim (speedup)
                }else if(Input.GetButtonUp("Sprint")){
                    anim.speed = 1; // revert back to normal speed animation
                }
            }

        }else if(inputHorizontal > 0.5f || inputHorizontal < -0.5f){   // if strafe
            currentSpeed = movementSpeed / 1.4f;
        }

        characterController.Move(movement * currentSpeed * Time.deltaTime); // keeps gravity fall

        // ANIMATION RELATED
        SmoothAnimation();
        
        anim.SetFloat("Vertical", smoothVer);
        anim.SetFloat("Horizontal", smoothHor);
    }

    void HandleJumping(){
        if(Input.GetButtonDown("Jump") && characterController.isGrounded){
            velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }
    }

    void HandleInteractPhone()
    {
        if(Input.GetButtonDown("Interact Phone")){
            if(!isInteractPhone){ // if not interacting phone, can proceed interact
                // Disable mouse look & enable cursor
                isInteractPhone = true;
                // Play animation
                anim.SetBool("InteractPhone", isInteractPhone);
                // Zoom In
            }else{
                // Disable mouse look & enable cursor
                isInteractPhone = false;
                // Play animation
                anim.SetBool("InteractPhone", isInteractPhone);
                // Zoom In
            }
        }
    }

} // end monobehaviour
