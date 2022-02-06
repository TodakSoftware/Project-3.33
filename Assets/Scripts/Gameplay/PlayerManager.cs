using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Title("Human")]
    public int currentCredits;
    [Range(0,100)] public int fearLevel;
    private bool hpGreen, hpYellow, hpRed;

    [Title("Timer Related")]
    public bool timeOut;
    public int Duration {get; private set;}
    private int remainingDuration;

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void Start(){
        // Clock Timer
        remainingDuration = 1560; // 1560 = Starting 26m
        StartCoroutine(UpdateTimer());
        StartCoroutine(UpdateHeartRate());
    }

    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------
    private void ResetTimer(){
        MobilePhone.instance.hpClockText.text = "<color=red>3:33</color>";
        PlayerHUD.instance.hudTimerText.text = "<color=red>3:33</color>";
        remainingDuration = 0;
    }

    private IEnumerator UpdateTimer(){
        while(remainingDuration > 0 && remainingDuration <= 1620){ // 1980 = 33m
            UpdateTimerUI(remainingDuration);
            remainingDuration++;
            yield return new WaitForSeconds(1f);
        }
        EndTimer();
    }

    private void UpdateTimerUI(int seconds){
        //print(string.Format("3:{0:D2}:{1:D2}", seconds/60, seconds % 60));
        if(MobilePhone.instance != null){
            MobilePhone.instance.hpClockText.text = "3<color=red>:</color>" + string.Format("{0:D2}", seconds/60) + "<size=30><color=red>:</color>" + string.Format("{0:D2}", seconds % 60) + "</size>";
        }else{
            print("Not initialized");
        }
        PlayerHUD.instance.hudTimerText.text = "3<color=red>:</color>" + string.Format("{0:D2}", seconds/60);
    }

    public void EndTimer(){
        timeOut = true;
        ResetTimer();
    }

    private void OnDestroy(){
        StopAllCoroutines();
    }
    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------

    // --------------------------------- HEART RATE FUNCTION START ----------------------------------

    private IEnumerator UpdateHeartRate(){
        while(fearLevel >= 0 && fearLevel <= 100){ 
            UpdateHeartUI();
            yield return new WaitForSeconds(1f);
        }
        EndHeartRate();
    }

    private void UpdateHeartUI(){
        if(fearLevel < 50){
            if(!hpGreen){
                PlayerHUD.instance.heartRateAnimator.SetTrigger("Healthy");
                hpGreen = true;
                hpYellow = false;
                hpRed = false;
            }
        }else if(fearLevel >= 50 && fearLevel < 80){
            if(!hpYellow){
                PlayerHUD.instance.heartRateAnimator.SetTrigger("Panic");
                hpGreen = false;
                hpYellow = true;
                hpRed = false;
            }
        }else if(fearLevel >= 80 && fearLevel <= 100){
            if(!hpRed){
                PlayerHUD.instance.heartRateAnimator.SetTrigger("Dying");
                hpGreen = false;
                hpYellow = false;
                hpRed = true;
            }
        }
    }

    public void EndHeartRate(){
       
    }

    public void AdjustFearLevel(int amount){
        fearLevel += amount;

        if(fearLevel <= 0){
            fearLevel = 0;
        }else if(fearLevel >= 100){
            fearLevel = 100;
        }
    }

    // --------------------------------- HEART RATE FUNCTION END ----------------------------------

    // --------------------------------- ALTAR FUNCTION START ----------------------------------
    public void UpdateAltarSlot(int amount){
        // Clear first
        foreach(var slot in PlayerHUD.instance.altarImages){
            slot.SetActive(false);
        }

        // Enable current amount
        for(int i = 0; i < amount; i++){
            PlayerHUD.instance.altarImages[i].SetActive(true);
        }
    }
    // --------------------------------- ALTAR FUNCTION END ----------------------------------
}
