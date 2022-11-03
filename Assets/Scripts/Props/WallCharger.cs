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

    public float wallChargerPercent = 103f;
    public float currentCharge;

    public bool isBatteryAvailable;
    public Vector2 maxChargeValue = new Vector2 (50, 103);

    [SerializeField] private WallChargeBar _chargebar;

    public GameObject chargebarCanvas;

    public bool isCharge;


    // Start is called before the first frame update
    void Start()
    {
       
        wallChargerPercent = Random.Range(maxChargeValue.x, maxChargeValue.y);
        currentCharge =  Mathf.Round(wallChargerPercent);
        _chargebar.UpdateBatteryCharge(103f,currentCharge);


        GetComponent<Interactable>().holdDuration = Mathf.Round(wallChargerPercent/10f);
        
        isBatteryAvailable = true;

        //isCharge = false;
    }

    void Update(){

        _chargebar.UpdateBatteryCharge(103f,currentCharge);
        //seeCharge();
        //isCharge = false;

    }

    public void AdjustCurrentCharger (float amount)
    {
       
       Debug.Log(currentCharge + "% Left");

        if (!chargebarCanvas.activeSelf){
            chargebarCanvas.SetActive(true);
       } 

       isCharge = true;

       if(currentCharge < 1f) //ngak tau sih ngapa jadi begini
       {
        currentCharge = 0.0f;
        Debug.Log("Charger Empty");
        isBatteryAvailable = false;
       } else {
        currentCharge += amount;
        
       }
    }

    public void SetNewHoldDuration(){
        GetComponent<Interactable>().holdDuration = Mathf.Round(currentCharge/10f);
        
    }

  /*   public void seeCharge(){

        if (isCharge == true)
        {
            chargebarCanvas.SetActive(true);
        }
        else
        {
            chargebarCanvas.SetActive(false);
        }

    } */

    
    }
