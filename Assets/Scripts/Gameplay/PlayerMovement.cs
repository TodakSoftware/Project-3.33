using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
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
    public SO_Characters charactersSO;

    [Title("Data Index")]
    public int charIndex;

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
    [HideInInspector] public MobilePhone mobilePhone;
    public GameObject hpPrefab;
    

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

        if(Input.GetKeyDown(KeyCode.Tab)){
            if(!selectAppMode){
                selectAppMode = true;
                animator.SetBool("Interact", true);
            }else{
                selectAppMode = false;
                animator.SetBool("Interact", false);
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void SetupPlayerMesh(){
        // Spawn Model
        if(spawnFullbody){
            playerModel = Instantiate (charactersSO.characterLists[charIndex].fullbodyPrefab, new Vector3(thirdModelGO.transform.position.x,thirdModelGO.transform.position.y,thirdModelGO.transform.position.z), Quaternion.identity);
            playerModel.transform.parent = thirdModelGO.transform;
            playerModel.GetComponent<MeshProperty>().player = this;

            thirdModelGO.transform.localPosition = new Vector3(thirdModelGO.transform.localPosition.x, -charactersSO.characterLists[charIndex].cameraHeight, thirdModelGO.transform.localPosition.z);
        }else if(spawnHandOnly){
            playerModel = Instantiate (charactersSO.characterLists[charIndex].handPrefab, new Vector3(firstModelGO.transform.position.x,firstModelGO.transform.position.y,firstModelGO.transform.position.z), Quaternion.identity);
            playerModel.transform.parent = firstModelGO.transform;
            playerModel.GetComponent<MeshProperty>().player = this;

            firstModelGO.transform.localPosition = new Vector3(firstModelGO.transform.localPosition.x, -charactersSO.characterLists[charIndex].cameraHeight, firstModelGO.transform.localPosition.z);
        }

        // Spawn Mobile
        var hp = Instantiate(hpPrefab);
        hp.transform.parent = playerModel.GetComponent<MeshProperty>().itemHandlerGO.transform;
        hp.transform.localRotation = Quaternion.identity;
        hp.transform.localPosition = Vector3.zero;

        mobilePhone = hp.GetComponent<MobilePhone>();
        mobilePhone.player = this;

        // Set player look at on MeshProperty
        playerModel.GetComponent<MeshProperty>().toLookAt = toLookGO;

        // Setup camera height for fps onl
        mouseLook.gameObject.transform.localPosition = new Vector3(mouseLook.gameObject.transform.localPosition.x, charactersSO.characterLists[charIndex].cameraHeight, mouseLook.gameObject.transform.localPosition.z);
    }

    public void SwitchPhoneLandscape(bool val){
        if(mobilePhone != null){
            if(val){
                mobilePhone.gameObject.transform.parent = playerModel.GetComponent<MeshProperty>().itemHandlerGO2.transform;
                mobilePhone.gameObject.transform.localRotation = Quaternion.identity;
                mobilePhone.gameObject.transform.localPosition = Vector3.zero;
                mobilePhone.gameObject.GetComponent<MobilePhone>().ChangeLandscape(true);
            }else{
                mobilePhone.gameObject.transform.parent = playerModel.GetComponent<MeshProperty>().itemHandlerGO.transform;
                mobilePhone.gameObject.transform.localRotation = Quaternion.identity;
                mobilePhone.gameObject.transform.localPosition = Vector3.zero;
                mobilePhone.gameObject.GetComponent<MobilePhone>().ChangeLandscape(false);
            }
        }
    }

    public void PhoneZoomIn(bool val){
        if(val){
            cam.DOFieldOfView(25f, .25f);
            cam.gameObject.transform.DOLocalMoveX(-0.05f, .3f);
            cam.gameObject.transform.DOLocalMoveY(-0.069f, .3f);

            enableMouseLook = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }else{
            cam.DOFieldOfView(65f, .25f);
            cam.gameObject.transform.DOLocalMoveX(0f, .3f);
            cam.gameObject.transform.DOLocalMoveY(0f, .3f);

            enableMouseLook = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}
