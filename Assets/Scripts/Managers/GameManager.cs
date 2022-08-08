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
    public GameObject playerOwned;
    
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

        if(PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteStart"] != null){
            remainingDuration = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteStart"] * 60;
        }else{
            remainingDuration = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteStartTime * 60; // 1560 = Starting 26m SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteStartTime
        }
        
        

    } // end Start

    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------
    private void ResetTimer(){
        remainingDuration = 0;
    }

    private IEnumerator UpdateTimer(){
        int _endMinute = 0;
        if(PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteEnd"] != null){
            _endMinute = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteEnd"];
        }else{
             _endMinute = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteEndTime;
        }

        while(remainingDuration > 0 && remainingDuration <= (_endMinute * 60)){ // 1980 = 33m
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
        int maxHuman = 0;
        int maxGhost = 0;
        if(PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxHuman"] != null){
            maxHuman = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxHuman"];
        }else{
            maxHuman = NetworkManager.instance.maxHumanPerGame;
        }
        
        if(PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxGhost"] != null){
            maxGhost = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxGhost"];
        }else{
            maxGhost = NetworkManager.instance.maxGhostPerGame;
        }
        // Set each players spawnPoints

        if(maxHuman > 0){
            for(int i = 0; i < (PhotonNetwork.PlayerList.Length - maxGhost); i++){
                int posValue = RandomExcept(0, spawnpoints_Human.Count, humanSpawnedPosition);
                humanSpawnedPosition.Add(posValue);
            }
        }
        
        if(maxGhost > 0){
            for(int i = 0; i < (PhotonNetwork.PlayerList.Length - maxHuman); i++){
                int posValue2 = RandomExcept(0, spawnpoints_Ghost.Count, ghostSpawnedPosition);
                ghostSpawnedPosition.Add(posValue2);
            }
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

            if(!gameStart){
                gameStart = true;
                // Start Timer
                StartCoroutine(UpdateTimer());
                print("Timer Start!");
            }
        }
        
    } // end PlayerInGame
    
    void SpawnPlayers(bool isHuman, int playerIndex){
        GameObject player = null;

        if(isHuman){
            if(humanSpawnedPosition.Count > 0 && spawnpoints_Human.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Player_Male01"), spawnpoints_Human[humanSpawnedPosition[playerIndex]].position, Quaternion.identity);
                playerOwned = GameObject.FindObjectOfType<PlayerController>().gameObject;
            }else{
                print("Missing spawnpoints : Human");
            }
        }else{
            if(ghostSpawnedPosition.Count > 0 && spawnpoints_Human.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Ghost_Pocong01"), spawnpoints_Ghost[ghostSpawnedPosition[playerIndex]].position, Quaternion.identity);
                playerOwned = GameObject.FindObjectOfType<PlayerController>().gameObject;
            }else{
                print("Missing spawnpoints : Ghost");
            }
        }

        if(player != null && photonView.IsMine){
            Hashtable playerProps = new Hashtable();
            playerProps.Add("PlayerCaptured", false);
            playerProps.Add("PlayerIsDead", false);
            player.GetPhotonView().Owner.SetCustomProperties(playerProps);
        }
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

        if((GetAllPlayersGhost().Count + GetAllPlayersHuman().Count) == PhotonNetwork.PlayerList.Length && !gameEnded){
            CheckWinningCondition();
            

        }
    } // end OnPlayerPropertiesUpdate

    public void CheckWinningCondition(){
        if(!gameEnded){
            // if total human ingame <= 0, Ghost WINS
            if(GetAllPlayersHuman().Count <= 0){
                HumanWin(false);
            }
            // if total ghost ingame <= 0, Human WINS
            if(GetAllPlayersGhost().Count <= 0){
                HumanWin(true);
            }

            // if players captured = total human in game, Ghost WIN
            if(NumberOfCapturedPlayer() >= GetAllPlayersHuman().Count){
                HumanWin(false);
            }
        } // if !gameEnded
    } // end CheckWinningCondition()

    public static List<GameObject> GetAllPlayersGhost(){ // Return list of players GAMEOBJECT
        List<GameObject> ghosts = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ghost"));

        return ghosts;
    } // end GetAllPlayersRobber

    public static List<GameObject> GetAllPlayersHuman(){ // Return list of players GAMEOBJECT
        List<GameObject> humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        return humans;
    } // end GetAllPlayersRobber

    public int NumberOfCapturedPlayer(){
        int capturedCount = 0;
        foreach(GameObject p in GetAllPlayersHuman()){
            if(p.GetPhotonView().Owner.CustomProperties["PlayerCaptured"] != null && (bool)p.GetPhotonView().Owner.CustomProperties["PlayerCaptured"]){
                capturedCount += 1;
            }
        } // end foreach
        return capturedCount;
    }
// ---------------------------------------------------- NETWORK RELATED END ----------------------------------------

}
