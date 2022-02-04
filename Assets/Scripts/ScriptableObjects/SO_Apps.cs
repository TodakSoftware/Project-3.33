using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public struct AppDetails{
    [TableColumnWidth(57, Resizable = false)][PreviewField(Alignment = ObjectFieldAlignment.Left)] public Sprite appIcon;
    [TableColumnWidth(50, Resizable = false)] public string code;
    public string name;
    [TextArea] public string description;
    public int price;
    [PropertyTooltip("Amount drain per second")] public float drainRate; // ex. 2, 3, 5, 2.5 light intensity @ battery life -= Time.deltaTime * (drainRate / 1000)
}


[CreateAssetMenu(fileName = "New App", menuName = "Database/Apps")]
public class SO_Apps : ScriptableObject
{
    [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]public List<AppDetails> appLists;
    
}
