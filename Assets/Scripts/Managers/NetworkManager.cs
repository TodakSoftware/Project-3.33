// For handling network related

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Player = Photon.Realtime.Player;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    DefaultPool pool;

    [Header("Connection Related")]
    [SerializeField] bool autoConnect = true; // Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.
    bool connectInUpdate = true; // if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()
    public bool isDisconnectedWhileGame; // status for checking if player disconnected while in game. Can be used for reconnecting & rejoin game. (Destroy Reconnect UI when successfully reconnect)

    [Header("Find Game Related")]
    float findGameTimeoutDuration; // <- 213 = 3:33 [Controlled by SO_GameSettings]
    [HideInInspector] public int maxHumanPerGame, maxGhostPerGame; // [Controlled by SO_GameSettings]
    byte maxPlayersPerRoom; // max player in a room (Addition of maxHumanPerGame & maxGhostPerGame)
    bool isFindingGame; // Active when we are searching for games.

    [Header("Ingame Related")]
    [Tooltip("0 : Hunt (Normal Mode)")]
    public int gameModeIndex = 0; // default = 0
    public string gameMapName = "Demo";

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        pool = PhotonNetwork.PrefabPool as DefaultPool;
        InitPrefabPooling(); // Start object pooling for photon gameobjects

        // Linking data via Game Settings Scriptable Objects
        findGameTimeoutDuration = SOManager.instance.gameSettings.gameMode[gameModeIndex].findGameTimeoutDuration;
        maxHumanPerGame = SOManager.instance.gameSettings.gameMode[gameModeIndex].maxHumanPerGame;
        maxGhostPerGame = SOManager.instance.gameSettings.gameMode[gameModeIndex].maxGhostPerGame;
        maxPlayersPerRoom = (byte)(maxHumanPerGame + maxGhostPerGame); // Set maxPlayersPerRoom for photon references
    }

    void Update(){
        // Reason putting in Update() & not Start() because to make sure we keep checking if we are not connected
        if(connectInUpdate && autoConnect && !PhotonNetwork.IsConnected){
            connectInUpdate = false; // run only once
            UIManager.instance.PopupLoadingNormal("Loading", "Connecting to server..."); // Popup Loading UI
            PhotonNetwork.ConnectUsingSettings(); // Connect to master server using settings | Noted: ConnectUsingSettings("v0.0.1") <-- Also can
        }

        if(Input.GetKeyDown(KeyCode.H)){ // Temporary, just for debugging
            UIManager.instance.PopupReconnectGame();
        }
    } // end Update

    // ----------------------- PREFABS POOLING RELATED START -------------------
    void InitPrefabPooling(){
        AddPrefabPool(SOManager.instance.prefabs.characterPrefabs); // Add Characters Prefabs Lists
        AddPrefabPool(SOManager.instance.prefabs.propsPrefabs); // Add Characters Prefabs Lists
        AddPrefabPool(SOManager.instance.prefabs.particlePrefabs); // Add Particle Prefabs Lists
    } // end InitPrefabPooling

    public void AddPrefabPool(List<C_PhotonPrefabAttributes> prefabAttributes)
    {
        if (pool != null && prefabAttributes != null)
        {
            foreach (C_PhotonPrefabAttributes prefabAtt in prefabAttributes)
            {
                if (!pool.ResourceCache.ContainsKey(prefabAtt.name))
                    pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }
    } // end AddPrefabPool

    public static string GetPhotonPrefab(string category, string prefabName){
        string pref = "";
        List<C_PhotonPrefabAttributes> photonPrefabsList = new List<C_PhotonPrefabAttributes>();
        photonPrefabsList.Clear(); // clear if not empty

        switch(category){
            case "Characters":
                photonPrefabsList = SOManager.instance.prefabs.characterPrefabs;
            break;

            case "Props":
                photonPrefabsList = SOManager.instance.prefabs.propsPrefabs;
            break;

            case "Particles":
                photonPrefabsList = SOManager.instance.prefabs.particlePrefabs;
            break;

            default:
                photonPrefabsList = null;
            break;
        }

        // Search for matching prefabs in the lists
        foreach(C_PhotonPrefabAttributes go in photonPrefabsList){
            if(go.name == prefabName){
                return go.name; // return this value if found
            }
        }

        return pref; // return default pref if not found
        
    } // end GetPhotonPrefab

    // ----------------------- PREFABS POOLING RELATED END -------------------


    // ----------------------- CONNECTION RELATED START -------------------
    public override void OnConnectedToMaster(){
        UIManager.instance.UpdateLoadingNormal("Loading", "Successfully Connected!"); // Popup Loading
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        connectInUpdate = true;
    } // end OnConnectedToMaster

    public override void OnJoinedLobby(){
        StartCoroutine(UIManager.instance.CloseNormalLoading(1f)); // Close loading popup
        PhotonNetwork.AutomaticallySyncScene = true; // Enable AutoSyncScene
    } // end OnJoinedLobby
    
    public override void OnJoinedRoom(){ // Only host affected by this
        print("Successfully join a room. Waiting for others to fill in");

        UpdateTotalFindGame(); // Update total players in room
        UIManager.instance.activeFindgameCancel(true); // Enable cancel find game button when joined
        UIManager.instance.modalFindGame.coroutinefindRoomTimeout = StartCoroutine(UIManager.instance.UpdateUI_FindgameTimeout(findGameTimeoutDuration)); // Timeout duration updates
    } // end OnJoinedRoom

    public override void OnJoinRandomFailed(short returnCode, string message){ // we create new room with this
        print("No available room found! Creating...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PlayerTtl = -1; // -1 sec for infinite : Duration for player to reconnect before kicked / timeout
        roomOptions.MaxPlayers = maxPlayersPerRoom;

        // #Critical: we failed to join a random room, maybe none exists or they are all full. We create a new room.
        PhotonNetwork.CreateRoom(null, roomOptions);
    } // end OnJoinRandomFailed

    public override void OnCreatedRoom(){ // Create default room properties on 1st created room
        if(PhotonNetwork.IsMasterClient){ // Set room properties after we are in a room
            Hashtable roomProperties = new Hashtable();
            roomProperties.Add("RoomTotalMaxHuman", maxHumanPerGame);
            roomProperties.Add("RoomTotalMaxGhost", maxGhostPerGame);
            roomProperties.Add("RoomGamemodeIndex", gameModeIndex);
            roomProperties.Add("RoomMapName", gameMapName); // Random map
            roomProperties.Add("RoomFullHuman", false);
            roomProperties.Add("RoomFullGhost", false);
            roomProperties.Add("GameItemContributed", 0);
            roomProperties.Add("GameTotalRitualItem", SOManager.instance.gameSettings.gameMode[gameModeIndex].totalRitualItems);
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            string[] exposedPropertiesInLobby = { "RoomTotalMaxHuman", "RoomTotalMaxGhost", "RoomGamemodeIndex", "RoomMapName", "RoomFullHuman", "RoomFullGhost" }; 
            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(exposedPropertiesInLobby);
        }
        
        print("Create room : " + PhotonNetwork.CurrentRoom.Name);
    } // end OnCreatedRoom

    public override void OnPlayerEnteredRoom(Player newPlayer){ // When non host player enter a room. "Display / Update UI"
        UpdateTotalFindGame();
    } // end OnPlayerEnteredRoom

    public override void OnPlayerLeftRoom(Player otherPlayer){ // When player cancel find game or after leave a room
        print("Player Left Room");
        Invoke("UpdateTotalFindGame", 1f);
    } // end OnPlayerLeftRoom

    public override void OnLeftRoom(){ // When player successfully left the room
        if(UIManager.instance.modalFindGame.coroutinefindRoomTimeout != null){
            StopCoroutine(UIManager.instance.modalFindGame.coroutinefindRoomTimeout); // Stop running courotine
        }

        if(!isFindingGame){
            //PhotonNetwork.Disconnect();
            // load back to mainmenu
            SceneManager.LoadScene("MainMenu");
        }
    } // end OnLeftRoom

    public override void OnDisconnected(DisconnectCause cause){
        print("Error : " + cause);
        //DisconnectCause.DisconnectByClientLogic // <--- Player close the game suddenly
        // if disconnect suddenly -> Popup Reconnect UI Prefab
        // if internet not reachable -> loading screen will popup & reconnect
    }

    // ----------------------- CONNECTION RELATED END -------------------


    // ----------------------- FIND GAME RELATED START -------------------
    void UpdateTotalFindGame(){ // Update room properties when player enters | Start the game here
        if(PhotonNetwork.InRoom){
            // Cache Network Room Variable
            int _roomTotalMaxHuman = 0;
            int _roomTotalMaxGhost = 0;
            string _roomMapName = "";

            if(PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxHuman"] != null && PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxGhost"] != null){
                _roomTotalMaxHuman = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxHuman"];
                _roomTotalMaxGhost = (int)PhotonNetwork.CurrentRoom.CustomProperties["RoomTotalMaxGhost"];
            }else{
                _roomTotalMaxHuman = maxHumanPerGame;
                _roomTotalMaxGhost = maxGhostPerGame;
            }
            
            if(PhotonNetwork.CurrentRoom.CustomProperties["RoomMapName"] != null){
                _roomMapName = PhotonNetwork.CurrentRoom.CustomProperties["RoomMapName"].ToString();
            } else {

                _roomMapName=gameMapName;
            }

            
            // Local variable for room count
            int _totalHuman = 0;
            int _totalGhost = 0;
            foreach(var player in PhotonNetwork.CurrentRoom.Players){
                if(player.Value.CustomProperties["Team"].ToString() == "Human"){
                    _totalHuman += 1;
                }else{
                    _totalGhost += 1;
                }
            }

            // Update room properties when someone joined a room
            if(PhotonNetwork.IsMasterClient){
                Hashtable roomProperties = new Hashtable();
                // Human
                if(_totalHuman >= _roomTotalMaxHuman){
                    roomProperties.Add("RoomFullHuman", true);
                }else{
                    roomProperties.Add("RoomFullHuman", false);
                }

                // Ghost
                if(_totalGhost >= _roomTotalMaxGhost){
                    roomProperties.Add("RoomFullGhost", true);
                }else{
                    roomProperties.Add("RoomFullGhost", false);
                }

                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            } // IsMasterClient

            // If a room match all requirement, Host responsible to change the scene
            if(_totalHuman == _roomTotalMaxHuman && _totalGhost == _roomTotalMaxGhost && isFindingGame){ // Only do this when we are finding game
                UIManager.instance.PopupLoadingScene(); // Popup Loading Scene UI

                if(PhotonNetwork.IsMasterClient){
                    PhotonNetwork.CurrentRoom.IsVisible = false; // Set Room IsVisible = false
                    ChangeScene(_roomMapName); // Host load level async
                }

                isFindingGame = false; // Set status to isFindingGame
            } // end _totalHuman == roomTotalMaxHuman

            if(isFindingGame){ // Only do this when we are finding game
                UIManager.instance.UpdateUI_FindgameTotal(_totalHuman, _roomTotalMaxHuman, _totalGhost, _roomTotalMaxGhost);
            } // end isFindingGame

        } // end PhotonNetwork.InRoom

    } // end UpdateTotalFindGame
    
    public void JoinTeam(string team){ // Used by buttons in ChooseRole Screen
        Hashtable playerProperties = new Hashtable();
        Hashtable expectedRoomProperties = new Hashtable();
        UIManager.instance.PopupFindGame();
        isFindingGame = true; // Set status to isFindingGame

            switch(team){
                case "Human":
                    print("Join as human");

                    // Set player team properties
                    playerProperties.Add("Team", "Human");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

                    // ExpectedCustomRoom properties. Example, Human search room that where human is not full
                    expectedRoomProperties["RoomFullHuman"] = false;
                break;

                case "Ghost":
                    print("Join as ghost");

                    // Set player team properties
                    playerProperties.Add("Team", "Ghost");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

                    // ExpectedCustomRoom properties. Example, Ghost search room that where ghost is not full
                    expectedRoomProperties["RoomFullGhost"] = false;
                break;

                default:
                    print("Pardon? Unassigned value on button. Please Check function (JoinTeam)");
                break;
            } // end switch(team)

        // Join Random Room With expected properties
        if(!PhotonNetwork.InRoom){
            PhotonNetwork.JoinRandomRoom(expectedRoomProperties, maxPlayersPerRoom);
            UIManager.instance.activeFindgameCancel(false);
        }else{
            print("You are still in room. Please wait.");
        }

    } // end JoinTeam

    public void CancelFindGameOrLeaveRoom(){ // Cancel while finding game or Leave Room. Used by cancel button in Modal_Findgame
        // Makesure we are in a room
        if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom){
            print("Player cancelling find game / leave room");
            isFindingGame = false; // Set status to isFindingGame
            PhotonNetwork.LeaveRoom();
        }else{
            print("Not ready yet to cancel or leave.");
        }
    } // end CancelFindGameOrLeaveRoom

    // ----------------------- FIND GAME RELATED END -------------------

    
    // ----------------------- INGAME RELATED START -------------------
    public void ReconnectToGame(){ // Attached into reconnect button on Reconnect Screen Popup
        if(PhotonNetwork.IsConnectedAndReady){
            PhotonNetwork.ReconnectAndRejoin(); // only works when player doesnt call Photon.LeaveRoom() or disconnect suddenly
        }else{
            print("Not ready to reconnect into the game");
        }
    } // end ReconnectToGame

    public void ChangeScene(string sceneName){ // Load level instantly without loading screen
        if(PhotonNetwork.IsMasterClient){
            if(sceneName == "MainMenu"){
                if(PhotonNetwork.IsConnectedAndReady){
                    PhotonNetwork.LeaveRoom();
                }
                PhotonNetwork.LoadLevel(sceneName);
            }else{
                PhotonNetwork.LoadLevel(sceneName);
            }
            //print(sceneName);
        }
    } // end ChangeScene

    // ----------------------- INGAME RELATED END -------------------
}
