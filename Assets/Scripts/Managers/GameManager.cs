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
    [HideInInspector] public bool gameStart;
    [HideInInspector] public bool gameEnded;
    // Game Rules

    // Starting Game Related 
    public List<Transform> spawnpoints_Human = new List<Transform>();
    public List<Transform> spawnpoints_Ghost = new List<Transform>();
    public List<Transform> spawnpoints_RitualItems = new List<Transform>();
    public List<Transform> spawnpoints_CapturedRoom = new List<Transform>();
    [HideInInspector] public List<int> humanSpawnedPosition, ghostSpawnedPosition; 

    [Header("Timer Related")]
    public bool timeOut;
    int remainingDuration;
    [HideInInspector] public string timerRef;
    
    // Ingame Related
    int playersInRoom; // Number of players successfully enter the room. For comparing with total of network player list

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
    } // end Awake

    void Start(){

        // Linking data via Game Settings Scriptable Objects

        // Set Spawnpoints
        if(PhotonNetwork.IsMasterClient){
            photonView.RPC("SetSpawnpoints", RpcTarget.AllBuffered);
        }

        // Spawn Player
        photonView.RPC("PlayerInGame", RpcTarget.AllBuffered);

        remainingDuration = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteStart"] * 60; // 1560 = Starting 26m SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteStartTime
        StartCoroutine(UpdateTimer());

    } // end Start

    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------
    private void ResetTimer(){
        remainingDuration = 0;
    }

    private IEnumerator UpdateTimer(){
        while(remainingDuration > 0 && remainingDuration <= ((int)PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteEnd"] * 60)){ // 1980 = 33m
            UpdateTimerUI(remainingDuration);
            remainingDuration++;
            yield return new WaitForSeconds(1f);
        }
        EndTimer();
    }

    private void UpdateTimerUI(int seconds){
        //print(string.Format("3:{0:D2}:{1:D2}", seconds/60, seconds % 60));
        timerRef = "3<color=red>:</color>" + string.Format("{0:D2}", seconds/60) + "<size=30><color=red>:</color>" + string.Format("{0:D2}", seconds % 60) + "</size>";
    }

    public void EndTimer(){
        timeOut = true;
        photonView.RPC("HumanWin", RpcTarget.All, false); //HumanWin(false);
        ResetTimer();
    }

    private void OnDestroy(){
        StopAllCoroutines();
    }
    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------

    [PunRPC]
    public void SetSpawnpoints(){ // Host set spawnpoints for human n ghost
        int maxHuman = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxHuman"];
        int maxGhost = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxGhost"];
        // Set each players spawnPoints
        for(int i = 0; i < (PhotonNetwork.PlayerList.Length - maxGhost); i++){
            int posValue = RandomExcept(0, spawnpoints_Human.Count, humanSpawnedPosition);
            humanSpawnedPosition.Add(posValue);
        }

        for(int i = 0; i < (PhotonNetwork.PlayerList.Length - maxHuman); i++){
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
            if(humanSpawnedPosition.Count > 0 && spawnpoints_Human.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Player_Male01"), spawnpoints_Human[humanSpawnedPosition[playerIndex]].position, Quaternion.identity);
            }else{
                print("Missing spawnpoints : Human");
            }
        }else{
            if(ghostSpawnedPosition.Count > 0 && spawnpoints_Human.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Ghost_Pocong01"), spawnpoints_Ghost[ghostSpawnedPosition[playerIndex]].position, Quaternion.identity);
            }else{
                 print("Missing spawnpoints : Ghost");
            }
        }

        PlayerController playerController = player.GetComponent<PlayerController>();
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


// ---------------------------------------------------- WIN LOSE CONDITION START ----------------------------------------
    [PunRPC]
    public void HumanWin(bool humanWin){
        gameEnded = true;
        
        if(humanWin){ // display all victory UI saying HUMAN WIN
            UIManager.instance.VictoryUI(true);
        }else{ // // display all victory UI saying GHOST WIN
            UIManager.instance.VictoryUI(false);
        }
    } // end HumanWin

    public void LeaveRoom(){
        if(PhotonNetwork.InRoom){
            PhotonNetwork.LeaveRoom();
        }
    }
// ---------------------------------------------------- WIN LOSE CONDITION END ----------------------------------------


// ---------------------------------------------------- NETWORK RELATED START ----------------------------------------
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

    } // end OnPlayerPropertiesUpdate
// ---------------------------------------------------- NETWORK RELATED END ----------------------------------------

}
