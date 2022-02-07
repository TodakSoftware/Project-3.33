using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public struct RitualItemDetails{
    [TableColumnWidth(57, Resizable = false)][PreviewField(Alignment = ObjectFieldAlignment.Left)] public Sprite itemIcon;
    [TableColumnWidth(50, Resizable = false)] public string code;
    public string name;
    [TextArea] public string description;
}


[CreateAssetMenu(fileName = "New Item", menuName = "Database/Ritual Item")]
public class SO_Ritual_Item : ScriptableObject
{
    [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]public List<RitualItemDetails> itemLists;
    
}
