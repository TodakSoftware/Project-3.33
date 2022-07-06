using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopupCredits : MonoBehaviour
{
    public int creditAmount;

    public void AddPhoneCredit(){
        print("Add " + creditAmount + " to " + GameManager.instance.playerOwned);
        GameManager.instance.playerOwned.GetComponent<Human>().playerMoney += creditAmount;
        // Popup notification..
        Destroy(gameObject);
    }
}
