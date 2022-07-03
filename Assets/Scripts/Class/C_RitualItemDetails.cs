using UnityEngine;

// RITUAL ITEMS DETAILS used in SO_RitualItem
[System.Serializable]
public class C_RitualItemDetails{
    public string name;
    public string code;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject itemPrefab; // The one that spawn somewhere
    public GameObject itemMesh; // The one that doesnt include collider

    [Header("Settings")]
    public E_InteractType interactType;
    public E_ButtonType buttonType;
    public float holdDuration; // only valid if buttonType == HOLD
}