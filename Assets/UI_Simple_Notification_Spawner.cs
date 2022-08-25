using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Simple_Notification_Spawner : MonoBehaviour
{
    public static UI_Simple_Notification_Spawner instance;

    [Header("The basics")]
    public GameObject NotificationPrefab;
    public GameObject ParentObject;
    //public image icon;

    //public NotificationType NotificationTypeHere;

    // Start is called before the first frame update
    void Start()
    {

        if(instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateNotificationSimple()
    {
        Instantiate(NotificationPrefab, ParentObject.transform);
    }

    /// <summary>
    /// Creates notification in the notification parent. Not for message notification however.
    /// </summary>
    /// <param name="notificationType">There's 4 types, PICKUPITEM, PICKUPCURRENCY, SLOTFULL and TESTTYPE</param>
    /// <param name="appName">the name of the app related, i.e. inventory, wallet</param>
    /// <param name="itemName">item name. for currency, just type in the number</param>
    public void CreateNotification(NotificationType notificationType, string appName, string itemName)
    {
        GameObject instance = Instantiate(NotificationPrefab, ParentObject.transform);
        instance.GetComponent<UI_Notification_Init>().CurrentNotificationType = notificationType; 
        instance.GetComponent<UI_Notification_Init>().appNameString = appName;
        instance.GetComponent<UI_Notification_Init>().itemNameString = itemName;

    }



    
    public void CreateNotificationWithType(NotificationType notificationTypeHere)
    {
        GameObject instance = Instantiate(NotificationPrefab, ParentObject.transform);
        instance.GetComponent<UI_Notification_Init>().CurrentNotificationType = notificationTypeHere;
    }

}
