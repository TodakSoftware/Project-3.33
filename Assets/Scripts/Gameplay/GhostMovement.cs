using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

public class GhostMovement : MonoBehaviour
{
    CharacterController controller;
    MouseLook mouseLook;
    public GameObject toLookGO;
    [HideInInspector] public Camera cam;
    bool selectAppMode;

    [Title("Enable Setting")]
    public bool enableMove = true;
    public bool enableJump = true;
    public bool enableMouseLook = true;

    [Title("Spawner")]
    public bool spawnFullbody = false;
    public bool spawnHandOnly = true;

    [Title("Databases")]
    public SO_Ghosts ghostsSO;

    [Title("Data Index")]
    public int ghostIndex;

    [Title("Movement Setting")]
    public float baseSpeed = 3f;
    public float slowMultiplier = 2f;
    public float runMultiplier = 2f;
    float currentSpeed;
    Animator animator;
    
    // -------------------------------------------
    [Title("Ground Check")]
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.1f;
    float gravity = -9.81f;
    // ----------------------------------------------
    [Title("Jump Setting")]
    [SerializeField] float jumpHeight = .5f;
    Vector3 velocity;
    // -------------------------------------------
    [Title("Mesh Setting")]
    [SerializeField] GameObject thirdModelGO; // in player prefab, For instantiated prefab to become a child
    [SerializeField] GameObject firstModelGO; // in player prefab, For instantiated prefab to become a child
    GameObject playerModel;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mouseLook = GetComponentInChildren<MouseLook>();
        cam = mouseLook.gameObject.transform.GetChild(0).GetComponent<Camera>();

        SetupPlayerMesh();

        // Get Animator
        if(playerModel != null)
            animator = playerModel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0, -.05f, 0f), groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");   // W & S
        float z = Input.GetAxis("Vertical");   // A & D

        Vector3 move = transform.forward * z + transform.right * x;
        move = Vector3.ClampMagnitude(move, 1f);

        if(z < -0.5f){  // if walk backward
            currentSpeed = baseSpeed / slowMultiplier;
        }else if(z > 0.5f){    // if walk forward
            currentSpeed = baseSpeed; 
        }else if(x > 0.5f || x < -0.5f){   // if strafe
            currentSpeed = baseSpeed / slowMultiplier; 
        }

        animator.SetFloat("Velocity Z", z);
        animator.SetFloat("Velocity X", x);

        controller.Move(move * currentSpeed * Time.deltaTime);

        if(!isGrounded){
            velocity.y += gravity * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && isGrounded && enableJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void SetupPlayerMesh(){
        // Spawn Model
        if(spawnFullbody){
            playerModel = Instantiate (ghostsSO.ghostLists[ghostIndex].fullbodyPrefab, new Vector3(thirdModelGO.transform.position.x,thirdModelGO.transform.position.y,thirdModelGO.transform.position.z), Quaternion.identity);
            playerModel.transform.parent = thirdModelGO.transform;
            
            //thirdModelGO.transform.localPosition = new Vector3(thirdModelGO.transform.localPosition.x, -ghostsSO.ghostLists[ghostIndex].cameraHeight, thirdModelGO.transform.localPosition.z);
        }else if(spawnHandOnly){
            playerModel = Instantiate (ghostsSO.ghostLists[ghostIndex].handPrefab, new Vector3(firstModelGO.transform.position.x,firstModelGO.transform.position.y,firstModelGO.transform.position.z), Quaternion.identity);
            playerModel.transform.parent = firstModelGO.transform;

            firstModelGO.transform.localPosition = new Vector3(firstModelGO.transform.localPosition.x, -ghostsSO.ghostLists[ghostIndex].cameraHeight, ghostsSO.ghostLists[ghostIndex].cameraDepth);
        }


        // Set player look at on MeshProperty
        playerModel.GetComponent<MeshProperty>().toLookAt = toLookGO;

        // Setup camera height for fps onl
        mouseLook.gameObject.transform.localPosition = new Vector3(mouseLook.gameObject.transform.localPosition.x, ghostsSO.ghostLists[ghostIndex].cameraHeight, mouseLook.gameObject.transform.localPosition.z);
    }

}
