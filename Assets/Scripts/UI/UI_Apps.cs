using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Photon.Pun;

public class UI_Apps : MonoBehaviourPunCallbacks
{
    [Header("Apps Details")]
    public string appCode;
    public Image appIcon;
    public TextMeshProUGUI appName;
    [HideInInspector] public MobilePhone phoneRef;

    public void AppClicked(){
        print("Run Apps: " + appName.text);
        RunApps(appCode);
    }

    void RunApps(string code){
        switch(code){
            case "A001": // Flashlight
                //phoneRef.phoneOwner.GetComponent<PlayerAbilities>().ToggleFlashlight(appCode);
                phoneRef.phoneOwner.GetComponent<PlayerAbilities>().photonView.RPC("ToggleFlashlight", RpcTarget.All, appCode);
                phoneRef.phoneOwner.GetComponent<Human>().HandleInteractPhone(); // Auto Close
            break;

            case "A002": // Shop
                phoneRef.GetComponent<MobilePhone>().OpenShopApp(true);
            break;

            case "A003": // Quickslot
                phoneRef.GetComponent<MobilePhone>().OpenQuickslotApp(true);
            break;

            case "A004": // Thermal Camera
                phoneRef.phoneOwner.GetComponent<PlayerAbilities>().ToggleThermalVision(appCode);
                phoneRef.phoneOwner.GetComponent<Human>().HandleInteractPhone(); // Auto Close
            break;

            case "A005": // Night Vision
                phoneRef.phoneOwner.GetComponent<PlayerAbilities>().ToggleNightVision(appCode);
                phoneRef.phoneOwner.GetComponent<Human>().HandleInteractPhone(); // Auto Close
            break;
        }
    }
}
