using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    bool playAsHuman;
    string selectedMapName;
    private byte maxPlayersPerRoom = 4;

    [Header("Prefabs Pooling")]
    public List<prefabAttributes> characterPrefabs;
    public List<prefabAttributes> propsPrefabs;
    public List<prefabAttributes> particlePrefabs;

    void Awake(){
        if(instance != null && instance != this){
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        PhotonNetwork.AutomaticallySyncScene = true;

        // ADDED PREFABS TO POOL START ---------------------------------------------
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
        // ADDED PREFABS TO POOL END ---------------------------------------------

    }

    void Start(){
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master server");
    }

    public void JoinAsHuman(string mapName){
        playAsHuman = true;

        Hashtable indexProps = new Hashtable() { { "team", "human" } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(indexProps);
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("Username333");

        JoinRandom(mapName);
    }

    public void JoinAsGhost(string mapName){
        playAsHuman = false;

        Hashtable indexProps = new Hashtable() { { "team", "ghost" } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(indexProps);
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("Username333");

        JoinRandom(mapName);
    }

    public void JoinRandom(string map)
    {
        selectedMapName = map;
        print("JOIN RANDOM WITH MAP: "+ map);
        Hashtable expectedCustomRoomProperties = new Hashtable()
        {
            { "Map", map }
        };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, maxPlayersPerRoom);
    }

    public override void OnCreatedRoom(){
        print("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("ON JOIN RANDON FAILED");
        Hashtable customPropreties = new Hashtable();
        customPropreties["Map"] = selectedMapName;
        string[] roomPropsInLobby = { "Map" };

        RoomOptions roomOpt = new RoomOptions();
        roomOpt.MaxPlayers = maxPlayersPerRoom; 
        roomOpt.CustomRoomProperties = customPropreties;
        roomOpt.CustomRoomPropertiesForLobby = roomPropsInLobby;

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, roomOpt);
    }

    public override void OnJoinedRoom()
    {
        print("ON JOINed ROOM : "+ PhotonNetwork.CurrentRoom.Name + " Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        int humanCount = 0;
        int ghostCount = 0;
        foreach (var player in PhotonNetwork.PlayerList){
            if(player.CustomProperties["team"].ToString() == "human"){
                humanCount += 1;
            }else{
                ghostCount += 1;
            }
        }

        // if we are human
        if(playAsHuman && humanCount > 3){
            print("Remove me as human");
            PhotonNetwork.LeaveRoom();
            // find rom
            JoinRandom(selectedMapName);
        }else{
            print("Current human : "+ humanCount);
        }

        // if we are ghost
        if(!playAsHuman && ghostCount > 1){
            print("Remove me as ghost");
            PhotonNetwork.LeaveRoom();
            // find room
            JoinRandom(selectedMapName);
        }else{
            print("Current Ghost : "+ ghostCount);
        }

        //if(humanCount == 3 && ghostCount == 1){
        if(humanCount == 1 && ghostCount == 1){
        //if(humanCount == 1){
            // Start Game
            photonView.RPC("ChangeScene", RpcTarget.All, PhotonNetwork.CurrentRoom.CustomProperties["Map"].ToString());
        }

        //Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    public void CancelOrLeaveRoom(){
        if(PhotonNetwork.InRoom){
            PhotonNetwork.LeaveRoom();
            print("leave room because cancel clicked");
            selectedMapName = "";
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
       
        base.OnPlayerLeftRoom(otherPlayer);
        print("LEAVEEE");
    }

    [PunRPC]
    public void ChangeScene(string sceneName){
        PhotonNetwork.LoadLevel(sceneName);
    }


    // ----------------------- GET PREFABS NAME -------------------
    public string GetCharacterName(string nama){
        string pref = "";
        foreach(prefabAttributes go in characterPrefabs){
            if(go.name == nama){
                return go.name;
            }
        }
        return pref;
    }

    public string GetPropName(string nama){
        string pref = "";
        foreach(prefabAttributes go in propsPrefabs){
            if(go.name == nama){
                pref = go.name;
            }
        }
        return pref;
    }

    public string GetParticlesName(string nama){
        string pref = "";
        foreach(prefabAttributes go in particlePrefabs){
            if(go.name == nama){
                pref = go.name;
            }
        }
        return pref;
    }

}
