using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public struct GhostDetails{
    [TableColumnWidth(70, Resizable = false)][PreviewField(Alignment = ObjectFieldAlignment.Left)] public Sprite avatarIcon;
    [TableColumnWidth(50, Resizable = false)] public string code;
    public string name;
    public GameObject prefab;
    public float cameraHeight;
    public float cameraDepth;
}


[CreateAssetMenu(fileName = "New Ghost", menuName = "Database/Ghost")]
public class SO_Ghosts : ScriptableObject
{
    [TableList(AlwaysExpanded = true, ShowIndexLabels = true)]public List<GhostDetails> ghostLists;
    
}
