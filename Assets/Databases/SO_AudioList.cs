using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Audio Data File", menuName = "Database/Audio Data")]
public class SO_AudioList : ScriptableObject
{
    public List<C_AudioData> list = new List<C_AudioData>();
}
