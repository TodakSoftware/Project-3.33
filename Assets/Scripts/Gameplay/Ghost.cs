using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Ghost : MonoBehaviourPunCallbacks
{
    PlayerController playerController;
    public List<GameObject> meshToHide = new List<GameObject>();
    bool isAttacking = false;
    public bool isInvisible;

    void Awake(){
        playerController = GetComponent<PlayerController>();
    }
    
    void Start()
    {
        if(photonView.IsMine){
            foreach(var mesh in meshToHide){
                mesh.SetActive(false);
            }

            photonView.RPC("SetInvisible", RpcTarget.Others, true);
        }
    }

    [PunRPC]
    public void SetInvisible(bool invisible){
        if(invisible){
            playerController.playerMesh.SetActive(false);
        }else{
            playerController.playerMesh.SetActive(true);
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
        photonView.RPC("SetInvisible", RpcTarget.Others, false);
        playerController.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        isAttacking = false;

        // Hide Mesh
        yield return new WaitForSeconds(3f);
        photonView.RPC("SetInvisible", RpcTarget.Others, true);
    }
}
