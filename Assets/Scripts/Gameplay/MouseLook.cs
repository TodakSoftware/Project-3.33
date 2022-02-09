using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MouseLook : MonoBehaviourPunCallbacks
{
    [SerializeField] float mouseSensitivity;
    [HideInInspector] public PlayerMovement playerMvmt;
    float mouseMultiplier = 100f;
    bool isGhost;
    [SerializeField] Transform playerBody;
    [SerializeField] float limitLookMin, limitLookMax;

    float xRotation = 0f;
    // Start is called before the first frame update
    void Start()
    {
            // Check if we are human or ghost
            if(photonView.IsMine){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if(PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString() == "human"){
                    isGhost = false;
                }else{
                    isGhost = true;
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine){
            if(!isGhost){ // if Player, use this
                if(playerMvmt.enableMouseLook){
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
        } //end ismine
    }
}
