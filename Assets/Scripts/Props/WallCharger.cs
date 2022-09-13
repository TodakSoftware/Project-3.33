using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;

public class WallCharger : MonoBehaviour
{
    Interactable interact;

    public float wallChargerPercent = 100f;
    public float currentCharge;

    public bool isBatteryAvailable;
    public Vector2 maxChargeValue = new Vector2 (50, 100);

    // Start is called before the first frame update
    void Start()
    {
        wallChargerPercent = Random.Range(maxChargeValue.x, maxChargeValue.y);
        currentCharge =  Mathf.Round(wallChargerPercent);
        GetComponent<Interactable>().holdDuration = Mathf.Round(wallChargerPercent/10f);
        isBatteryAvailable = true;
    }

    public void AdjustCurrentCharger (float amount)
    {
       
       Debug.Log(currentCharge + "% Left");

       if(currentCharge <= 0.5f) //ngak tau sih ngapa jadi begini
       {
        currentCharge = 0.0f;
        Debug.Log("Charger Empty");
        isBatteryAvailable = false;
       } else {
        currentCharge -= amount;
       }
    }

    
}
