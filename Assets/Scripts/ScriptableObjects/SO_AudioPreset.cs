using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "New Audio Preset", menuName = "Database/Audio Preset")]
public class SO_AudioPreset : ScriptableObject
{
    public C_Sound[] sounds;      // store all our sounds
}
