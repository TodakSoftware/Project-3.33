// Handle UI Popup Related (Mostly modal)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject mainCanvas; // Ref for mainCanvas
    public GameObject cacheLoadingDuration, cacheLoadingScene, cacheFindGame, cacheReconnectGame;
    public Modal_FindGame modalFindGame;
    public Modal_ReconnectGame modalReconnectGame;
    
    void Awake()
    {
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if(mainCanvas == null){ // Find maincanvas on load if mainCanvas is empty.
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        }
    }

    void Update()
    {
        if(mainCanvas == null){ // Always check if mainCanvas is missing. 
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        }
    }

/* ------------------------------------------------ NORMAL LOADING START ---------------------------------------------------------*/
    public void PopupLoadingNormal(string title, string description){ // Popup normal loading UI with params Title & Desc.
        if(cacheLoadingDuration == null){ // Check if 1st time popup
            var loading = Instantiate(SOManager.instance.prefabs.modalLoadingNormal); 
            loading.GetComponent<Modal_NormalLoading>().titleText.text = title; // Set title
            loading.GetComponent<Modal_NormalLoading>().descriptionText.text = description; // Set Desc.
            loading.transform.SetParent(mainCanvas.transform, false); // Parent it inside mainCanvas
            cacheLoadingDuration = loading; // caching Gameobject for next time used
        }else{
            // if we have cache the loading UI, resuse it by SetActive
            cacheLoadingDuration.SetActive(true);
            cacheLoadingDuration.GetComponent<Modal_NormalLoading>().titleText.text = title; // Set title
            cacheLoadingDuration.GetComponent<Modal_NormalLoading>().descriptionText.text = description; // Set Desc.
        }
    } // end PopupLoadingNormal

    public void UpdateLoadingNormal(string title, string description){ // Update the Loading UI while it still open/active
        if(cacheLoadingDuration != null){ // Make sure we have the cache version 
            cacheLoadingDuration.GetComponent<Modal_NormalLoading>().titleText.text = title; // Set title
            cacheLoadingDuration.GetComponent<Modal_NormalLoading>().descriptionText.text = description; // Set Desc.
        } // end cacheLoadingDuration != null
    } // end UpdateLoadingNormal

    public IEnumerator CloseNormalLoading(float duration){ // Close the loading UI with certain duration
        if(cacheLoadingDuration != null){
            yield return new WaitForSeconds(duration);
            cacheLoadingDuration.SetActive(false);
        } // end cacheLoadingDuration != null
    } // end CloseNormalLoading

/* ------------------------------------------------ NORMAL LOADING END ---------------------------------------------------------*/

/* ------------------------------------------------ SCENE LOADING START ---------------------------------------------------------*/
#region SCENE LOADING
    public void PopupLoadingScene(){ // Popup loading scene
        if(cacheLoadingScene == null){
            var loading = Instantiate(SOManager.instance.prefabs.modalLoadingScene);
            loading.transform.SetParent(mainCanvas.transform, false);
            cacheLoadingScene = loading;
        }else{
            // if we have cache the loading UI, resuse it by SetActive
            cacheLoadingScene.SetActive(true);
        }
    } // end PopupLoadingScene

    public IEnumerator CloseLoadingScene(float duration){ // Close the loading UI with certain duration
        if(cacheLoadingScene != null){
            yield return new WaitForSeconds(duration);
            cacheLoadingScene.SetActive(false);
        }
    } // end CloseLoadingScene

#endregion
/* ------------------------------------------------ SCENE LOADING END ---------------------------------------------------------*/

/* ------------------------------------------------  FIND GAME RELATED START ---------------------------------------------------------*/
#region FIND GAME RELATED

    public void PopupFindGame(){
        if(cacheFindGame == null){
            var find = Instantiate(SOManager.instance.prefabs.modalFindGame);
            find.transform.SetParent(mainCanvas.transform, false);
            cacheFindGame = find;
            modalFindGame = find.GetComponent<Modal_FindGame>();
        }else{
            cacheFindGame.SetActive(true); // if we have cache the UI, resuse instead destroy
            modalFindGame.findRoomTimeoutText.text = "0:00"; // Reset find game timeout timer
        }
    }

    public IEnumerator CloseFindGame(float duration){
        if(cacheFindGame != null){
            yield return new WaitForSeconds(duration);
            cacheFindGame.SetActive(false);
            NetworkManager.instance.CancelFindGameOrLeaveRoom(); // tell networkmanager to close the finding
        }
    }

    public void activeFindgameCancel(bool isOn){
        if(isOn){
            modalFindGame.cancelFindGameBtn.interactable = true;
        }else{
            modalFindGame.cancelFindGameBtn.interactable = false;
        }
    }

    public void UpdateUI_FindgameTotal(int currentHuman, int totalHuman, int currentGhost, int totalGhost){ // Called by NetworkManager. For updating UI on how much player have connected
        if(currentHuman == 1 && currentGhost == 0 || currentHuman == 0 && currentGhost == 1){
            modalFindGame.findGameInfoText.text = "Finding game... Please wait.";
        }else{
            modalFindGame.findGameInfoText.text = "Match found! \nHuman = " + currentHuman + " / "+ totalHuman +" | Ghost = " + currentGhost + " / " + totalGhost;
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

            if(modalFindGame != null){
                modalFindGame.findRoomTimeoutText.text = formattedTimer;
            }
            
            yield return null;
        }

        if(timeoutTimer >= duration){
            modalFindGame.findRoomTimeoutText.text = "TIMEOUT!";
            StartCoroutine(CloseFindGame(.3f)); // Close find game UI 
        }
    } // end UpdateUI_FindgameTimeout

#endregion
/* ------------------------------------------------  FIND GAME RELATED END ---------------------------------------------------------*/

/* ------------------------------------------------  INGAME RECONNECT RELATED START ---------------------------------------------------------*/
#region INGAME RECONNECT RELATED
    public void PopupReconnectGame(){
        if(cacheReconnectGame == null){
            var reconnect = Instantiate(SOManager.instance.prefabs.modalReconnectGame);
            reconnect.transform.SetParent(mainCanvas.transform, false);
            cacheReconnectGame = reconnect;
            modalReconnectGame = reconnect.GetComponent<Modal_ReconnectGame>();
        }else{
            cacheReconnectGame.SetActive(true); // if we have cache the UI, resuse instead destroy
        }
    }

    public IEnumerator CloseReconnectGame(float duration){
        if(cacheReconnectGame != null){
            yield return new WaitForSeconds(duration);
            cacheReconnectGame.SetActive(false);
        }
    }

#endregion
/* ------------------------------------------------  INGAME RECONNECT RELATED END ---------------------------------------------------------*/


/* ------------------------------------------------  INGAME RELATED START ---------------------------------------------------------*/
    public void VictoryUI(bool humanWin){
        var endUI = Instantiate(SOManager.instance.prefabs.uiVictory);
        if(humanWin){
            endUI.GetComponent<UI_VictoryResult>().HumanWin();
        }else{
            endUI.GetComponent<UI_VictoryResult>().GhostWin();
        }
        endUI.transform.SetParent(mainCanvas.transform, false);
    }
/* ------------------------------------------------  INGAME RELATED END ---------------------------------------------------------*/
}