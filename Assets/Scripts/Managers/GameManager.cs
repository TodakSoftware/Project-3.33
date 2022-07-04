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
    public int currentItemContributed; // Will replaced with Photon Customproperties ["GameItemContributed"]
    [HideInInspector] public int totalRitualItems; // [Controlled by SO_GameSettings]

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
    [HideInInspector] public PlayerController[] playerControllerArray; // List of PlayerController[Array] for all players in the game

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
        totalRitualItems = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].totalRitualItems;

         // Set Spawnpoints
        if(PhotonNetwork.IsMasterClient){
            photonView.RPC("SetSpawnpoints", RpcTarget.AllBuffered);
        }

        // Spawn Player
        photonView.RPC("PlayerInGame", RpcTarget.AllBuffered);

        remainingDuration = SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteStartTime * 60; // 1560 = Starting 26m
        StartCoroutine(UpdateTimer());

    } // end Start

    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------
    private void ResetTimer(){
        remainingDuration = 0;
    }

    private IEnumerator UpdateTimer(){
        while(remainingDuration > 0 && remainingDuration <= (SOManager.instance.gameSettings.gameMode[NetworkManager.instance.gameModeIndex].minuteEndTime * 60)){ // 1980 = 33m
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
        HumanWin(false);
        ResetTimer();
    }

    private void OnDestroy(){
        StopAllCoroutines();
    }
    // --------------------------------- CLOCK TIMER FUNCTION START ----------------------------------

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


// ---------------------------------------------------- WIN LOSE CONDITION START ----------------------------------------
    public void HumanWin(bool humanWin){
        gameEnded = true;
        
        List<GameObject> humans = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        List<GameObject> ghosts = new List<GameObject>(GameObject.FindGameObjectsWithTag("Ghost"));

        if(humanWin){ // display all victory UI saying HUMAN WIN
            foreach(var h in humans){
                h.GetComponent<PlayerUI>().uiVictoryResult.gameObject.SetActive(true);
                h.GetComponent<PlayerUI>().uiVictoryResult.HumanWin();
                
            }

            foreach(var g in ghosts){
                g.GetComponent<PlayerUI>().uiVictoryResult.gameObject.SetActive(true);
                g.GetComponent<PlayerUI>().uiVictoryResult.HumanWin();
                
            }
        }else{ // // display all victory UI saying GHOST WIN
            foreach(var h in humans){
                h.GetComponent<PlayerUI>().uiVictoryResult.gameObject.SetActive(true);
                h.GetComponent<PlayerUI>().uiVictoryResult.GhostWin();
                
            }

            foreach(var g in ghosts){
                g.GetComponent<PlayerUI>().uiVictoryResult.gameObject.SetActive(true);
                g.GetComponent<PlayerUI>().uiVictoryResult.GhostWin();
                
            }
        }
    } // end HumanWin
// ---------------------------------------------------- WIN LOSE CONDITION END ----------------------------------------

}
