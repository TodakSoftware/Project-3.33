using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Serializable]
public struct CharacterDetails{
    public string name;
    [PreviewField] public Sprite avatarIcon;
    public GameObject fullbodyPrefab;
    public GameObject handPrefab;
    public float cameraHeight;
    public float cameraDepth;
}


[CreateAssetMenu(fileName = "New Characters", menuName = "Database/Characters")]
public class SO_Characters : ScriptableObject
{
    [TableList(ShowIndexLabels = true)]public List<CharacterDetails> characterLists;
    
}
