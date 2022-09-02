using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Footstep Data File", menuName = "Database/Footstep Data")]
public class SO_FootstepList : ScriptableObject
{
    public List<C_FootstepData> list = new List<C_FootstepData>();
}
