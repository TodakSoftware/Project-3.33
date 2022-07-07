using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CaughtCollider : MonoBehaviourPunCallbacks
{
    public GameObject ghostGO;
    bool isTriggered;
    float scareDuration = 5f;

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !other.gameObject.GetComponent<Human>().isScared && !other.gameObject.GetComponent<Human>().isCaptured){
            if(!isTriggered){
                isTriggered = true;
                other.gameObject.GetComponent<Human>().photonView.RPC("Scared", other.gameObject.GetPhotonView().Owner, scareDuration, 35); // 2f = Scared Duration (maybe set as abilities), 35 = Fear Amount Added
                Invoke("ResetIsTriggered", scareDuration);
            }
        }
    }

    void ResetIsTriggered(){
        isTriggered = false;
    }
}
