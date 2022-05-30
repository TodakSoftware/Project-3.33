using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Ritual Item File", menuName = "Database/Ritual Items")]
public class SO_RitualItem : ScriptableObject
{
    public List<ritualItemDetails> ritualItemLists = new List<ritualItemDetails>();
    
}
