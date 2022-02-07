using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ritual_Altar : MonoBehaviour
{
    public List<string> itemSlot = new List<string>();
    public List<GameObject> tempItem = new List<GameObject>();
    public bool humanInArea;

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            humanInArea = true;
            other.GetComponent<PlayerManager>().canTransferItem = true;
            other.GetComponent<PlayerManager>().altar = this;
            other.GetComponent<Interactor>().pickupText.text = "[E] Put Item";
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")){
            humanInArea = false;
            other.GetComponent<PlayerManager>().canTransferItem = false;
            other.GetComponent<PlayerManager>().altar = null;
            other.GetComponent<Interactor>().pickupText.text = "";
        }
    }

    public void TempShowItem(){
        foreach(var g in tempItem){
            g.SetActive(false);
        }

        for(var i = 0; i < itemSlot.Count; i++){
            tempItem[i].SetActive(true);
        }

        PlayerManager.instance.totalContributed = itemSlot.Count;
    }
}
