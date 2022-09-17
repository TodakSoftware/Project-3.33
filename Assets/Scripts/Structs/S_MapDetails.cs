using System.Collections.Generic;
using UnityEngine;

// MAPS DETAILS used in SO_Maps
[System.Serializable]
public struct S_MapDetails{
    [Tooltip("Must correctly matched with scene file name")]
    public string name;
    public string displayName;
    [TextArea] public string description;
}