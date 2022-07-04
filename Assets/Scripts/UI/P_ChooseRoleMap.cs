using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P_ChooseRoleMap : MonoBehaviour
{
    public Button joinHumanBtn; // for disabled when "totalPlayers" > 1 (With friends)
    public Button joinGhostBtn; 

    // Start is called before the first frame update
    void OnEnable()
    {
        if(MenuManager.instance.joinHumanBtn == null || MenuManager.instance.joinGhostBtn == null){
            MenuManager.instance.joinHumanBtn = joinHumanBtn;
            MenuManager.instance.joinGhostBtn = joinGhostBtn;

            joinHumanBtn.onClick.AddListener(delegate{ NetworkManager.instance.JoinTeam("Human"); });
            joinGhostBtn.onClick.AddListener(delegate{ NetworkManager.instance.JoinTeam("Ghost"); });
        }
    }
}
