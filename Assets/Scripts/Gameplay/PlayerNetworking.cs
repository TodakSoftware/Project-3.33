// Handling player networking related. Example, if the player is not ours, then ignore the scripts
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworking : MonoBehaviourPunCallbacks
{
    public List<MonoBehaviour> scriptsToIgnore = new List<MonoBehaviour>();
    public Camera mainCam;

    void Start()
    {
        if(!photonView.IsMine){
            foreach(var script in scriptsToIgnore){
                script.enabled = false;
            }
            mainCam.gameObject.SetActive(false);
        }
    }
}
