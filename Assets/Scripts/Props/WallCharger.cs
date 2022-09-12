using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using DG.Tweening;


public class WallCharger : MonoBehaviour
{
    Interactable interactable;
    //[SerializeField] GameObject interactableScript;
    
    public bool isBatteryAvailable;
    public int wallChargerPercent;
    
    void Awake()
    {
       //interactable = interactableScript.GetComponent<Interactable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        wallChargerPercent = Random.Range(0, 100);
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetKeyDown(KeyCode.F)){
            ChargePhone();
        }
    }

    public void ChargePhone()
    {
        wallChargerPercent -= 10;
        Debug.Log(wallChargerPercent + " Left");
    }
}
