using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TesterPlus : MonoBehaviour
{

    //public GameObject SimpleNotificationSpawnerScriptObject;
    public GameObject TesterPlusDropdownType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TesterPlusChangeNotificationType()
    {

        switch (TesterPlusDropdownType.GetComponent<TMPro.TMP_Dropdown>().value)
        {
            case 0:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.PICKUPITEM);
                break;
            default:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.TESTTYPE);
                break;
        }

        
    }

}
