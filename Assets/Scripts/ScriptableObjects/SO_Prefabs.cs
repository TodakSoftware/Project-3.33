using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Prefabs File", menuName = "Database/Prefabs")]
public class SO_Prefabs : ScriptableObject
{
    [Header("UI Prefab")]
    public GameObject modalReconnectGame;
    public GameObject modalLoadingScene;
    public GameObject modalLoadingNormal;
    public GameObject modalFindGame;

    [Header("Photon Prefab Pooling")]
    public List<C_PhotonPrefabAttributes> characterPrefabs = new List<C_PhotonPrefabAttributes>();
    public List<C_PhotonPrefabAttributes> propsPrefabs = new List<C_PhotonPrefabAttributes>();
    public List<C_PhotonPrefabAttributes> particlePrefabs = new List<C_PhotonPrefabAttributes>();
}
