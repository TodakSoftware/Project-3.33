using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
    [SerializeField] GameObject modelGO; // in player prefab
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

        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0, -.05f, 0f), groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * z + transform.right * x;
        move = Vector3.ClampMagnitude(move, 1f);

        if(z < 0){
            currentSpeed = baseSpeed / slowMultiplier;
        }else if(z > 0){
            currentSpeed = baseSpeed; 
        }else if(x > 0 || x < 0){
            currentSpeed = baseSpeed / slowMultiplier; 
        }

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
        // Fullbody
        if(spawnFullbody){
            var fullbodyModel = Instantiate (charactersSO.characterLists[charIndex].fullbodyPrefab, new Vector3(modelGO.transform.position.x,modelGO.transform.position.y,modelGO.transform.position.z), Quaternion.identity);
            fullbodyModel.transform.parent = modelGO.transform;
    
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
            var handModel = Instantiate (charactersSO.characterLists[charIndex].handPrefab, new Vector3(modelGO.transform.position.x,modelGO.transform.position.y,modelGO.transform.position.z), Quaternion.identity);
            handModel.transform.parent = modelGO.transform;
    
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
    }
}
