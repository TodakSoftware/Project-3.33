using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MAPS DETAILS used in SO_Maps
[System.Serializable]
public struct MapDetails{
    public string name;
    [TextArea] public string description;
    [Tooltip("Must correctly matched with scene file name")]
    public string sceneName; 
    [Tooltip("Must correctly matched with build index settings")]
    public int buildIndex;
}
/* --------------------------------------------------------------------------------- */

// PHOTON ATTRIBUTES PREFABS used in SO_Prefabs
[System.Serializable]
public class photonPrefabAttributes{
    public string name;
    public GameObject prefabs;
}
/* --------------------------------------------------------------------------------- */

// GAME MODE used in SO_GameSettings
[System.Serializable]
public class gameMode{ // gameModeIndex : 0 = Normal Hunt 3 v 1
    public string name; // for gamemode ref
    public int gameModeIndex;
    public string displayName; // same as name but maybe specific
    [TextArea] public string descriptions;
    
    [Header("Find Game Settings")]
    public int maxHumanPerGame; // Normal mode 3
    public int maxGhostPerGame; // Normal mode 1
    public float findGameTimeoutDuration; // <- 213 = 3:33

    [Header("Ingame Settings")]
    public int clockStartTime; // Clock time when the game START in integer. START : 3.26 = 206 (Hunt Mode)
    public int clockEndTime; // Clock time when the game END in integer. END : 3.33 = 213 (Hunt Mode)
    public int totalRitualItems; // Total ritual items suppose to spawn in the game
}
/* --------------------------------------------------------------------------------- */

// RITUAL ITEMS DETAILS used in SO_RitualItem
[System.Serializable]
public class ritualItemDetails{
    public string name;
    public string displayName;
    [TextArea] public string descriptions;
    public Sprite image;
    public GameObject prefabs;
}
/* --------------------------------------------------------------------------------- */

public class GlobalStructs : MonoBehaviour
{
    
}
