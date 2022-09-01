using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdvancedSkin2 {

    [CreateAssetMenu( fileName = "New Wrinkle Data Asset", menuName = "PIDI Game Development Framework/Advanced Skin Shader 2/Wrinkles Data Asset" )]
    public class WrinkleDataAsset : ScriptableObject {

        public Texture2D[] normalMaps = new Texture2D[0];
        [HideInInspector]public Texture2D[] occlusionMaps = new Texture2D[0];


#if UNITY_EDITOR
        public string[] NormalMapNames { get { var l = new List<string>(); foreach ( Texture2D n in normalMaps ) l.Add( n.name ); return l.ToArray(); } }
        public string[] OcclusionMapNames { get { var l = new List<string>(); foreach ( Texture2D o in occlusionMaps ) l.Add( o.name ); return l.ToArray(); } }
#endif

    }
}