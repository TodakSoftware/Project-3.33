using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Simple_Notification_Spawner : MonoBehaviour
{
    public static UI_Simple_Notification_Spawner instance;

    [Header("The basics")]
    public GameObject NotificationPrefab;
    public GameObject ParentObject;

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

    public void CreateNotification(string AppName, string ItemName)
    {
        GameObject instance = Instantiate(NotificationPrefab, ParentObject.transform);
        instance.GetComponent<UI_Notification_Init>().appNameString = AppName;
    }

    public void CreateNotificationWithType(NotificationType NotificationTypeHere)
    {
        GameObject instance = Instantiate(NotificationPrefab, ParentObject.transform);
        instance.GetComponent<UI_Notification_Init>().CurrentNotificationType = NotificationTypeHere;
    }

}
