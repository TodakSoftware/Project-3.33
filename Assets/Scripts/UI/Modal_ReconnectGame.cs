using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Modal_ReconnectGame : MonoBehaviour
{
    public TextMeshProUGUI reconnectText;
    public Button reconnectButton, leaveButton;

    void Start(){
        reconnectButton.onClick.AddListener(delegate{ReconnectButtonPressed();});
        leaveButton.onClick.AddListener(delegate{LeaveButtonPressed();});
    }

    void ReconnectButtonPressed(){
        reconnectText.text = "Reconnecting...";
        NetworkManager.instance.ReconnectToGame();
        // Close popup
    }

    void LeaveButtonPressed(){
        reconnectText.text = "Leaving room...";
        NetworkManager.instance.CancelFindGameOrLeaveRoom();
        // Destroy(this.gameObject);
    }
}
