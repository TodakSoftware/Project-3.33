using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Map File", menuName = "Database/Maps")]
public class SO_Maps : ScriptableObject
{
    public List<MapDetails> mapsList = new List<MapDetails>();
}
