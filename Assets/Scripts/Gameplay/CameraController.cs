//Camera controller for x and y movement

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

    public class CameraController : MonoBehaviourPunCallbacks
    {
        public bool isEnable;
        public float Sensitivity = 10f;
        public float minPitch = -30f;
        public float maxPitch = 60f;
        public Transform parent;
        public Transform boneParent;

        private float pitch = 0f;
        [HideInInspector] public float yaw = 0f;
        [HideInInspector] public float relativeYaw = 0f;

        public GameObject headPointlight;

        new void OnEnable()
        {
            LockCursor(true);
        }

        void Start(){
            if(headPointlight != null){
                if(photonView.IsMine){
                    headPointlight.SetActive(true);
                }else{
                    headPointlight.SetActive(false);
                }
            }
        }

        void Update(){
            if(isEnable){
                parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Sensitivity);

                transform.position = boneParent.position;
                CameraRotate();
            }
        }

        void FixedUpdate()
        {
            /* if(isEnable){
                transform.position = boneParent.position;
                CameraRotate();
            } */
        }

        void CameraRotate()
        {
            //Get input to turn the cam view
            relativeYaw = Input.GetAxis("Mouse X") * Sensitivity;
            pitch -= Input.GetAxis("Mouse Y") * Sensitivity;
            yaw += Input.GetAxis("Mouse X") * Sensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        public void LockCursor(bool isLock){
            if(isLock){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }else{
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }