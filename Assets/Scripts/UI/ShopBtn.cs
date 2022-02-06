using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class ShopBtn : MonoBehaviour
{
    [Title("Shop Details")]
    public string appCode;
    public int appPrice;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image icon;
    public TextMeshProUGUI price;
    public Button btn;
    public TextMeshProUGUI btnText;

    public void SetButtonSold(){
        btn.interactable = false;
        btn.image.fillCenter = false;
        btnText.text = "Sold";
        btnText.color = Color.white;
    }

    public void BuyApps(){
        // if currentCredits >= appPrice, current - price = sold,
        if(PlayerManager.instance.currentCredits >= appPrice){
            PlayerManager.instance.currentCredits -= appPrice;
            if(!MobilePhone.instance.currentApps.Contains(appCode)){
                MobilePhone.instance.currentApps.Add(appCode);
            }
            MobilePhone.instance.RefreshMainScreen();
            MobilePhone.instance.RefreshCreditUI();
            SetButtonSold();
            print("Buy Apps, Deduct credits, add code to currentApps list, refresh main screen to add new app");
        }else{
            print("Not enough credits");
        }
    } // end buyapps
}
