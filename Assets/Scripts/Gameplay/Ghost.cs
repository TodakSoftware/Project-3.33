using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ghost : MonoBehaviourPunCallbacks
{
    PlayerController playerController;
    public List<GameObject> meshToHide = new List<GameObject>();
    bool isAttacking = false;

    void Awake(){
        playerController = GetComponent<PlayerController>();
    }
    
    void Start()
    {
        if(photonView.IsMine){
            foreach(var mesh in meshToHide){
                mesh.SetActive(false);
            }
        }
    }

    void Update(){
        if(!photonView.IsMine){
            return;
        }

        if(Input.GetMouseButtonDown(0)){
            if(!isAttacking){
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack(){
        isAttacking = true;
        playerController.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
}
