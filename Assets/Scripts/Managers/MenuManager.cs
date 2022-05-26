// Handle menu related input & actions

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    // Loading related
    public TextMeshProUGUI connectionInfoText; // just to display connected to server
    public GameObject loadingPanel;
    // Play Button Pressed
    [SerializeField] public Button joinHumanBtn, joinGhostBtn; // for disabled when "totalPlayers" > 1 (With friends)
    // Team member party
    [Range(1,3)] [SerializeField] int totalPlayers = 1; // total amount of player in party
    // Find game related
    public GameObject modalFindGame;
    public TextMeshProUGUI findRoomTimeoutText;
    
    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    public IEnumerator CloseLoadingScreen(float duration){
        yield return new WaitForSeconds(duration);
        if(loadingPanel.activeSelf){
            loadingPanel.SetActive(false);
        }
        StopCoroutine(CloseLoadingScreen(0)); // stop coroutine
    }
}
