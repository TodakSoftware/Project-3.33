using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public bool gameStart;
    public bool gameEnded;
    // Game Rules
    int clockStartTime, clockEndTime; // [Controlled by SO_GameSettings]
    public int currentItemContributed; // Will link with Photon Customproperties ["CurrentItemContributed"]
    int totalRitualItems; // [Controlled by SO_GameSettings]

    // Starting Game Related 
    public List<Transform> spawnpoints_Human = new List<Transform>();
    public List<Transform> spawnpoints_Ghost = new List<Transform>();
    public List<Transform> spawnpoints_RitualItems = new List<Transform>();
    public List<Transform> spawnpoints_CapturedRoom = new List<Transform>();
    
    // Ingame Related
    int playersInRoom; // Number of players successfully enter the room. For comparing with total of network player list
    public PlayerController[] playerControllerArray; // List of PlayerController[Array] for all players in the game

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    } // end Awake

    void Start(){
        playerControllerArray = new PlayerController[PhotonNetwork.PlayerList.Length];

        // Linking data via Game Settings Scriptable Objects
        clockStartTime = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].clockStartTime;
        clockEndTime = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].clockEndTime;
        totalRitualItems = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].totalRitualItems;

        // Spawn Player
        photonView.RPC("PlayerInGame", RpcTarget.AllBuffered);
    } // end Start

    [PunRPC]
    void PlayerInGame(){ // Check if all player successfully load in the game
        playersInRoom++;
        if(playersInRoom == PhotonNetwork.PlayerList.Length){
            if(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Human"){
                SpawnPlayers(true); // <-- TRUE : We spawn human
            }else if(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Ghost"){
                SpawnPlayers(false); // <-- FALSE : We spawn ghost
            }
        }
    } // end PlayerInGame
    
    public void SpawnPlayers(bool isHuman){
        GameObject player = null;

        if(isHuman){
            player = PhotonNetwork.Instantiate(NetworkManager.instance.GetCharactersName("Player_Male01"), spawnpoints_Human[0].position, Quaternion.identity);
        }else{
            player = PhotonNetwork.Instantiate(NetworkManager.instance.GetCharactersName("Player_Male01"), spawnpoints_Ghost[0].position, Quaternion.identity);
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.photonView.RPC("InitializePhotonPlayer", RpcTarget.All, PhotonNetwork.LocalPlayer);
    } // end SpawnPlayers
}
