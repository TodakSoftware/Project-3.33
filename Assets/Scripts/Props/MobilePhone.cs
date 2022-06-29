using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    [Header("Always On Display")]
    public GameObject heartrateUI;
    public GameObject alwaysOnDisplayPanel;

    [Header("Shop Related")]
    public GameObject shopBtnPrefab;
    public Transform shopAppsContent;
    public TextMeshProUGUI shopCreditsText;
    bool shopIsOpen;

    [Header("Quickslot Related")]
    bool quickslotIsOpen;

    [Header("Clock Timer")]
    public TextMeshProUGUI hpClockText;

    [Header("Main Phone Related")]
    [SerializeField]bool phoneIsDead;
    public Canvas phoneCanvas;
    public Canvas offScreenCanvas;
    public GameObject phoneLight;
    public GameObject appsPrefab;
    public Transform menuAppsContent;
    public bool isLandscape;
    [HideInInspector] public GameObject phoneOwner; // who own the mobile phone?

    void Start(){
        phoneCanvas.worldCamera = phoneOwner.GetComponent<Human>().cameraGO.GetComponent<Camera>(); // Set phoneCanvas with current world camera

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

        // Full up the battery percentage (Just to make sure)
        currentBattery = 103f;

        // Add passive drain
        drainRateTotal += 0.01f;
    } // end Start()

    void Update(){
        if(currentBattery >= 100f){ // fake it
            batteryPercentText.text = "100%";
        }else{
            batteryPercentText.text = (int)currentBattery + "%";
        }
        
        if(currentBattery >= 80f){
            imageBatteryBar.fillAmount = 1f;
        }else if(currentBattery >= 60f && currentBattery < 80){
            imageBatteryBar.fillAmount = 0.8f;
        }else if(currentBattery >= 40f && currentBattery < 60){
            imageBatteryBar.fillAmount = 0.6f;
        }else if(currentBattery >= 20f && currentBattery < 40){
            imageBatteryBar.fillAmount = 0.4f;
            imageBatteryBar.color = Color.white;
        }else if(currentBattery >= 1f && currentBattery < 20){
            imageBatteryBar.fillAmount = 0.2f;
            imageBatteryBar.color = Color.red;
            imageBatteryCase.sprite = spriteBatCaseOK;
        }else{
            imageBatteryBar.fillAmount = 0f;
            imageBatteryCase.sprite = spriteBatCaseEmpty;
        }
        
        if(enableDrain && !phoneIsDead){
            HandleDrainCalculation();
        }
    } // end Update()

#region Drain Calculation
    void HandleDrainCalculation(){
        if(!phoneIsDead){
            currentBattery -= Time.deltaTime * drainRateTotal;

            if(currentBattery < 0){
                currentBattery = 0;
                phoneIsDead = true;

                phoneCanvas.gameObject.SetActive(false); // Close main apps
                offScreenCanvas.gameObject.SetActive(true); // Open phone off canvas
            }
        }
    } // end HandleDrainCalculation()

    public void drainBattery(bool isDrain, string appCode){
        if(isDrain){
            foreach(var app in appsSO.appLists){
                if(appCode.ToUpper() == app.code){
                    drainRateTotal += app.drainRate;
                }
            }
        }else{
            foreach(var app in appsSO.appLists){
                if(appCode.ToUpper() == app.code){
                    if(drainRateTotal <= 0){
                        drainRateTotal = 0;
                    }else{
                        drainRateTotal -= app.drainRate;
                    }
                }
            }
        } // else end
    } // end drainBattery()

#endregion // End Drain Calculation

#region Phone View Related
    public void SwitchPhoneView(bool toLandscape){
        if(toLandscape){
            isLandscape = true;
        }else{
            isLandscape = false;

            // Close all open panel
            if(shopIsOpen || quickslotIsOpen){
                OpenShopApp(false);
                OpenQuickslotApp(false);
            }
        }
        GetComponent<Animator>().SetBool("Landscape", toLandscape);
    } // end SwitchPhoneView()

#endregion // end region Phone View

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
                        ap.GetComponent<UI_Apps>().phoneRef = this;
                        ap.transform.SetParent(menuAppsContent, false);
                    }
                } // end foreach appSO
            } // end foreach currentApps
        } // end count > 0

    } // end RefreshMainScreen()


#region SHOP RELATED
    public void OpenShopApp(bool open){
        if(open){
            shopPanel.SetActive(true);
            shopIsOpen = true;
            RefreshCreditUI();

            // Clear any existing content
            if(shopAppsContent.childCount > 0){
                foreach(Transform c in shopAppsContent){
                    Destroy(c.gameObject);
                }
            }

            // List non system app
            if(appsSO.appLists.Count > 0){
                foreach(var apps in appsSO.appLists){
                    if(!apps.isSystemApp && !apps.hideInShop){ // If not system app && hideInShop is disabled, then show it to us
                        var ap = Instantiate(shopBtnPrefab);
                        ap.GetComponent<UI_ShopBtn>().appCode = apps.code;
                        ap.GetComponent<UI_ShopBtn>().appPrice = apps.price;
                        ap.GetComponent<UI_ShopBtn>().icon.sprite = apps.appIcon;
                        ap.GetComponent<UI_ShopBtn>().title.text = apps.name;
                        ap.GetComponent<UI_ShopBtn>().desc.text = apps.description;
                        ap.GetComponent<UI_ShopBtn>().price.text = "$"+apps.price;
                        ap.GetComponent<UI_ShopBtn>().phoneRef = this;
                        if(currentApps.Contains(apps.code)){
                            ap.GetComponent<UI_ShopBtn>().SetButtonSold();
                        }
                        ap.transform.SetParent(shopAppsContent,false);
                    }
                } // end foreach appSO
            } // end count > 0

        }else{
            shopPanel.SetActive(false);
            shopIsOpen = false;
        }
    } // end OpenShopApp
    
    public void RefreshCreditUI(){
        // Link credits to UI
        shopCreditsText.text = "$" + phoneOwner.GetComponent<Human>().playerMoney; // Player Phone Apps (Store in Human scripts)
    } // end RefreshCreditUI()

#endregion // End Region SHOP RELATED

#region QUICKSLOT RELATED
    public void OpenQuickslotApp(bool open){
        if(open){
            quickslotIsOpen = true;
            quickslotPanel.SetActive(true);
        }else{
            quickslotIsOpen = false;
            quickslotPanel.SetActive(false);
        }
    }// end OpenQuickslotApp

#endregion // end region Quickslot related

}
