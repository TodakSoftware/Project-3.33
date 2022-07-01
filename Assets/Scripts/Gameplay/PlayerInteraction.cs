using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public Transform cam;
    public float interactDistance;
    bool interactable;

    void Update(){
        RaycastHit hit;
        interactable = Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, interactDistance);

        if(interactable && Input.GetButtonDown("Interact")){
            print(hit.collider.gameObject.name);
        }
    }
}
