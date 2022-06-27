using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobilePhone : MonoBehaviour
{
    [Header("Enabler")]
    public bool enableDrain;

    [Header("Apps Related")]
    public SO_Apps appsSO;
    public List<string> currentApps = new List<string>();
    public List<string> soldApps = new List<string>();

    [Header("Battery")]
    public TextMeshProUGUI batteryPercentText;
    public Image imageBatteryCase, imageBatteryBar;
    [SerializeField] Sprite spriteBatCaseOK, spriteBatCaseEmpty;
    [SerializeField][Range(0f,103f)] float currentBattery;
    [SerializeField]float drainRateTotal;

    [Header("Panel Related")]
    public GameObject shopPanel;
    public GameObject quickslotPanel;

    [Header("Shop Related")]
    public GameObject shopBtnPrefab;
    public Transform shopAppsContent;
    public TextMeshProUGUI shopCreditsText;

    [Header("Clock Timer")]
    public TextMeshProUGUI hpClockText;

    [Header("Main Phone Related")]
    [SerializeField]bool phoneIsDead;
    public Canvas phoneCanvas;
    public GameObject phoneLight;
    public GameObject appsPrefab;
    public Transform menuAppsContent;
    public bool isLandscape;
    public Human humanRef; // who own the mobile phone?

    void Start(){
        phoneCanvas.worldCamera = humanRef.cameraGO.GetComponent<Camera>(); // Set phoneCanvas with current world camera

        // Add Preinstalled apps
        if(appsSO.appLists.Count > 0){
            foreach(var app in appsSO.appLists){
                if(app.isSystemApp){
                    currentApps.Add(app.code);
                }
            }
        }

        // Refresh main screen
        RefreshMainScreen();
    }

    public void SwitchPhoneView(bool toLandscape){
        if(toLandscape){
            isLandscape = true;
        }else{
            isLandscape = false;
        }
        GetComponent<Animator>().SetBool("Landscape", toLandscape);
    } // end SwitchPhoneView()

    public void RefreshMainScreen(){ // Refresh main screen apps
        // Clear any existing content + currentAppList
        if(menuAppsContent.childCount > 0){
            foreach(Transform c in menuAppsContent){
                Destroy(c.gameObject);
            }
        }

        // Shown on phone
        if(currentApps.Count > 0){
            foreach(var code in currentApps){
                foreach(var apps in appsSO.appLists){
                    if(code == apps.code){
                        var ap = Instantiate(appsPrefab);
                        ap.GetComponent<UI_Apps>().appCode = apps.code;
                        ap.GetComponent<UI_Apps>().appIcon.sprite = apps.appIcon;
                        ap.GetComponent<UI_Apps>().appName.text = apps.name;
                        ap.transform.SetParent(menuAppsContent, false);
                    }
                } // end foreach appSO
            } // end foreach currentApps
        } // end count > 0

    } // end RefreshMainScreen()
}
