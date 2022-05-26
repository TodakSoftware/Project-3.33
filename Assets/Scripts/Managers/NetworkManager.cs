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

[System.Serializable]
public class prefabAttributes{
    public string name;
    public GameObject prefabs;
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    
    [Header("Prefabs Pooling")]
    public List<prefabAttributes> characterPrefabs;
    public List<prefabAttributes> propsPrefabs;
    public List<prefabAttributes> particlePrefabs;

    [Header("Connection Related")]
    [SerializeField] bool autoConnect = true; // Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.
    bool connectInUpdate = true; // if we don't want to connect in Start(), we have to "remember" if we called ConnectUsingSettings()

    [Header("Find Game Related")]
    [SerializeField] private byte maxPlayersPerRoom = 4;


    void Awake(){
        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        InitPrefabPooling(); // Start object pooling for photon gameobjects
    }

    void Update(){
        // Reason putting in Update() & not Start() because to make sure we keep checking is we are not connected
        if(connectInUpdate && autoConnect && !PhotonNetwork.IsConnected){
            connectInUpdate = false; // run only once
            MenuManager.instance.loadingPanel.SetActive(true); // Display loading screen if not connected
            MenuManager.instance.connectionInfoText.text = "Connecting to server..."; // just to notify we are connection
            PhotonNetwork.ConnectUsingSettings(); // Connect to master server using settings
        }
    }


    // ----------------------- PREFABS POOLING RELATED START -------------------
    void InitPrefabPooling(){
        // ------------------------- ADDED PREFABS TO POOL START ----------------------------------------
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
        
        // Add Characters Prefabs Lists
        if (pool != null && this.characterPrefabs != null)
        {
            foreach (prefabAttributes prefabAtt in this.characterPrefabs)
            {
                if(!pool.ResourceCache.ContainsKey(prefabAtt.name))
                pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }

        // Add Props Prefabs Lists
        if (pool != null && this.propsPrefabs != null)
        {
            foreach (prefabAttributes prefabAtt in this.propsPrefabs)
            {
                if(!pool.ResourceCache.ContainsKey(prefabAtt.name))
                pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }

        // Add Particle Prefabs Lists
        if (pool != null && this.particlePrefabs != null)
        {
            foreach (prefabAttributes prefabAtt in this.particlePrefabs)
            {
                if(!pool.ResourceCache.ContainsKey(prefabAtt.name))
                pool.ResourceCache.Add(prefabAtt.name, prefabAtt.prefabs);
            }
        }
        // ------------------------- ADDED PREFABS TO POOL END ----------------------------------------
    } // end InitPrefabPooling

    public string GetCharactersName(string nama){
        string pref = "";
        foreach(prefabAttributes go in characterPrefabs){
            if(go.name == nama){
                return go.name;
            }
        }
        return pref;
    } // end GetCharactersName

    public string GetPropsName(string nama){
        string pref = "";
        foreach(prefabAttributes go in propsPrefabs){
            if(go.name == nama){
                pref = go.name;
            }
        }
        return pref;
    } // end GetPropsName

    public string GetParticlesName(string nama){
        string pref = "";
        foreach(prefabAttributes go in particlePrefabs){
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

    public override void OnJoinedLobby()
    {
        // Close loading popup
        StartCoroutine(MenuManager.instance.CloseLoadingScreen(1f));
    } // end OnJoinedLobby

    public override void OnCreatedRoom(){
        print("Create room : " + PhotonNetwork.CurrentRoom.Name);
    }
    
    public override void OnJoinedRoom(){
        print("Successfully joined room. Waiting for others to fill");

        if(PhotonNetwork.LocalPlayer.CustomProperties["Team"] != null){
            print("Our role is " + PhotonNetwork.LocalPlayer.CustomProperties["Team"].ToString() + " Players " + (int)PhotonNetwork.CurrentRoom.PlayerCount + "/" + (int)PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message){
        print("No available room found! Creating...");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    

/*    public void FindOrCreateGameRoom(){ // custom properties to find / create new game room
        // Set room properties
        //Hashtable roomCustomProperties = new Hashtable();
        //roomCustomProperties["Map"] = "Demo"; // randomly assign any map

        // Properties to be exposed when people find room with filter otpions
        //string[] exposedPropertiesInLobby = { "Map" };

        // Room rules
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        //roomOptions.CustomRoomProperties = roomCustomProperties;
        //roomOptions.CustomRoomPropertiesForLobby = exposedPropertiesInLobby;
    }*/
    // ----------------------- CONNECTION RELATED END -------------------


    // ----------------------- FIND GAME RELATED START -------------------
    public void JoinTeam(string team){
        Hashtable playerProperties = new Hashtable();
        switch(team){
            case "Human":
                print("Join as human");
                // Popup searching modal
                MenuManager.instance.modalFindGame.SetActive(true);

                // Set player team properties
                playerProperties.Add("Team", "Human");
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

                // Join Random Room
                PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom);
            break;

            case "Ghost":
                print("Join as ghost");
                // Popup searching modal
                MenuManager.instance.modalFindGame.SetActive(true);

                // Set player team properties
                playerProperties.Add("Team", "Ghost");
                PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

                // Join Random Room
                PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom);
            break;

            default:
                print("Pardon? Unassigned value on button. Please Check function (JoinTeam)");
            break;
        }
    }
    // ----------------------- FIND GAME RELATED END -------------------
}
