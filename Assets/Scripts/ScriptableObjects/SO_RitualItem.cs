using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Ritual Item File", menuName = "Database/Ritual Items")]
public class SO_RitualItem : ScriptableObject
{
    public List<C_RitualItemDetails> ritualItemLists = new List<C_RitualItemDetails>();
    
}
