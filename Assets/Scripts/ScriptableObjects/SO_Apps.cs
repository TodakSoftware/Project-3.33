using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New App", menuName = "Database/Apps")]
public class SO_Apps : ScriptableObject
{
    public List<S_AppDetails> appLists;
}
