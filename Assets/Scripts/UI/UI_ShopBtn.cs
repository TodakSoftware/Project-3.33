using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ShopBtn : MonoBehaviour
{
    [Header("Shop Details")]
    public string appCode;
    public int appPrice;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;
    public Image icon;
    public TextMeshProUGUI price;
    public Button btn;
    public TextMeshProUGUI btnText;
    [HideInInspector] public MobilePhone phoneRef;
     public AK.Wwise.Event buySound = new AK.Wwise.Event();

    public void SetButtonSold(){
        btn.interactable = false;
        btn.image.fillCenter = false;
        btnText.text = "Sold";
        btnText.color = Color.white;
    }

    public void BuyApps(){
        // if currentCredits >= appPrice, current - price = sold,
        if(phoneRef.phoneOwner.GetComponent<Human>().playerMoney >= appPrice){
            phoneRef.phoneOwner.GetComponent<Human>().playerMoney -= appPrice;
            if(!phoneRef.currentApps.Contains(appCode)){
                phoneRef.currentApps.Add(appCode);
                AkSoundEngine.PostEvent(buySound.Name, gameObject);
            }
            phoneRef.RefreshMainScreen();
            phoneRef.RefreshCreditUI();
            SetButtonSold();
            print("Buy Apps, Deduct credits, add code to currentApps list, refresh main screen to add new app");
        }else{
            print("Not enough credits");
        }
    } // end buyapps
}
