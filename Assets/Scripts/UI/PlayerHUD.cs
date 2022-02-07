using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD instance;
    [Title("Clock Timer")]
    public TextMeshProUGUI hudTimerText;

    [Title("Heart Rate Related")]
    public Animator heartRateAnimator;

    [Title("Altar Related")]
    public GameObject[] altarImages;

    [Title("Ritual Item Related")]
    public SO_Ritual_Item ritualSO;
    public Image ritualIcon;

    [Title("Human Avatar")]
    public Image playerIcon1;
    public Image playerIcon2;
    public Image playerIcon3;
    public TextMeshProUGUI playerName1;
    public TextMeshProUGUI playerName2;
    public TextMeshProUGUI playerName3;

    void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void Start(){
        SetItemSlot();
    }

    public void SetItemSlot(){
        if(PlayerManager.instance.itemSlot != ""){
            foreach(var i in ritualSO.itemLists){
                if(i.code == PlayerManager.instance.itemSlot){
                    ritualIcon.gameObject.SetActive(true);
                    ritualIcon.sprite = i.itemIcon;
                }
            }
        }else{
            ritualIcon.gameObject.SetActive(false);
        }
    }
}
