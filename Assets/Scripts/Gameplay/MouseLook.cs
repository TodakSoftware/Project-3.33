using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] float mouseSensitivity;
    float mouseMultiplier = 100f;
    bool isGhost;
    [SerializeField] Transform playerBody;
    [SerializeField] float limitLookMin, limitLookMax;

    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    
        // Check if we are human or ghost
        if(PlayerMovement.instance != null){
            isGhost = false;
        }else{
            isGhost = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGhost){ // if Player, use this
            if(PlayerMovement.instance.enableMouseLook){
                float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * mouseMultiplier * Time.deltaTime;
                float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * mouseMultiplier * Time.deltaTime;
                
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, limitLookMin, limitLookMax);
                
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }else{ // if Ghost, use this
            if(playerBody.GetComponent<GhostMovement>().enableMouseLook){
                float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * mouseMultiplier * Time.deltaTime;
                float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * mouseMultiplier * Time.deltaTime;
                
                xRotation -= mouseY;
                xRotation = Mathf.Clamp(xRotation, limitLookMin, limitLookMax);
                
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * mouseX);
            }
        }
        
    }
}
