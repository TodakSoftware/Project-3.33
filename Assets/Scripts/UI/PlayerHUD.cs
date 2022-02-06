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

    void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
