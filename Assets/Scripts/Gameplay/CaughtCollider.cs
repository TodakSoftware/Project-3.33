using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CaughtCollider : MonoBehaviourPunCallbacks
{
    public GameObject ghostGO;
    float scareDuration = 2f;

    void OnTriggerEnter(Collider other) {
        if(!GameManager.instance.gameEnded && other.CompareTag("Player") && !other.gameObject.GetComponent<Human>().isScared && !other.gameObject.GetComponent<Human>().isCaptured){
            other.gameObject.GetComponent<Human>().photonView.RPC("Scared", other.gameObject.GetPhotonView().Owner, scareDuration, 35); // 2f = Scared Duration (maybe set as abilities), 35 = Fear Amount Added
            gameObject.SetActive(false);
        }
    }
}
