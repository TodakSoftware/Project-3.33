using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public struct CharacterDetails{
    [TableColumnWidth(70, Resizable = false)][PreviewField(Alignment = ObjectFieldAlignment.Left)] public Sprite avatarIcon;
    [TableColumnWidth(50, Resizable = false)] public string code;
    public string name;
    public GameObject fullbodyPrefab;
    public GameObject handPrefab;
    public float cameraHeight;
    public float cameraDepth;
}


[CreateAssetMenu(fileName = "New Characters", menuName = "Database/Characters")]
public class SO_Characters : ScriptableObject
{
    [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]public List<CharacterDetails> characterLists;
    
}
