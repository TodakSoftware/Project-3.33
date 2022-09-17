using System.Collections.Generic;
using UnityEngine;

// GAME MODE used in SO_GameSettings
[System.Serializable]
public class C_GameMode{ // gameModeIndex : 0 = Normal Hunt 3 v 1
    public string name; // for gamemode ref
    public int gameModeIndex;
    public string displayName; // same as name but maybe specific
    [TextArea] public string descriptions;
    
    [Header("Find Game Settings")]
    public int maxHumanPerGame; // Normal mode 3
    public int maxGhostPerGame; // Normal mode 1
    public float findGameTimeoutDuration; // <- 213 = 3:33

    [Header("Ingame Settings")]
    public int minuteStartTime; // Clock time when the game START in integer. START : 3.[26] = Minutes Only (Hunt Mode)
    public int minuteEndTime; // Clock time when the game END in integer. END : 3.[33] = Minutes Only (Hunt Mode)
    public int totalRitualItems; // Total ritual items suppose to spawn in the game
}