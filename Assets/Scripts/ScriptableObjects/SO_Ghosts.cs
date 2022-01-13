using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public struct GhostDetails{
    public string name;
    [PreviewField] public Sprite avatarIcon;
    public GameObject fullbodyPrefab;
    public GameObject handPrefab;
    public float cameraHeight;
    public float cameraDepth;
}


[CreateAssetMenu(fileName = "New Ghost", menuName = "Database/Ghost")]
public class SO_Ghosts : ScriptableObject
{
    [TableList(ShowIndexLabels = true)]public List<GhostDetails> ghostLists;
    
}
