using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    [Header("Capture Human")]
    public Collider[] humanInRadiusList;
    [SerializeField] float nearbyDetectDistance = 2f;
    public LayerMask humanLayermask;

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

        // ------------------------------------- DETECT HUMAN IN RANGE UPDATE START -----------------------------------------
        humanInRadiusList = Physics.OverlapSphere(this.transform.position, nearbyDetectDistance, humanLayermask);
        if(humanInRadiusList.Length > 0 && !GameManager.instance.gameEnded){
            if(humanInRadiusList[0].gameObject.GetComponent<Human>().fearLevel >= 100){
                if(!GetComponent<PlayerUI>().captureTextUI.activeSelf){
                    GetComponent<PlayerUI>().captureTextUI.SetActive(true);
                }
                
                if(Input.GetButtonDown("Interact")){
                    humanInRadiusList[0].gameObject.GetComponent<Human>().photonView.RPC("Captured", humanInRadiusList[0].gameObject.GetPhotonView().Owner);

                    if(GetComponent<PlayerUI>().captureTextUI.activeSelf){
                        GetComponent<PlayerUI>().captureTextUI.SetActive(false);
                    }
                }
            }
        }else{
            if(GetComponent<PlayerUI>().captureTextUI.activeSelf){
                GetComponent<PlayerUI>().captureTextUI.SetActive(false);
            }
        }
        // ------------------------------------- DETECT HUMAN IN RANGE UPDATE END -----------------------------------------
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position, nearbyDetectDistance);
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
