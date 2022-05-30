// Handle menu related input & actions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    [Header("Loading Related")]
    public TextMeshProUGUI connectionInfoText; // just to display connected to server
    public GameObject loadingPanel;

    [Header("Misc. Related")]
    public TextMeshProUGUI versionText;

    [Header("Find Game Related")]
    public GameObject findGamePanel;
    public Button joinHumanBtn, joinGhostBtn; // for disabled when "totalPlayers" > 1 (With friends)
    public TextMeshProUGUI findRoomTimeoutText, findGameInfoText;
    public Button cancelFindGameBtn;
    public Coroutine coroutinefindRoomTimeout; // store coroutine for stopping timer
    
    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }

        // Set Version
        versionText.text = "v"+Application.version;

        // Set fps Limit = 60fps
        Application.targetFrameRate = 60;
    }

    public IEnumerator CloseLoadingScreen(float duration){ // Close loading screen after certain seconds
        yield return new WaitForSeconds(duration);
        if(loadingPanel.activeSelf){
            loadingPanel.SetActive(false);
        }
        yield return null;
    } // end CloseLoadingScreen

    // ----------------------- FIND GAME RELATED START -------------------
    public void UpdateUI_FindgameTotal(int totalHuman, int totalGhost){ // Called by NetworkManager. For updating UI on how much player have connected
        if(totalHuman == 1 && totalGhost == 0 || totalHuman == 0 && totalGhost == 1){
            findGameInfoText.text = "Finding game... Please wait.";
        }else{
            findGameInfoText.text = "Match found! \nHuman = " + totalHuman + " / "+ SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].maxHumanPerGame +" | Ghost = " + totalGhost + " / " + SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].maxGhostPerGame;
        }
    } // end UpdateUI_FindgameTotal

    public IEnumerator UpdateUI_FindgameTimeout(float duration){ // Called by NetworkManager. For timeout countdown display when find game
        float timeoutTimer = 1;
        while(timeoutTimer <= duration)
        {
            timeoutTimer += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeoutTimer / 60f);
            int seconds = Mathf.FloorToInt(timeoutTimer - minutes * 60f);
            string formattedTimer = string.Format("{0:0}:{1:00}", minutes, seconds);

            findRoomTimeoutText.text = formattedTimer;
            yield return null;
        }

        if(timeoutTimer >= duration){
            findRoomTimeoutText.text = "TIMEOUT!";
            // tell networkmanager to close the finding
            NetworkManager.instance.CancelFindGameOrLeaveRoom();
        }
    } // end UpdateUI_FindgameTimeout

    // ----------------------- FIND GAME RELATED END -------------------

    // ----------------------- INVITE FRIENDS RELATED START -------------------
    public void UpdateTotalPartyMember(int members){ // Call this when friends join/leave a party
        if(members > 1){ // if 2 or more players in a party, disable joinGhostBtn
            joinGhostBtn.interactable = false;
        }else{ // if we are alone or 1 player only, enable both
            joinGhostBtn.interactable = true;
        }
    } // end UpdateTotalPartyMember
    
    // ----------------------- INVITE FRIENDS RELATED END -------------------
}
