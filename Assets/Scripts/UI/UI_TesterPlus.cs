using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TesterPlus : MonoBehaviour
{
    [Header("Notification Stuff----------------------------")]
    //public GameObject SimpleNotificationSpawnerScriptObject;
    public GameObject TesterPlusDropdownType;
    public GameObject TesterPlusAppNameObject;
    public GameObject TesterPlusItemNameObject;

    public GameObject UINotificationSpawnerScript;

    [Space(10)]
    [Header("Messages Stuff--------------------------------")]
    public GameObject UIMessageSpawnerScript;
    
    public GameObject TesterPlusPlayerNameObject;
    public GameObject TesterPlusPlayerMessageObject;

    [Header("Player Colors")]
    public Color PlayerColor00;
    public Color PlayerColor01;
    public Color PlayerColor02;

    public int ColorCyclerCounter;

    [Space(10)]
    [Header("Task Stuff------------------------------------")]
    public GameObject[] TasksObjects;
    public GameObject TesterPlusDropdownTaskNumber;

    // Start is called before the first frame update
    void Start()
    {
    

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //notification demo
    public void TesterPlusChangeNotificationType()
    {

        switch (TesterPlusDropdownType.GetComponent<TMPro.TMP_Dropdown>().value)
        {
            case 0:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.PICKUPITEM);
                break;
            case 1:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.PICKUPCURRENCY);
                break;
            case 2:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.SLOTFULL);
                break;
            case 3:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.TESTTYPE);
                break;
            default:
                UI_Simple_Notification_Spawner.instance.CreateNotificationWithType(NotificationType.TESTTYPE);
                break;
        }

        
    }
    
    //TesterPlusAppNameObject.GetComponent<TMP_InputField>().text;

    public void TesterPlusCreateNotification()
    {

        //Instance Mode

        //switch (TesterPlusDropdownType.GetComponent<TMPro.TMP_Dropdown>().value)
        //{
        //    case 0:
        //        UI_Simple_Notification_Spawner.instance.CreateNotification(NotificationType.PICKUPITEM, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
        //        break;
        //    case 1:
        //        UI_Simple_Notification_Spawner.instance.CreateNotification(NotificationType.PICKUPCURRENCY, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
        //        break;
        //    case 2:
        //        UI_Simple_Notification_Spawner.instance.CreateNotification(NotificationType.SLOTFULL, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
        //        break;
        //    case 3:
        //        UI_Simple_Notification_Spawner.instance.CreateNotification(NotificationType.TESTTYPE, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
        //        break;
        //    default:
        //        UI_Simple_Notification_Spawner.instance.CreateNotification(NotificationType.TESTTYPE, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
        //        break;
        //}

        switch (TesterPlusDropdownType.GetComponent<TMPro.TMP_Dropdown>().value)
        {
            case 0:
                UINotificationSpawnerScript.GetComponent<UI_Simple_Notification_Spawner>().CreateNotification(NotificationType.PICKUPITEM, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
                break;
            case 1:
                UINotificationSpawnerScript.GetComponent<UI_Simple_Notification_Spawner>().CreateNotification(NotificationType.PICKUPCURRENCY, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
                break;
            case 2:
                UINotificationSpawnerScript.GetComponent<UI_Simple_Notification_Spawner>().CreateNotification(NotificationType.SLOTFULL, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
                break;
            case 3:
                UINotificationSpawnerScript.GetComponent<UI_Simple_Notification_Spawner>().CreateNotification(NotificationType.TESTTYPE, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
                break;
            default:
                UINotificationSpawnerScript.GetComponent<UI_Simple_Notification_Spawner>().CreateNotification(NotificationType.TESTTYPE, TesterPlusAppNameObject.GetComponent<TMP_InputField>().text, TesterPlusItemNameObject.GetComponent<TMP_InputField>().text);
                break;
        }

    }


    public void TesterPlusCreateIncomingMessage()
    {
        //cycling colors
        switch (ColorCyclerCounter)
        {
            case 0:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateIncomingMessage(TesterPlusPlayerNameObject.GetComponent<TMP_InputField>().text, TesterPlusPlayerMessageObject.GetComponent<TMP_InputField>().text, PlayerColor00);
                ColorCyclerCounter = 1;
                break;
            case 1:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateIncomingMessage(TesterPlusPlayerNameObject.GetComponent<TMP_InputField>().text, TesterPlusPlayerMessageObject.GetComponent<TMP_InputField>().text, PlayerColor01);
                ColorCyclerCounter = 2;
                break;
            case 2:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateIncomingMessage(TesterPlusPlayerNameObject.GetComponent<TMP_InputField>().text, TesterPlusPlayerMessageObject.GetComponent<TMP_InputField>().text, PlayerColor02);
                ColorCyclerCounter = 0;
                break;
            default:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateIncomingMessage("Error", "Error", PlayerColor00);
                ColorCyclerCounter = 0;
                break;
        }

    }

    public void TesterPlusCreateOutgoingMessage()
    {
        //cycling colors
        switch (ColorCyclerCounter)
        {
            case 0:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateOutgoingMessage(TesterPlusPlayerNameObject.GetComponent<TMP_InputField>().text, TesterPlusPlayerMessageObject.GetComponent<TMP_InputField>().text, PlayerColor00);
                ColorCyclerCounter = 1;
                break;
            case 1:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateOutgoingMessage(TesterPlusPlayerNameObject.GetComponent<TMP_InputField>().text, TesterPlusPlayerMessageObject.GetComponent<TMP_InputField>().text, PlayerColor01);
                ColorCyclerCounter = 2;
                break;
            case 2:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateOutgoingMessage(TesterPlusPlayerNameObject.GetComponent<TMP_InputField>().text, TesterPlusPlayerMessageObject.GetComponent<TMP_InputField>().text, PlayerColor02);
                ColorCyclerCounter = 0;
                break;
            default:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateOutgoingMessage("Error", "Error", PlayerColor00);
                ColorCyclerCounter = 0;
                break;
        }

    }


    public void TesterPlusCompleteTask()
    {

        switch (TesterPlusDropdownTaskNumber.GetComponent<TMPro.TMP_Dropdown>().value)
        {
            case 0:
                TasksObjects[0].GetComponent<UI_TaskPrefabStatus>().SetToComplete();
                break;
            case 1:
                TasksObjects[1].GetComponent<UI_TaskPrefabStatus>().SetToComplete();
                break;
            case 2:
                TasksObjects[2].GetComponent<UI_TaskPrefabStatus>().SetToComplete();
                break;
            case 3:
                TasksObjects[3].GetComponent<UI_TaskPrefabStatus>().SetToComplete();
                break;
            case 4:
                TasksObjects[4].GetComponent<UI_TaskPrefabStatus>().SetToComplete();
                break;
            default:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateOutgoingMessage("Error", "Error", PlayerColor00);
                ColorCyclerCounter = 0;
                break;
        }

    }

    public void TesterPlusIncompleteTask()
    {

        switch (TesterPlusDropdownTaskNumber.GetComponent<TMPro.TMP_Dropdown>().value)
        {
            case 0:
                TasksObjects[0].GetComponent<UI_TaskPrefabStatus>().SetToIncomplete();
                break;
            case 1:
                TasksObjects[1].GetComponent<UI_TaskPrefabStatus>().SetToIncomplete();
                break;
            case 2:
                TasksObjects[2].GetComponent<UI_TaskPrefabStatus>().SetToIncomplete();
                break;
            case 3:
                TasksObjects[3].GetComponent<UI_TaskPrefabStatus>().SetToIncomplete();
                break;
            case 4:
                TasksObjects[4].GetComponent<UI_TaskPrefabStatus>().SetToIncomplete();
                break;
            default:
                UIMessageSpawnerScript.GetComponent<UI_ChatMessageSpawner>().CreateOutgoingMessage("Error", "Error", PlayerColor00);
                ColorCyclerCounter = 0;
                break;
        }

    }
}
