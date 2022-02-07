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
    public bool isGhost;

    [Title("Human")]
    public int currentCredits;
    [Range(0,100)] public int fearLevel;
    private bool hpGreen, hpYellow, hpRed;
    public string itemSlot;
    public bool canTransferItem;
    public int totalContributed;
    public Ritual_Altar altar;

    [Title("Timer Related")]
    public bool timeOut;
    public int Duration {get; private set;}
    private int remainingDuration;

    [Title("Win Lose Related")]
    public bool gameEnded;
    public GameObject uiWinLose;
    public TextMeshProUGUI winLoseText;
    

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this);
        }
    }

    void Start(){
        UpdateAltarSlot(totalContributed);
        // Clock Timer
        remainingDuration = 1560; // 1560 = Starting 26m
        StartCoroutine(UpdateTimer());
        if(!isGhost){
            StartCoroutine(UpdateHeartRate());
        }
    }

    void Update(){
        if(canTransferItem){
            if(Input.GetKeyDown(KeyCode.E)){
                if(itemSlot != ""){
                    altar.itemSlot.Add(itemSlot);
                    altar.TempShowItem();
                    itemSlot = "";
                    PlayerHUD.instance.SetItemSlot();
                    UpdateAltarSlot(altar.itemSlot.Count);
                }else{
                    print("No Item To Be Put");
                } 
            }
        }

        if(gameEnded){
            StopAllCoroutines();
        }
    }

    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------
    private void ResetTimer(){
        if(!isGhost){
            if(MobilePhone.instance != null){
                MobilePhone.instance.hpClockText.text = "<color=red>3:33</color>";
            }
            if(PlayerHUD.instance != null)
            PlayerHUD.instance.hudTimerText.text = "<color=red>3:33</color>";
        }else{
            if(GhostHUD.instance != null)
            GhostHUD.instance.hudTimerText.text = "<color=red>3:33</color>";
        }
        
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
        if(!isGhost){
            if(MobilePhone.instance != null){
                MobilePhone.instance.hpClockText.text = "3<color=red>:</color>" + string.Format("{0:D2}", seconds/60) + "<size=30><color=red>:</color>" + string.Format("{0:D2}", seconds % 60) + "</size>";
            }

            if(PlayerHUD.instance != null)
            PlayerHUD.instance.hudTimerText.text = "3<color=red>:</color>" + string.Format("{0:D2}", seconds/60);
        }else{ // else if ghost
            if(GhostHUD.instance != null)
            GhostHUD.instance.hudTimerText.text = "3<color=red>:</color>" + string.Format("{0:D2}", seconds/60);
        }
    }

    public void EndTimer(){
        timeOut = true;

        if(timeOut && totalContributed < 5){
            // popup win for ghost/ lose for human
           GhostWin();
        }
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
                if(PlayerHUD.instance != null){
                    PlayerHUD.instance.heartRateAnimator.SetTrigger("Healthy");
                    hpGreen = true;
                    hpYellow = false;
                    hpRed = false;
                }
            }
        }else if(fearLevel >= 50 && fearLevel < 80){
            if(!hpYellow){
                if(PlayerHUD.instance != null){
                    PlayerHUD.instance.heartRateAnimator.SetTrigger("Panic");
                    hpGreen = false;
                    hpYellow = true;
                    hpRed = false;
                }
            }
        }else if(fearLevel >= 80 && fearLevel <= 100){
            if(!hpRed){
                if(PlayerHUD.instance != null){
                    PlayerHUD.instance.heartRateAnimator.SetTrigger("Dying");
                    hpGreen = false;
                    hpYellow = false;
                    hpRed = true;
                }
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

        if(!isGhost){
            // Clear first
            foreach(var slot in PlayerHUD.instance.altarImages){
                slot.SetActive(false);
            }

            // Enable current amount
            for(int i = 0; i < amount; i++){
                PlayerHUD.instance.altarImages[i].SetActive(true);
            }
        }else{
            // Clear first
            foreach(var slot in GhostHUD.instance.altarImages){
                slot.SetActive(false);
            }

            // Enable current amount
            for(int i = 0; i < amount; i++){
                GhostHUD.instance.altarImages[i].SetActive(true);
            }
        }

        if(amount >= 5){
            amount = 5;
            // popup win for human/ lose for ghost
            HumanWin();
        }
    }
    // --------------------------------- ALTAR FUNCTION END ----------------------------------

    public void HumanWin(){
        gameEnded = true;
        uiWinLose.SetActive(true);
        if(!isGhost){
            winLoseText.text = "Human Win!";
        }else{ // else ghost win
            winLoseText.text = "<color=red>Ghost Lose!</color>";
        }

        PlayerMovement.instance.enableMouseLook = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetComponent<Interactor>().crosshair.SetActive(false);
    }

    public void GhostWin(){
        gameEnded = true;
        uiWinLose.SetActive(true);
        if(!isGhost){
            winLoseText.text = "<color=red>Human Lose!</color>";
        }else{ // else ghost win
            winLoseText.text = "Ghost Win!";
        }

        PlayerMovement.instance.enableMouseLook = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GetComponent<Interactor>().crosshair.SetActive(false);
    }
}
