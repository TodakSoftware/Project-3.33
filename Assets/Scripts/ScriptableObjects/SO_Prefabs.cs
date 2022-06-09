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
    public List<photonPrefabAttributes> characterPrefabs = new List<photonPrefabAttributes>();
    public List<photonPrefabAttributes> propsPrefabs = new List<photonPrefabAttributes>();
    public List<photonPrefabAttributes> particlePrefabs = new List<photonPrefabAttributes>();
}
