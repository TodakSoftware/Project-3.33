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

    [Header("Connection Related")]
    [SerializeField] bool autoConnect = true; // Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.
    bool connectInUpdate = true; // if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()

    [Header("Find Game Related")]
    float findGameTimeoutDuration; // <- 213 = 3:33 [Controlled by SO_GameSettings]
    int maxHumanPerGame, maxGhostPerGame; // [Controlled by SO_GameSettings]
    byte maxPlayersPerRoom; // max player in a room (Addition of maxHumanPerGame & maxGhostPerGame)
    bool isFindingGame; // Active when we are searching for games.

    [Header("Ingame Related")]
    [Tooltip("0 : Hunt (Normal Mode)")]
    public int gameModeIndex = 0; // default = 0

    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
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
            MenuManager.instance.loadingPanel.SetActive(true); // Display loading screen if not connected
            MenuManager.instance.connectionInfoText.text = "Connecting to server..."; // just to notify we are connection
            PhotonNetwork.ConnectUsingSettings(); // Connect to master server using settings | Noted: ConnectUsingSettings("v0.0.1") <-- Also can
        }

        if(Input.GetKeyDown(KeyCode.H)){ // Temporary, just for debugging
            
        }
    } // end Update

    // ----------------------- PREFABS POOLING RELATED START -------------------
    void InitPrefabPooling(){
        // ------------------------- ADDED PREFABS TO POOL START ----------------------------------------
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        
        // Add Characters Prefabs Lists
        if (pool != null && SOManager.instance.prefabs.characterPrefabs != null)
        {
            foreach (photonPrefabAttributes prefabAtt in SOManager.instance.prefabs.characterPrefabs)
            {
                if(!pool.ResourceCache.ContainsKey(prefabAtt.name))
                pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }

        // Add Props Prefabs Lists
        if (pool != null && SOManager.instance.prefabs.propsPrefabs != null)
        {
            foreach (photonPrefabAttributes prefabAtt in SOManager.instance.prefabs.propsPrefabs)
            {
                if(!pool.ResourceCache.ContainsKey(prefabAtt.name))
                pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }

        // Add Particle Prefabs Lists
        if (pool != null && SOManager.instance.prefabs.particlePrefabs != null)
        {
            foreach (photonPrefabAttributes prefabAtt in SOManager.instance.prefabs.particlePrefabs)
            {
                if(!pool.ResourceCache.ContainsKey(prefabAtt.name))
                pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }
        // ------------------------- ADDED PREFABS TO POOL END ----------------------------------------
    } // end InitPrefabPooling

    public string GetCharactersName(string nama){
        string pref = "";
        foreach(photonPrefabAttributes go in SOManager.instance.prefabs.characterPrefabs){
            if(go.name == nama){
                return go.name;
            }
        }
        return pref;
    } // end GetCharactersName

    public string GetPropsName(string nama){
        string pref = "";
        foreach(photonPrefabAttributes go in SOManager.instance.prefabs.propsPrefabs){
            if(go.name == nama){
                pref = go.name;
            }
        }
        return pref;
    } // end GetPropsName

    public string GetParticlesName(string nama){
        string pref = "";
        foreach(photonPrefabAttributes go in SOManager.instance.prefabs.particlePrefabs){
            if(go.name == nama){
                pref = go.name;
            }
        }
        return pref;
    } // end GetParticlesName
    // ----------------------- PREFABS POOLING RELATED END -------------------


    // ----------------------- CONNECTION RELATED START -------------------
    public override void OnConnectedToMaster(){
        MenuManager.instance.connectionInfoText.text = "Successfully Connected!";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
        connectInUpdate = true;
    } // end OnConnectedToMaster

    public override void OnJoinedLobby(){
        // Close loading popup
        StartCoroutine(MenuManager.instance.CloseLoadingScreen(1f));
        // Enable AutoSyncScene
        PhotonNetwork.AutomaticallySyncScene = true;
    } // end OnJoinedLobby
    
    public override void OnJoinedRoom(){ // Only host affected by this
        print("Successfully join a room. Waiting for others to fill in");
        UpdateTotalFindGame(); // Update total players in room
        MenuManager.instance.cancelFindGameBtn.interactable = true; // Enable cancel find game button when joined
        MenuManager.instance.coroutinefindRoomTimeout = StartCoroutine(MenuManager.instance.UpdateUI_FindgameTimeout(findGameTimeoutDuration)); // Timeout duration updates
    } // end OnJoinedRoom

    public override void OnJoinRandomFailed(short returnCode, string message){ // we create new room with this
        print("No available room found! Creating...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PlayerTtl = -1; // -1 sec for infinite : Duration for player to reconnect before kicked / timeout
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        // put map here also

        // #Critical: we failed to join a random room, maybe none exists or they are all full. We create a new room.
        PhotonNetwork.CreateRoom(null, roomOptions);
    } // end OnJoinRandomFailed

    public override void OnCreatedRoom(){ // Create default room properties on 1st created room
        if(PhotonNetwork.IsMasterClient){
            Hashtable roomProperties = new Hashtable();
            roomProperties.Add("networkTotalHuman", 0);
            roomProperties.Add("networkTotalGhost", 0);
            roomProperties.Add("CurrentItemContributed", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            string[] exposedPropertiesInLobby = { "networkTotalHuman", "networkTotalGhost" }; // can set map here aswell
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
        // Reset find game timeout timer
        MenuManager.instance.findRoomTimeoutText.text = "0:00";
        StopCoroutine(MenuManager.instance.coroutinefindRoomTimeout);
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
            int totalHuman = 0;
            int totalGhost = 0;
            foreach(var player in PhotonNetwork.CurrentRoom.Players){
                if(player.Value.CustomProperties["Team"].ToString() == "Human"){
                    totalHuman += 1;
                }else{
                    totalGhost += 1;
                }
            }

            // Update room properties when someone joined a room
            if(PhotonNetwork.IsMasterClient){
                Hashtable roomProperties = new Hashtable();
                roomProperties.Add("networkTotalHuman", totalHuman);
                roomProperties.Add("networkTotalGhost", totalGhost);
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }

            // If a room match all requirement, Host responsible to change the scene
            if(totalHuman == maxHumanPerGame && totalGhost == maxGhostPerGame && isFindingGame){ // Only do this when we are finding game
                if(PhotonNetwork.IsMasterClient){
                    PhotonNetwork.CurrentRoom.IsVisible = false; // Set Room IsVisible = false
                    ChangeSceneAsync("TestMapGaban"); // Host load level async
                }else{
                    // Popup Loading UI on non Host
                    var canvas = GameObject.FindGameObjectWithTag("MainCanvas");
                    var loading = Instantiate(SOManager.instance.prefabs.modalLoadingScene);
                    loading.transform.SetParent(canvas.transform, false);
                }

                isFindingGame = false; // Set status to isFindingGame
            }

            if(isFindingGame){ // Only do this when we are finding game
                MenuManager.instance.UpdateUI_FindgameTotal(totalHuman, totalGhost);
            }
        } // end PhotonNetwork.InRoom

    } // end UpdateTotalFindGame
    
    public void JoinTeam(string team){ // Used by buttons in ChooseRole Screen
        Hashtable playerProperties = new Hashtable();
        Hashtable expectedRoomProperties = new Hashtable();
        MenuManager.instance.findGamePanel.SetActive(true); // Popup findgame UI
        isFindingGame = true; // Set status to isFindingGame

            switch(team){
                case "Human":
                    print("Join as human");

                    // Set player team properties
                    playerProperties.Add("Team", "Human");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

                    // ExpectedCustomRoom properties. Example, Human search room with totalGhost = 1
                    expectedRoomProperties["networkTotalGhost"] = maxGhostPerGame;
                break;

                case "Ghost":
                    print("Join as ghost");

                    // Set player team properties
                    playerProperties.Add("Team", "Ghost");
                    PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

                    // ExpectedCustomRoom properties. Example, Ghost search room with totalHuman = 3
                    expectedRoomProperties["networkTotalHuman"] = maxHumanPerGame;
                break;

                default:
                    print("Pardon? Unassigned value on button. Please Check function (JoinTeam)");
                break;
            } // end switch(team)

        // Join Random Room With expected properties
        if(!PhotonNetwork.InRoom){
            PhotonNetwork.JoinRandomRoom(expectedRoomProperties, maxPlayersPerRoom);
            MenuManager.instance.cancelFindGameBtn.interactable = false;
        }else{
            print("You are still in room. Please wait.");
        }

    } // end JoinTeam

    public void CancelFindGameOrLeaveRoom(){ // Cancel while finding game or Leave Room. Used by cancel button in Modal_Findgame
        // Makesure we are in a room
        if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom){
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
            PhotonNetwork.LoadLevel(sceneName);
        }
    } // end ChangeScene

    public void ChangeSceneAsync(string sceneName){ // Load level using async with loading screen
        foreach(var map in SOManager.instance.maps.mapsList){
            if(map.sceneName == sceneName){
                StartCoroutine(LoadSceneAsync(map.buildIndex));
            }else{
                print("Invalid Scene Name");
            }
        }
    } // end ChangeSceneAsync

    IEnumerator LoadSceneAsync(int sceneId){ // Handles the async load level
        float timer = 0;
        float delay = 3f; // Wait 3 seconds before proceed
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        operation.allowSceneActivation = false;
        // Load prefab : Modal_LoadingScene | Can add a bar to check for progress, etc.
        var canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        var loading = Instantiate(SOManager.instance.prefabs.modalLoadingScene);
        loading.transform.SetParent(canvas.transform, false);

        while(!operation.isDone){
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            timer += Time.deltaTime;

            if(timer >= delay && progressValue >= 1){
                timer = delay;
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    } // end LoadSceneAsync

    // ----------------------- INGAME RELATED END -------------------
}
