using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using Sirenix.OdinInspector;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    [Title("Win Lose Related")]
    public bool gameEnded = false;
    public GameObject uiWinLose;
    public TextMeshProUGUI winLoseText;
    
    [Title("Prefabs")]
    public Transform[] humanSpawnPoints;
    public Transform[] ghostSpawnPoints;
    public PlayerManager[] players;
    private int playersInGame;

    public GameObject playerHUD, ghostHUD;

    void Awake(){
        instance = this;
    }

    void Start(){
        players = new PlayerManager[PhotonNetwork.PlayerList.Length];
        photonView.RPC("joiningTheGame", RpcTarget.AllBuffered);

        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    [PunRPC]
    void joiningTheGame(){
        playersInGame++;

        if(playersInGame == PhotonNetwork.PlayerList.Length){
            if(PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString() == "human"){
                SpawnHuman();
            }else if(PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString() == "ghost"){
                SpawnGhost();
            }
        }
    }

    public void SpawnHuman(){
        var playerGO = PhotonNetwork.Instantiate(NetworkManager.instance.GetCharacterName("Player"), humanSpawnPoints[Random.Range(0, humanSpawnPoints.Length)].position, Quaternion.identity);
        //playerGO.GetComponent<PlayerMovement>().spawnFullbody = false;
        //playerGO.GetComponent<PlayerMovement>().spawnHandOnly = true;
        PlayerManager playerMgr = playerGO.GetComponent<PlayerManager>();
        playerMgr.isGhost = false;
        playerMgr.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        playerHUD.SetActive(true);
        //ghostHUD.SetActive(false);
    }

    public void SpawnGhost(){
        var playerGO = PhotonNetwork.Instantiate(NetworkManager.instance.GetCharacterName("Ghost_Humanoid"), ghostSpawnPoints[Random.Range(0, ghostSpawnPoints.Length)].position, Quaternion.identity);
        //playerGO.GetComponent<GhostMovement>().spawnFullbody = false;
        //playerGO.GetComponent<GhostMovement>().spawnHandOnly = true;
        PlayerManager playerMgr = playerGO.GetComponent<PlayerManager>();
        playerMgr.isGhost = true;
        playerMgr.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        ghostHUD.SetActive(true);
        //playerHUD.SetActive(false);
    }

    public PlayerManager GetPlayer(int playerId){
        return players.First(x => x.id == playerId);
    }

    public PlayerManager GetPlayer(GameObject playerObj){
        return players.First(x => x.gameObject == playerObj);
    }
}
