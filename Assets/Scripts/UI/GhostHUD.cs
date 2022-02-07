using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GhostHUD : MonoBehaviour
{
    public static GhostHUD instance;
    [Title("Clock Timer")]
    public TextMeshProUGUI hudTimerText;

    [Title("Altar Related")]
    public GameObject[] altarImages;
    
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
}
