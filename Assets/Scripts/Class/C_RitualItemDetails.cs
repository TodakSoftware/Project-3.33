using UnityEngine;

// RITUAL ITEMS DETAILS used in SO_RitualItem
[System.Serializable]
public class C_RitualItemDetails{
    public string name;
    public string displayName;
    [TextArea] public string descriptions;
    public Sprite image;
    public GameObject prefabs;
}