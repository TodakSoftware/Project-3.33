using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobilePhone : MonoBehaviour
{
    public static MobilePhone instance;
    [Title("Enabler")]
    public bool enableDrain;
    
    [Title("Apps Related")]
    public SO_Apps appsSO;
    //public List<string> preinstalledApps = new List<string>();
    public List<string> currentApps = new List<string>();
    public List<string> soldApps = new List<string>();

    [Title("Battery")]
    public TextMeshProUGUI batteryText;
    public Image batteryCase, batteryBar;
    [SerializeField] Sprite batteryOk, batteryEmpty;
    [SerializeField][Range(0f,103f)] float currentBattery;
    [SerializeField]float drainRateTotal;

    [Title("Others")]
    [SerializeField]bool phoneIsDead;
    public Canvas canvas;
    public GameObject flashLight;
    public GameObject appsPrefab;
    public Transform appsContentGroup;
    [HideInInspector] public PlayerMovement playerMvmt;

    [Title("In Apps Setup")]
    public GameObject shopPanel;
    public GameObject quickslotPanel;
    public GameObject chatGroupPanel;

    [Title("Shop Related")]
    public GameObject shopBtnPrefab;
    public Transform shopContentGroup;
    public TextMeshProUGUI shopCreditsText;

    [Title("Clock Timer")]
    public TextMeshProUGUI hpClockText;

    [Title("Chat Related")]
    public TMP_InputField chatInput;
    public GameObject aboutAppPage;

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void Start()
    {
        canvas.worldCamera = playerMvmt.cam;

        if(playerMvmt.spawnHandOnly){
            // flashLight.transform.localPosition = new Vector3(0.029f, -4f, 0.012f); OLD VALUES
            flashLight.transform.localPosition = new Vector3(0.0159f, 0.021f, 0.069f);
        }

        // Add Preinstalled apps
        if(appsSO.appLists.Count > 0){
            foreach(var app in appsSO.appLists){
                if(app.systemApp){
                    currentApps.Add(app.code);
                }
            }
        }

        // Refresh main screen
        RefreshMainScreen();

        // Default flash light is on (Beinning of the game)
        if(PlayerAbilities.instance.flashlightOn){
            drainBattery(true, "A001");
        }
    }

    void Update(){
        if(currentBattery >= 100f){ // fake it
            batteryText.text = "100%";
        }else{
            batteryText.text = (int)currentBattery + "%";
        }
        
        if(currentBattery >= 80f){
            batteryBar.fillAmount = 1f;
        }else if(currentBattery >= 60f && currentBattery < 80){
            batteryBar.fillAmount = 0.8f;
        }else if(currentBattery >= 40f && currentBattery < 60){
            batteryBar.fillAmount = 0.6f;
        }else if(currentBattery >= 20f && currentBattery < 40){
            batteryBar.fillAmount = 0.4f;
            batteryBar.color = Color.white;
        }else if(currentBattery >= 1f && currentBattery < 20){
            batteryBar.fillAmount = 0.2f;
            batteryBar.color = Color.red;
            batteryCase.sprite = batteryOk;
        }else{
            batteryBar.fillAmount = 0f;
            batteryCase.sprite = batteryEmpty;
        }
        
        if(enableDrain && !phoneIsDead){
            HandleDrainCalculation();
        }
    }

    // Update is called once per frame
    public void ChangeLandscape(bool val){
        if(val){
            GetComponent<Animator>().SetBool("Landscape", true);
        }else{
            GetComponent<Animator>().SetBool("Landscape", false);
        }
    }

    void HandleDrainCalculation(){
        if(!phoneIsDead){
            currentBattery -= Time.deltaTime * drainRateTotal;

            if(currentBattery < 0){
                phoneIsDead = true;
            }
        }
    }

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
    }

    // Refresh main screen apps
    public void RefreshMainScreen(){
        // Clear any existing content + currentAppList
        if(appsContentGroup.childCount > 0){
            foreach(Transform c in appsContentGroup){
                Destroy(c.gameObject);
            }
        }

        // Shown on phone
        if(currentApps.Count > 0){
            foreach(var code in currentApps){
                foreach(var apps in appsSO.appLists){
                    if(code == apps.code){
                        var ap = Instantiate(appsPrefab);
                        ap.GetComponent<AppsUI>().appCode = apps.code;
                        ap.GetComponent<AppsUI>().appIcon.sprite = apps.appIcon;
                        ap.GetComponent<AppsUI>().appName.text = apps.name;
                        ap.transform.SetParent(appsContentGroup,false);
                    }
                } // end foreach appSO
            } // end foreach currentApps
        } // end count > 0
    }
    
    // Shop Apps
    public void OpenShopApp(bool open){
        if(open){
            shopPanel.SetActive(true);
            RefreshCreditUI();

            // Clear any existing content
            if(shopContentGroup.childCount > 0){
                foreach(Transform c in shopContentGroup){
                    Destroy(c.gameObject);
                }
            }

            // List non system app
            if(appsSO.appLists.Count > 0){
                foreach(var apps in appsSO.appLists){
                    if(!apps.systemApp){
                        var ap = Instantiate(shopBtnPrefab);
                        ap.GetComponent<ShopBtn>().appCode = apps.code;
                        ap.GetComponent<ShopBtn>().appPrice = apps.price;
                        ap.GetComponent<ShopBtn>().icon.sprite = apps.appIcon;
                        ap.GetComponent<ShopBtn>().title.text = apps.name;
                        ap.GetComponent<ShopBtn>().desc.text = apps.description;
                        ap.GetComponent<ShopBtn>().price.text = "$"+apps.price;
                        if(currentApps.Contains(apps.code)){
                            ap.GetComponent<ShopBtn>().SetButtonSold();
                        }
                        ap.transform.SetParent(shopContentGroup,false);
                    }
                } // end foreach appSO
            } // end count > 0

        }else{
            shopPanel.SetActive(false);
        }
    }

    // Refresh Credits UI
    public void RefreshCreditUI(){
        // Link credits to UI
        shopCreditsText.text = "$" + PlayerManager.instance.currentCredits;
    }

    // Quickslot Apps
    public void OpenQuickslotApp(bool open){
        if(open){
            quickslotPanel.SetActive(true);
        }else{
            quickslotPanel.SetActive(false);
        }
    }

    // Chat Group Apps
    public void OpenChatGroup(bool open){
        if(open){
            chatGroupPanel.SetActive(true);
            chatInput.text = "";
        }else{
            chatGroupPanel.SetActive(false);
            if(aboutAppPage.activeSelf){
                aboutAppPage.SetActive(false);
            }
        }
    }

    // Chat Back Button
    public void CloseChatGroup(){
        playerMvmt.HandleChatGroup();
    }
}
