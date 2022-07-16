using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInventory : MonoBehaviourPunCallbacks
{
    public int maxSlot = 2;
    public List<string> ritualItemLists = new List<string>();
    public List<string> propItems = new List<string>();

    [PunRPC]
    public void AddRitualItem(string code){
        if(ritualItemLists.Count < maxSlot){
            ritualItemLists.Add(code);
        }else{
            print("Inventory Full!");
        }
    } // end AddItem

    public void AddPropItem(string code){
        if(propItems.Count < maxSlot){
            propItems.Add(code);
        }else{
            print("Inventory Full!");
        }
    } // end AddPropItem

    public bool IsInventoryFull(){
        int totalItem = ritualItemLists.Count + propItems.Count;
        if(totalItem >= maxSlot){
            print("Inventory Full!");
            return true;
        }
        return false;
    } // end IsInventoryFull

    [PunRPC]
    public void ClearRitualItems(){
        if(ritualItemLists.Count > 0){
            ritualItemLists.Clear();
        }
    } // end ClearRitualItems
}
