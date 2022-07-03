using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    public SO_RitualItem ritualItemSO;
    public List<GameObject> itemSpawnpoints = new List<GameObject>();
    public List<Transform> meshSpawnpoints = new List<Transform>(); // for displaying mesh on cup
    public List<string> itemLists = new List<string>(); // Link with SO Game Settings total ritual item
    [SerializeField] bool inRange;
    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")){
            inRange = true;
            other.GetComponent<PlayerInteraction>().canContributeItem = true;
            other.GetComponent<PlayerInteraction>().altarRef = this;
        }
    } // end OnTriggerEnter

    void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")){
            inRange = false;
            other.GetComponent<PlayerInteraction>().canContributeItem = false;
            other.GetComponent<PlayerInteraction>().altarRef = null;
        }
    } // end OnTriggerExit

    public void ContributeRitualItem(List<string> itemCodes){
        if(inRange){
            foreach(var item in itemCodes){
                if(itemLists.Count < 5){
                    // Add to list
                    itemLists.Add(item);
                }
            }

            DisplayItemOnAltar();
        } // end inRange
    } // end ContributeRitualItem

    void DisplayItemOnAltar(){
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

        if(itemLists.Count >= 5){
            print("Human WIN!");
        }
    } // end DisplayItemOnAltar
}
