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

    [Title("Enable Setting")]
    public bool enableMove = true;
    public bool enableJump = true;
    public bool enableMouseLook = true;

    [Title("Spawner")]
    public bool spawnFullbody = true;
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
    GameObject fullbodyModel;
    GameObject handModel;
    [HideInInspector] public MobilePhone mobilePhone;
    [HideInInspector] public MobilePhone mobilePhone2;
    public GameObject hpPrefab;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mouseLook = GetComponentInChildren<MouseLook>();
        cam = mouseLook.gameObject.transform.GetChild(0).GetComponent<Camera>();

        SetupPlayerMesh();

        // Get Animator
        if(handModel != null)
            animator = handModel.GetComponent<Animator>();
        else if(fullbodyModel != null)
            animator = fullbodyModel.GetComponent<Animator>();
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

        if(Input.GetKeyDown(KeyCode.K)){
            animator.SetBool("Interact", true);
        }

        if(Input.GetKeyDown(KeyCode.L)){
            animator.SetBool("Interact", false);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void SetupPlayerMesh(){
        // Fullbody
        if(spawnFullbody){
            fullbodyModel = Instantiate (charactersSO.characterLists[charIndex].fullbodyPrefab, new Vector3(thirdModelGO.transform.position.x,thirdModelGO.transform.position.y,thirdModelGO.transform.position.z), Quaternion.identity);
            fullbodyModel.transform.parent = thirdModelGO.transform;
            fullbodyModel.GetComponent<MeshProperty>().player = this;
    
            var hpPlaceholder = Instantiate(hpPrefab);
            hpPlaceholder.transform.parent = fullbodyModel.GetComponent<MeshProperty>().itemHandlerGO.transform;
            hpPlaceholder.transform.localRotation = Quaternion.identity;
            hpPlaceholder.transform.localPosition = Vector3.zero;
            hpPlaceholder.GetComponent<MobilePhone>().enabled = false;

            mobilePhone = hpPlaceholder.GetComponent<MobilePhone>();

            fullbodyModel.GetComponent<MeshProperty>().toLookAt = toLookGO;
        }
        
        
        // Hand only
        if(spawnHandOnly){
            handModel = Instantiate (charactersSO.characterLists[charIndex].handPrefab, new Vector3(firstModelGO.transform.position.x,firstModelGO.transform.position.y,firstModelGO.transform.position.z), Quaternion.identity);
            handModel.transform.parent = firstModelGO.transform;
            handModel.GetComponent<MeshProperty>().player = this;
    
            // Spawn Mobile Phone
            var hpPlaceholder2 = Instantiate(hpPrefab);
            hpPlaceholder2.GetComponent<MobilePhone>().player = this;
            hpPlaceholder2.transform.parent = handModel.GetComponent<MeshProperty>().itemHandlerGO.transform;
            hpPlaceholder2.transform.localRotation = Quaternion.identity;
            hpPlaceholder2.transform.localPosition = Vector3.zero;

            mobilePhone2 = hpPlaceholder2.GetComponent<MobilePhone>();

            handModel.GetComponent<MeshProperty>().toLookAt = toLookGO;
        }

        // Setup camera height
        mouseLook.gameObject.transform.localPosition = new Vector3(mouseLook.gameObject.transform.localPosition.x, charactersSO.characterLists[charIndex].cameraHeight, mouseLook.gameObject.transform.localPosition.z);
        firstModelGO.transform.localPosition = new Vector3(firstModelGO.transform.localPosition.x, -charactersSO.characterLists[charIndex].cameraHeight, firstModelGO.transform.localPosition.z);
    }

    public void SwitchPhoneLandscape(){
        if(mobilePhone != null){
            mobilePhone.gameObject.transform.parent = fullbodyModel.GetComponent<MeshProperty>().itemHandlerGO2.transform;
            mobilePhone.gameObject.transform.localRotation = Quaternion.identity;
            mobilePhone.gameObject.transform.localPosition = Vector3.zero;
        }

        if(mobilePhone2 != null){
            mobilePhone2.gameObject.transform.parent = handModel.GetComponent<MeshProperty>().itemHandlerGO2.transform;
            mobilePhone2.gameObject.transform.localRotation = Quaternion.identity;
            mobilePhone2.gameObject.transform.localPosition = Vector3.zero;
        }
    }

    public void SwitchPhonePotrait(){
        if(mobilePhone != null){
            mobilePhone.gameObject.transform.parent = fullbodyModel.GetComponent<MeshProperty>().itemHandlerGO.transform;
            mobilePhone.gameObject.transform.localRotation = Quaternion.identity;
            mobilePhone.gameObject.transform.localPosition = Vector3.zero;
        }
        
        if(mobilePhone2 != null){
            mobilePhone2.gameObject.transform.parent = handModel.GetComponent<MeshProperty>().itemHandlerGO.transform;
            mobilePhone2.gameObject.transform.localRotation = Quaternion.identity;
            mobilePhone2.gameObject.transform.localPosition = Vector3.zero;
        }
    }

    public void PhoneZoomIn(){
        //mouseLook.gameObject.transform.DOLocalMoveZ(0.13f, .3f);
        cam.DOFieldOfView(30f, .3f);
        cam.gameObject.transform.DOLocalMoveX(-0.02f, .3f);
        cam.gameObject.transform.DOLocalMoveY(-0.06f, .3f);
    }

    public void PhoneZoomOut(){
        //mouseLook.gameObject.transform.DOLocalMoveZ(0f, .3f);
        cam.DOFieldOfView(50f, .3f);
        cam.gameObject.transform.DOLocalMoveX(0f, .3f);
        cam.gameObject.transform.DOLocalMoveY(0f, .3f);
    }
}
