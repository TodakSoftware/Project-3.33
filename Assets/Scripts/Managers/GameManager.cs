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
    int totalRitualItem;
    public List<string> filterItemLists = new List<string>();
    // Game Rules

    // Starting Game Related 
    [Header("Spawnpoint Related")]
    public List<Transform> spawnpoints_Human = new List<Transform>();
    public List<Transform> spawnpoints_Ghost = new List<Transform>();
    public List<Transform> spawnpoints_RitualItems = new List<Transform>();
    public List<Transform> spawnpoints_CapturedRoom = new List<Transform>();
    // Unique list (Avoid Duplicate/Double spawn) * Handle by GenerateRandomIndex function
    List<int> selectedHumanSpawnpoints = new List<int>();
    List<int> selectedGhostSpawnpoints = new List<int>();
    List<int> selectedRitualItemSpawnpoints = new List<int>();

    [Header("Timer Related")]
    public bool timeOut;
    int remainingDuration;
    [HideInInspector] public string timerRef;
    public GameObject playerOwned;
    bool isDebugMode;
    
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
        // Set Ritual item (Must run before setting spawnpoints)
        if(PhotonNetwork.CurrentRoom.CustomProperties["GameTotalRitualItem"] != null){
            totalRitualItem = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameTotalRitualItem"];
        }else{
            totalRitualItem = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].totalRitualItems; // amount if ritual item to be spawned. Must less than total ritual item spawnpoints
        }

        // Set Start & End Timer
        if(PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteStart"] != null){
            remainingDuration = (int)PhotonNetwork.CurrentRoom.CustomProperties["GameMinuteStart"] * 60;
        }else{
            remainingDuration = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteStartTime * 60; // 1560 = Starting 26m SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteStartTime
        }

        // Set Spawnpoints
        if(PhotonNetwork.IsMasterClient){
            photonView.RPC("SetSpawnpoints", RpcTarget.AllBuffered);
        }

        // Spawn Player
        photonView.RPC("PlayerInGame", RpcTarget.AllBuffered);
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

        // Generate Unique SpawnPoints
        GenerateRandomIndex("HumanSpawn", maxHuman, spawnpoints_Human.Count);
        GenerateRandomIndex("GhostSpawn", maxGhost, spawnpoints_Ghost.Count);
        GenerateRandomIndex("RitualItem", totalRitualItem, spawnpoints_RitualItems.Count);
        GenerateRandomIndex("CapturedRoom", spawnpoints_CapturedRoom.Count, spawnpoints_CapturedRoom.Count);

        // Check if we are setting only 1 player, then we are debugging
        if(maxHuman == 1 && maxGhost == 0){
            isDebugMode = true;
        }else if(maxHuman == 0 && maxGhost == 1){
            isDebugMode = true;
        }
    } // end SetSpawnpoints

    [PunRPC]
    void PlayerInGame(){ // Check if all player successfully load in the game
        playersInRoom++;
        int humanCount = 0;
        int ghostCount = 0;
        
        if(playersInRoom == PhotonNetwork.PlayerList.Length){
            if(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Human"){
                //SpawnPlayers(true, humanCount); // <-- TRUE : We spawn human
                humanCount ++;
            }else if(PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() == "Ghost"){
                //SpawnPlayers(false, ghostCount); // <-- FALSE : We spawn ghost
                ghostCount++;
            }

            for(int i = 0; i < humanCount; i++){
                SpawnPlayers(true, i);
            }

            for(int y = 0; y < ghostCount; y++){
                SpawnPlayers(false, y);
            }

            if(!gameStart && !isDebugMode){
                gameStart = true;
                // Start Timer
                StartCoroutine(UpdateTimer());
                
                print("Timer Start!");
            }

            // Spawn Ritual Item on Map
            SpawnRitualItemOnMap();
        }
        
    } // end PlayerInGame
    
    void SpawnPlayers(bool isHuman, int playerIndex){
        GameObject player = null;

        if(isHuman){
            if(selectedHumanSpawnpoints.Count > 0 && spawnpoints_Human.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Player_Male01"), spawnpoints_Human[selectedHumanSpawnpoints[playerIndex]].position, Quaternion.identity);
                playerOwned = GameObject.FindObjectOfType<PlayerController>().gameObject;
            }else{
                print("Missing spawnpoints : Human");
            }
        }else{
            if(selectedGhostSpawnpoints.Count > 0 && spawnpoints_Ghost.Count > 0){
                player = PhotonNetwork.Instantiate(NetworkManager.GetPhotonPrefab("Characters", "Ghost_Pocong01"), spawnpoints_Ghost[selectedGhostSpawnpoints[playerIndex]].position, Quaternion.identity);
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

    void SpawnRitualItemOnMap(){
        if(selectedRitualItemSpawnpoints.Count > 0 && spawnpoints_RitualItems.Count > 0){
            for(int i = 0; i < totalRitualItem; i++){
                    var item = PhotonNetwork.Instantiate(GetRitualItemPrefabName(filterItemLists[i].ToString()), spawnpoints_RitualItems[selectedRitualItemSpawnpoints[i]].position, Quaternion.identity);
            }
        }
    }

    string GetRitualItemPrefabName(string itemCode){ // Get itemPrefab Name of the item code
        if(filterItemLists.Count > 0){
            foreach(var go in SOManager.instance.ritualItems.ritualItemLists){
                if(itemCode == go.code){
                    return go.networkItemName;
                }
            }
        }

        return "";
    } // end GetRitualItemPrefabName

    // Handle Random Index Function
    void GenerateRandomIndex(string variableName, int _howMuch, int _total){
        
        if(_howMuch <= _total){
            switch(variableName){
                case "HumanSpawn":
                    while(selectedHumanSpawnpoints.Count < _howMuch && spawnpoints_Human.Count > 0){
                        var randomValue = Random.Range(0, _total);
                        if(!selectedHumanSpawnpoints.Contains(randomValue)){
                            selectedHumanSpawnpoints.Add(randomValue);
                        }
                    }
                break;

                case "GhostSpawn":
                    while(selectedGhostSpawnpoints.Count < _howMuch && spawnpoints_Ghost.Count > 0){
                        var randomValue = Random.Range(0, _total);
                        if(!selectedGhostSpawnpoints.Contains(randomValue)){
                            selectedGhostSpawnpoints.Add(randomValue);
                        }
                    }
                break;

                case "RitualItem":
                    while(selectedRitualItemSpawnpoints.Count < _howMuch && spawnpoints_RitualItems.Count > 0){
                        var randomValue = Random.Range(0, _total);
                        if(!selectedRitualItemSpawnpoints.Contains(randomValue)){
                            selectedRitualItemSpawnpoints.Add(randomValue);
                        }
                    }
                break;

                default:
                    print("Invalid variable name");
                break;
            } // end switch
        }else{
            print("GenerateRandomIndex() Error: " + _howMuch + " > " + _total);
        }
    } //end GenerateRandomIndex


// ---------------------------------------------------- WIN LOSE CONDITION START ----------------------------------------
    [PunRPC]
    public void HumanWin(bool humanWin){
        if(!isDebugMode){
            gameEnded = true;

            if(humanWin){ // display all victory UI saying HUMAN WIN
                UIManager.instance.VictoryUI(true);
            }else{ // // display all victory UI saying GHOST WIN
                UIManager.instance.VictoryUI(false);
            }
        }
    } // end HumanWin / ghost lose

    public void LeaveRoom(){
        if(PhotonNetwork.InRoom){
            PhotonNetwork.LeaveRoom();
        }
    } // end LeaveRoom
// ---------------------------------------------------- WIN LOSE CONDITION END ----------------------------------------


// ---------------------------------------------------- NETWORK RELATED START ----------------------------------------
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if((GetAllPlayersGhost().Count + GetAllPlayersHuman().Count) == PhotonNetwork.PlayerList.Length && gameStart && !gameEnded && PhotonNetwork.IsMasterClient){
            CheckWinningCondition();
        }
    } // end OnPlayerPropertiesUpdate

    public void CheckWinningCondition(){
        if(gameStart && !gameEnded && !isDebugMode){
            // if total human ingame <= 0, Ghost WINS
            if(GetAllPlayersHuman().Count <= 0){
                HumanWin(false);
                print("Ghost win because All Human Rage QUit");
            }
            // if total ghost ingame <= 0, Human WINS
            if(GetAllPlayersGhost().Count <= 0){
                HumanWin(true);
                print("Human win because Ghost Rage QUit");
            }

            // if players captured = total human in game, Ghost WIN
            if(NumberOfCapturedPlayer() >= GetAllPlayersHuman().Count && GetAllPlayersHuman().Count > 0){
                HumanWin(false);
                print("Ghost win because captured human "+ NumberOfCapturedPlayer() +" >= total human("+GetAllPlayersHuman().Count+") in game");
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
        if(GetAllPlayersHuman().Count > 0){
            foreach(GameObject p in GetAllPlayersHuman()){
                if(p.GetPhotonView().Owner.CustomProperties["PlayerCaptured"] != null && (bool)p.GetPhotonView().Owner.CustomProperties["PlayerCaptured"]){
                    capturedCount += 1;
                }
            } // end foreach
        }
        return capturedCount;
    }
// ---------------------------------------------------- NETWORK RELATED END ----------------------------------------

}
