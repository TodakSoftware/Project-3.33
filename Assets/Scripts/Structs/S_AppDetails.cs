using UnityEngine;

[System.Serializable]
public struct S_AppDetails{
    public string name;
    public string code;
    public Sprite appIcon;
    [TextArea] public string description;
    public int price;
    public bool isSystemApp; // build in app (non-purchased)
    [Tooltip("Amount drain per second")] public float drainRate; // ex. 2, 3, 5, 2.5 light intensity @ battery life -= Time.deltaTime * (drainRate / 1000)
}