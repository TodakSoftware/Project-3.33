using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Notification_Init : MonoBehaviour
{

    //public enum NotificationType {PickupItem, PickupCurrency, SlotFull, TestType};
    
    public NotificationType CurrentNotificationType;


    [Header("Texts objects")]
    public TextMeshProUGUI appNameText;
    public TextMeshProUGUI itemNameText;

    [Header("Texts strings")]
    public string appNameString;
    public string itemNameString;

    [Header("Images")]
    public GameObject iconImage;

    //[Header("Animations")]
    //public Animator NotificationSpawnerAnimator;

    // Start is called before the first frame update
    void Start()
    {

        //initialisations
        switch(CurrentNotificationType)
        {
            case NotificationType.PICKUPITEM:
                appNameText.text = "<color=#000000>" + appNameString + "</color>" + "<color=#606060>- Now</color>";
                itemNameText.text = itemNameString + "has been picked up";
                break;
            case NotificationType.PICKUPCURRENCY:
                appNameText.text = "<color=#000000>" + appNameString + "</color>" + "<color=#606060>- Now</color>";
                itemNameText.text = itemNameString + "has been picked up";
                break;
            case NotificationType.SLOTFULL:
                appNameText.text = "<color=#FF1E00>" + appNameString + "</color>" + "<color=#606060>- Now</color>";
                itemNameText.text = "Item slot is full!";
                break;
            case NotificationType.TESTTYPE:
                appNameText.text = "<color=#FF1E00>" + appNameString + "</color>" + "<color=#606060>- Now</color>";
                itemNameText.text = "Testing Mode!";
                break;
            default:
                appNameText.text = "<color=#FF1E00>" + appNameString + "</color>" + "<color=#606060>- Now</color>";
                itemNameText.text = "Unknown notification type!";
                break;
        }

        //appNameText.text = "<color=#000000>" + appNameString + "</color>" + "<color=#606060>- Now</color>";
        //itemNameText.text = itemNameString + "has been picked up";

        //NotificationSpawnerAnimator.Play("UI_Notification_Entry");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this is called in animation event
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }


}
