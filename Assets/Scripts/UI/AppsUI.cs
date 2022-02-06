using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class AppsUI : MonoBehaviour
{
    [Title("Apps Details")]
    public string appCode;
    public Image appIcon;
    public TextMeshProUGUI appName;

    public void AppClicked(){
        print("Run Apps: " + appName.text);
        RunApps(appCode);
    }
    
    void RunApps(string code){
        switch(code){
            case "A001": // Flashlight
                PlayerAbilities.instance.ToggleFlashlight();
                PlayerMovement.instance.HandleSelectApp();
            break;

            case "A002": // Shop
                MobilePhone.instance.OpenShopApp(true);
            break;

            case "A003": // Quickslot
                MobilePhone.instance.OpenQuickslotApp(true);
            break;

            case "A004": // Thermal Camera
                PlayerAbilities.instance.ToggleThermalVision();
                PlayerMovement.instance.HandleSelectApp();
            break;

            case "A005": // Night Vision
                PlayerAbilities.instance.ToggleNightVision();
                PlayerMovement.instance.HandleSelectApp();
            break;
        }
    }
}
