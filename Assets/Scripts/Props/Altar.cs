using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Altar : MonoBehaviourPunCallbacks
{
    public SO_RitualItem ritualItemSO;
    public List<GameObject> itemSpawnpoints = new List<GameObject>();
    public List<Transform> meshSpawnpoints = new List<Transform>(); // for displaying mesh on cup
    public List<string> itemLists = new List<string>(); // Link with SO Game Settings total ritual item


    [PunRPC]
    public void addRitualItems(string code){
        if(!itemLists.Contains(code)){
            itemLists.Add(code);
            DisplayItemOnAltar();
        }
    }

    //[PunRPC]
    public void DisplayItemOnAltar(){
        int itemIndex = 0;
        foreach(var item in itemLists){
            foreach(var ritual in ritualItemSO.ritualItemLists){
                if(ritual.code == item){
                    itemSpawnpoints[itemIndex].SetActive(true);
                    // Instantiate ritual item
                    if(ritual.itemMesh != null){
                        var mesh = Instantiate(ritual.itemMesh, Vector3.zero, ritual.itemMesh.transform.rotation); //Quaternion.Euler(0, -90, 0)
                        mesh.transform.SetParent(meshSpawnpoints[itemIndex],false);
                    }
                    itemIndex += 1;
                }
            }
        } // end foreach

        // Save ItemContributed in CustomProperties Here
        //

        if(itemLists.Count >= (int)PhotonNetwork.CurrentRoom.CustomProperties["GameTotalRitualItem"]){
            print("Human WIN!");
            GameManager.instance.photonView.RPC("HumanWin", RpcTarget.All, true);
        }
    } // end DisplayItemOnAltar
}
