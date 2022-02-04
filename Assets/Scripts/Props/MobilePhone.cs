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
    public List<string> preinstalledApps = new List<string>();
    public List<string> currentApps = new List<string>();

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

    [Title("In Apps Behaviour")]
    public GameObject shopPanel;

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void Start()
    {
        canvas.worldCamera = PlayerMovement.instance.cam;

        if(PlayerMovement.instance.spawnHandOnly){
            flashLight.transform.localPosition = new Vector3(0.029f, -4f, 0.012f);
        }

        // Add Preinstalled apps
        if(preinstalledApps.Count > 0){
            foreach(var app in preinstalledApps){
                if(!currentApps.Contains(app)){
                    currentApps.Add(app);
                }
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

    
    public void OpenShopApp(){
        shopPanel.SetActive(true);
    }

    public void CloseShopApp(){
        shopPanel.SetActive(true);
    }

}
