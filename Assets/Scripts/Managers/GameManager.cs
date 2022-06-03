using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    [HideInInspector] public List<int> humanSpawnedPosition, ghostSpawnedPosition; 
    
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

         // Set Spawnpoints
        if(PhotonNetwork.IsMasterClient){
            photonView.RPC("SetSpawnpoints", RpcTarget.AllBuffered);
        }

        // Spawn Player
        photonView.RPC("PlayerInGame", RpcTarget.AllBuffered);

    } // end Start

    [PunRPC]
    public void SetSpawnpoints(){ // Host set spawnpoints for human n ghost
        // Set each players spawnPoints
        for(int i = 0; i < (PhotonNetwork.PlayerList.Length - NetworkManager.instance.maxGhostPerGame); i++){
            int posValue = RandomExcept(0, spawnpoints_Human.Count, humanSpawnedPosition);
            humanSpawnedPosition.Add(posValue);
        }

        for(int i = 0; i < (PhotonNetwork.PlayerList.Length - NetworkManager.instance.maxHumanPerGame); i++){
            int posValue2 = RandomExcept(0, spawnpoints_Ghost.Count, ghostSpawnedPosition);
            ghostSpawnedPosition.Add(posValue2);
        }
    } // end SetSpawnpoints

    [PunRPC]
    void PlayerInGame(){ // Check if all player successfully load in the game
        playersInRoom++;
        int humanCount = 0;
        int ghostCount = 0;
        
        if(playersInRoom == PhotonNetwork.PlayerList.Length){
            if(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Human"){
                SpawnPlayers(true, humanCount); // <-- TRUE : We spawn human
                humanCount ++;
            }else if(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Ghost"){
                SpawnPlayers(false, ghostCount); // <-- FALSE : We spawn ghost
                ghostCount++;
            }
        }
    } // end PlayerInGame
    
    void SpawnPlayers(bool isHuman, int playerIndex){
        GameObject player = null;

        if(isHuman){
            if(humanSpawnedPosition.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Player_Male01"), spawnpoints_Human[humanSpawnedPosition[playerIndex]].position, Quaternion.identity);
            }else{
                print("Not ready human");
            }
        }else{
            if(ghostSpawnedPosition.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Ghost_Pocong01"), spawnpoints_Ghost[ghostSpawnedPosition[playerIndex]].position, Quaternion.identity);
            }else{
                 print("Not ready ghost");
            }
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.photonView.RPC("InitializePhotonPlayer", RpcTarget.All, PhotonNetwork.LocalPlayer);
    } // end SpawnPlayers

    int RandomExcept(int min, int max, List<int> except)
    {
        int random = Random.Range(min, max);
        foreach(int nmbr in except){
            if(random >= nmbr){
                random = (random + 1) % max;
            }
        }
        
        return random;
    } // end RandomExcept

}
