using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ritual_Item : MonoBehaviour
{
    public SO_Ritual_Item ritualSO;
    public string itemCode;
    public string itemName;
    public UnityEvent onInteract;

    void Start(){
        foreach(var i in ritualSO.itemLists){
            if(i.code == itemCode){
                itemName =  i.name;
            }
        }
    }

    public void AddItemToSlot(){
        
        if(itemCode != ""){
            PlayerManager.instance.itemSlot = itemCode;

            foreach(var i in ritualSO.itemLists){
                if(i.code == itemCode){
                    print("Added into player item slot: "+ i.name);
                    PlayerHUD.instance.SetItemSlot();
                }
            }
            
        }else{
            print("Empty Item Code");
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 1f);
    }
}
