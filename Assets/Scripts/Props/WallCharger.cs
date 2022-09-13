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

    public float keyhold;
   
    
    
    private void Awake()
    {
       interact = GetComponent<Interactable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        keyhold = interact.GetComponent<Interactable>().holdDuration;

        //wallChargerPercent = Random.Range(50, 100);
        currentCharge = wallChargerPercent;
        isBatteryAvailable = true;
    }

    

    public void AdjustCurrentCharger (float amount)
    {
       currentCharge -= amount;
       
       Debug.Log(currentCharge + "% Left");

       if(currentCharge < 0.1f)
       {
        currentCharge = 0.0f;
        Debug.Log("Charger Empty");
       }
    }

    
}
