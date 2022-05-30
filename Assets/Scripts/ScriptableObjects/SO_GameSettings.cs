using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Setting File", menuName = "Database/Game Settings")]
public class SO_GameSettings : ScriptableObject
{
    public List<gameMode> gameMode = new List<gameMode>();
}
