// Handle menu related. Example, Game Settings, Pause Menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Misc. Related")]
    public TextMeshProUGUI versionText;

    [Header("Find Game Related")]
    public Button joinHumanBtn; // for disabled when "totalPlayers" > 1 (With friends)
    public Button joinGhostBtn; 
    
    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        // Set Version
        versionText.text = "v"+Application.version;

        // Set fps Limit = 60fps
        Application.targetFrameRate = 60;
    }

    

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
