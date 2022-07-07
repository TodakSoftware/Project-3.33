using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Ghost : MonoBehaviourPunCallbacks
{
    PlayerController playerController;
    public List<GameObject> meshToHide = new List<GameObject>();
    bool isAttacking = false;
    [SerializeField] bool canAttacking;
    [SerializeField] float attackCooldown = 5f;
    float attackTimer;
    [SerializeField] bool isInvisible;
    public GameObject caughtCollider;

    void Awake(){
        playerController = GetComponent<PlayerController>();
    }
    
    void Start()
    {
        if(photonView.IsMine){
            foreach(var mesh in meshToHide){
                mesh.SetActive(false);
            }

            if(!isInvisible){
                photonView.RPC("SetInvisible", RpcTarget.Others, true);
            }
        }
    }

    [PunRPC]
    public void SetInvisible(bool invisible){
        if(invisible){
            isInvisible = true;
            playerController.playerMesh.SetActive(false);
        }else{
            isInvisible = false;
            playerController.playerMesh.SetActive(true);
        }
    }

    void Update(){
        if(!photonView.IsMine){
            return;
        }

        if(Input.GetMouseButtonDown(0) && canAttacking){
            if(!isAttacking){
                StartCoroutine(Attack());
            }
        }

        if(attackTimer > 0){
            attackTimer -= Time.deltaTime;
        }else{
            attackTimer = 0;
            canAttacking = true;
        }
    }

    IEnumerator Attack(){
        isAttacking = true;
        canAttacking = false;
        attackTimer = attackCooldown;
        photonView.RPC("SetInvisible", RpcTarget.Others, false);
        playerController.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        isAttacking = false;

        // Hide Mesh
        yield return new WaitForSeconds(3f);
        if(!isInvisible){
            photonView.RPC("SetInvisible", RpcTarget.Others, true);
        }
    }

    public void EnableCaughtCollider(){
        caughtCollider.SetActive(true);
    }

    public void DisableCaughtCollider(){
        caughtCollider.SetActive(false);
    }
}
