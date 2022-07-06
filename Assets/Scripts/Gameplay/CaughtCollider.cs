using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CaughtCollider : MonoBehaviourPunCallbacks
{
    public GameObject ghostGO;
    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !other.gameObject.GetComponent<Human>().isScared){
            other.gameObject.GetComponent<Human>().AdjustFearLevel(35);
            StartCoroutine(other.gameObject.GetComponent<Human>().Scared(1f));
        }
    }
}
