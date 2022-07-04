// Handling player networking related. Example, if the player is not ours, then ignore the scripts
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworking : MonoBehaviourPunCallbacks
{
    public List<MonoBehaviour> scriptsToIgnore = new List<MonoBehaviour>();
    public List<GameObject> gameObjectToDisable = new List<GameObject>();

    void Start()
    {
        if(!photonView.IsMine){
            foreach(var script in scriptsToIgnore){
                script.enabled = false;
            }

            foreach(var go in gameObjectToDisable){
                go.SetActive(false);
            }
        }
    }
}
