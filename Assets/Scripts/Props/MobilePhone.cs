using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobilePhone : MonoBehaviour
{
    [Header("Enabler")]
    public bool enableDrain;

    [Header("Apps Related")]
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

    [Header("Shop Related")]
    public GameObject shopBtnPrefab;
    public Transform shopAppsContent;
    public TextMeshProUGUI shopCreditsText;

    [Header("Clock Timer")]
    public TextMeshProUGUI hpClockText;

    [Header("Main Phone Related")]
    [SerializeField]bool phoneIsDead;
    public Canvas phoneCanvas;
    public GameObject phoneLight;
    public GameObject appsPrefab;
    public Transform menuAppsContent;
}
