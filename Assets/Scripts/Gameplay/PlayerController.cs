using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float currentSpeed; // Realtime walk speed value (For debugging) Do Not Change this
    [SerializeField] float movementSpeed = 2f; // Actual speed for movement
    [SerializeField] float jumpSpeed = .5f; // Jump height
    [SerializeField] float runMultiplier = 1; // Multiplier for running
    float gravity = -9.81f; // Ground check (For debugging) Do Not Change this
    Vector3 velocity; 
    CharacterController characterController;

    // ANIMATION RELATED
    Animator anim;
    float inputVertical, inputHorizontal, smoothVer, smoothHor;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleGravity();

        if(Input.GetButton("Jump") && characterController.isGrounded){
            HandleJumping();
        }
    }

    void HandleGravity(){
        if(characterController.isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
        
        // Handle Gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleMovement(){
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");

        if(inputVertical < -0.5f){  // if walk backward
            currentSpeed = movementSpeed / 1.4f;
        }else if(inputVertical > 0.5f){    // if walk forward
            currentSpeed = movementSpeed; 
        }else if(inputHorizontal > 0.5f || inputHorizontal < -0.5f){   // if strafe
            currentSpeed = movementSpeed / 1.4f;
        }

        Vector3 movement = transform.right * inputHorizontal + transform.forward * inputVertical;
        movement = Vector3.ClampMagnitude(movement, 1f);

        characterController.Move(movement * currentSpeed * Time.deltaTime);

        if(Input.GetButton("Sprint")){
            characterController.Move(movement * Time.deltaTime * runMultiplier);
            anim.speed = runMultiplier + 0.3f; // additional 0.3f to match with walk anim (speedup)
        }else if(Input.GetButtonUp("Sprint")){
            anim.speed = 1; // revert back to normal speed animation
        }
        

        // ANIMATION RELATED
        SmoothAnimation();
        
        anim.SetFloat("Vertical", smoothVer);
        anim.SetFloat("Horizontal", smoothHor);
    }

    void HandleJumping(){
        velocity.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
    }

    void SmoothAnimation(){ // Smoothing animation movement value by inc/dec value by 0.08f per second instead snap
        // Smooth Vertical
        if(inputVertical > 0){
            smoothVer += 0.08f;
            if(smoothVer >= 1){
                smoothVer = 1;
            }
        }else if(inputVertical < 0){
            smoothVer -= 0.08f;
            if(smoothVer <= -1){
                smoothVer = -1;
            }
        }else{
            if(smoothVer > 0){
                smoothVer -= 0.08f;
                if(smoothVer <= 0){
                    smoothVer = 0;
                }
            }else if(smoothVer < 0){
                smoothVer += 0.08f;
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

} // end monobehaviour
