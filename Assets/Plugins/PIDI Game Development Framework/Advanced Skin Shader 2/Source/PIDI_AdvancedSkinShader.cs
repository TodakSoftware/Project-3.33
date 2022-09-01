#define SKIN2_PRO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedSkin2 {


#if SKIN2_PRO
    [System.Serializable]
    public struct BoneTransformData {
        public Transform bone;
        public Vector3 position;
        public Quaternion rotation;

        public BoneTransformData( Transform setBone, Vector3 setPos, Quaternion setRot ) {
            bone = setBone;
            position = setPos;
            rotation = setRot;
        }

    }


    [System.Serializable]
    public struct WrinklesSet {

        public Texture2D regionsMapOverride;

        public Transform[] trackedBones;

        public int[] blendshapeIDs;

        public float[] blendshapesSnapshot;

        public BoneTransformData[] bonesSnapshot;

        public BoneTransformData[] bonesRestPose;

        public int normalMapID;

        public int occlusionMapID;

        public float[] regionsBlend;

        private float wrinkleBlend;

        public float Blend { get { return wrinkleBlend; } }

        public float[] distancesToRest;
        public float[] anglesToRest;

        public Vector3[] directionsToRest;

        public void UpdateBlend( Transform headBone, SkinnedMeshRenderer renderer ) {

            wrinkleBlend = 0.0f;

            if ( bonesRestPose.Length != trackedBones.Length || bonesSnapshot.Length != trackedBones.Length ) {
                return;
            }

            if ( directionsToRest == null || directionsToRest.Length != trackedBones.Length ) {
                directionsToRest = new Vector3[trackedBones.Length];

                for ( int i = 0; i < trackedBones.Length; i++ ) {
                    directionsToRest[i] = (bonesRestPose[i].position - bonesSnapshot[i].position).normalized;
                }
            }

            if ( distancesToRest == null || distancesToRest.Length != trackedBones.Length ) {
                distancesToRest = new float[trackedBones.Length];
                anglesToRest = new float[trackedBones.Length];

                for ( int i = 0; i < bonesRestPose.Length; i++ ) {
                    distancesToRest[i] = (bonesRestPose[i].position - bonesSnapshot[i].position).sqrMagnitude;
                    anglesToRest[i] = Quaternion.Angle( bonesSnapshot[i].rotation, bonesRestPose[i].rotation );
                }

            }

            for ( int i = 0; i < trackedBones.Length; i++ ) {
                if ( trackedBones[i] != null ) {
                    var d = (Mathf.Clamp01( 1 - (headBone.InverseTransformPoint( trackedBones[i].position ) - bonesSnapshot[i].position).sqrMagnitude / (distancesToRest[i] <= 0 ? 1 : distancesToRest[i]) ) * Mathf.Clamp01( 1 - Quaternion.Angle( trackedBones[i].localRotation, bonesSnapshot[i].rotation ) / (Mathf.Abs( anglesToRest[i] ) < 0.05f ? 1 : anglesToRest[i]) ));
                    d *= Mathf.Clamp01( Vector3.Dot( (bonesRestPose[i].position - headBone.InverseTransformPoint( trackedBones[i].position )).normalized, directionsToRest[i] ) );
                    wrinkleBlend += d;
                }
            }

            wrinkleBlend = Mathf.Clamp01( wrinkleBlend / Mathf.Max( trackedBones.Length, 1 ) );

            float blendshapeWeights = 0.0f;

            if ( blendshapesSnapshot.Length != blendshapeIDs.Length ) {
                blendshapesSnapshot = new float[blendshapeIDs.Length];
            }

            for ( int i = 0; i < blendshapeIDs.Length; i++ ) {
                blendshapeWeights += Mathf.Clamp01( renderer.GetBlendShapeWeight( blendshapeIDs[i] ) / blendshapesSnapshot[i] );
            }

            wrinkleBlend = Mathf.Clamp01( wrinkleBlend + (blendshapeWeights / Mathf.Max( blendshapeIDs.Length, 1 )) );
        }

        public WrinklesSet( int normalID ) {
            normalMapID = normalID;
            occlusionMapID = 0;
            regionsBlend = new float[20];
            bonesSnapshot = new BoneTransformData[0];
            trackedBones = new Transform[0];
            blendshapeIDs = new int[0];
            blendshapesSnapshot = new float[0];
            wrinkleBlend = 0.0f;
            bonesRestPose = new BoneTransformData[0];
            distancesToRest = new float[0];
            anglesToRest = new float[0];
            directionsToRest = new Vector3[0];
            regionsMapOverride = null;
        }

    }

    public enum WrinkleSetupMode { None, TensionMap, ExplicitRegions }
#endif

    [System.Serializable]
    public struct DecalData {
        public int decalBlendMode;

        public int decalUVSet;

        public Vector4 decalCoords;

        public Texture2D decalTex;

        public Color decalColor;
        public Color decalSpecCol;

        public Texture2D decalSpecMap;
    }


    [System.Serializable]
    public class SkinProfile {

        public bool isSkinMaterial;


#if SKIN2_MOBILE || SKIN2_PRO
        public bool mobileMode;
#endif

        public bool enableDecals = false;

#if SKIN2_PRO        

        public bool enableWrinkles = false;

        public WrinkleSetupMode wrinklesMode;

        public Texture2D regionsMap;

        public RenderTexture wrinklesNormalMap;

        public WrinkleDataAsset wrinkleDataAsset;

        public WrinklesSet[] wrinkleDataSets = new WrinklesSet[0];

#endif

        public bool enableTranslucency = true;

        public Color skinColor = Color.white;
        public Color specColor = new Color( 0.15f, 0.15f, 0.15f, 1.0f );
        public Color sssColor = Color.red;

#if SKIN2_PRO
        public float tessellationLevel = 16.0f;
        public float phongLevel = 1.0f;
#endif

        public float glossinessLevel = 0.4f;
        public float occlusionStrength = 1.0f;
        public float translucencyStrength = 1.0f;
        public float skinSurfaceWarp = 0.25f;


        public float microSkinUVScale = 24;
        public float debugTensionMap;
        public float minVertexDistance;

        public Texture2D colorMap;
        public Texture2D specGlossMap;
        public Texture2D translucencyMap;
        public Texture2D normalMap;
        public Texture2D occlusionMap;
        public Texture2D microSkinMap;
        public Texture2D skinDataMap;
        public Texture2D wrinklesMap;

        [SerializeField] public List<DecalData> skinDecals = new List<DecalData>();

        public int decal0BlendMode;
        public int decal1BlendMode;

        public int decal0UVSet;
        public int decal1UVSet;

        public Vector4 decal0Coords = new Vector4( 1, 1, 0, 0 );
        public Vector4 decal1Coords = new Vector4( 1, 1, 0, 0 );

        public Texture2D decal0Tex;
        public Texture2D decal1Tex;

        public Color decal0Color = Color.white;
        public Color decal1Color = Color.white;
        public Color decal0SpecCol = new Color( 0.15f, 0.15f, 0.15f, 0.5f );
        public Color decal1SpecCol = new Color( 0.15f, 0.15f, 0.15f, 0.5f );

        public Texture2D decal0SpecMap;
        public Texture2D decal1SpecMap;



    }


#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    [RequireComponent( typeof( SkinnedMeshRenderer ) )]
    public class PIDI_AdvancedSkinShader : MonoBehaviour {



#if SKIN2_PRO
        public List<BoneTransformData> restPoseBones = new List<BoneTransformData>();
#endif


#if UNITY_EDITOR
        public bool[] folds = new bool[32];
#endif

        private string version = "2.4";

        public string Version { get { return version; } }

        private Renderer rend;
        private Material[] sharedMats = new Material[1];

        [SerializeField] protected Texture2D defaultBump;

#if SKIN2_PRO
        public Mesh originalMesh;
        public Mesh patchedMesh;

        public int patchState;

        public bool deferredMode;
        public bool cartoonMode;

        [SerializeField] protected Material wrinklesMaterial;
#endif

#if SKIN2_LWRP || SKIN2_PRO
        public bool srpMode;
#endif


#if SKIN2_PRO
        public Light[] deferredLights = new Light[4];
#endif

        public SkinProfile[] skinProfiles = new SkinProfile[1];

#if UNITY_EDITOR
        public int currentSkinProfile;
#endif

        public bool updateDynamically;
        public float updateTime = 0.015f;

        public Transform headBone;
        public Quaternion hRotation;

        private float timer = 0.015f;



        public void InitialSetup() {
            var slots = new string[GetComponent<Renderer>().sharedMaterials.Length];

            List<SkinProfile> skins = new List<SkinProfile>();

            for ( int i = 0; i < slots.Length; i++ ) {
                slots[i] = "MATERIAL SLOT " + i;
                if ( skinProfiles.Length > i ) {
                    skins.Add( skinProfiles[i] == null ? new SkinProfile() : skinProfiles[i] );
                }
                else {
                    skins.Add( new SkinProfile() );
                }

                var shaderName = GetComponent<SkinnedMeshRenderer>().sharedMaterials[i].shader.name;

                skins[i].isSkinMaterial = shaderName.Contains( "Advanced Skin Shader" );

                if ( skins[i].isSkinMaterial ) {
                    skins[i].mobileMode = shaderName.Contains( "Mobile" );

                    if ( shaderName.Contains( "Tension" ) ) {
                        skins[i].wrinklesMode = WrinkleSetupMode.TensionMap;
                    }
                    else if ( shaderName.Contains( "Region" ) ) {
                        skins[i].wrinklesMode = WrinkleSetupMode.ExplicitRegions;
                    }
                    else {
                        skins[i].wrinklesMode = WrinkleSetupMode.None;
                    }

                }

            }

            skinProfiles = skins.ToArray();
        }


        public void Start() {
            if ( !originalMesh ) {
                originalMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            if ( !patchedMesh || !GetComponent<SkinnedMeshRenderer>().sharedMesh ) {
                GetComponent<SkinnedMeshRenderer>().sharedMesh = originalMesh;
            }

            InitialSetup();

            UpdateSkinMaterials();
        }

        public void OnEnable() {

            if ( !originalMesh ) {
                originalMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            if ( !patchedMesh || !GetComponent<SkinnedMeshRenderer>().sharedMesh ) {
                GetComponent<SkinnedMeshRenderer>().sharedMesh = originalMesh;
            }

            InitialSetup();

            UpdateSkinMaterials();
        }

        public void OnDisable() {

            GetComponent<SkinnedMeshRenderer>().SetPropertyBlock( null );

#if UNITY_2018_1_OR_NEWER
            for ( int i = 0; i < GetComponent<SkinnedMeshRenderer>().sharedMaterials.Length; i++ ) {
                GetComponent<SkinnedMeshRenderer>().SetPropertyBlock( null, i );
            }
#endif
            if ( !GetComponent<SkinnedMeshRenderer>().sharedMesh ) {
                GetComponent<SkinnedMeshRenderer>().sharedMesh = originalMesh;
            }
        }


        public void Update() {

#if UNITY_EDITOR

            if ( !Application.isPlaying ) {
                UpdateSkinMaterials();
            }

#endif
            if ( Application.isPlaying ) {
                bool dynamicWrinkles = false;

                for ( int i = 0; i < skinProfiles.Length; i++ ) {
                    if ( skinProfiles[i].isSkinMaterial && skinProfiles[i].wrinklesMode == WrinkleSetupMode.ExplicitRegions ) {
                        dynamicWrinkles = true;
                    }
                }

                if ( (dynamicWrinkles || updateDynamically) && Time.timeSinceLevelLoad > timer ) {
                    UpdateSkinMaterials();
                    timer = Time.timeSinceLevelLoad + (Application.platform == RuntimePlatform.Android ? (updateTime * 8.0f) : (updateTime));
                }
            }
        }


        public void UpdateWrinklesBlend( int skinProfileIndex ) {

            for ( int i = 0; i < skinProfiles[skinProfileIndex].wrinkleDataSets.Length; i++ ) {
                skinProfiles[skinProfileIndex].wrinkleDataSets[i].UpdateBlend( headBone, GetComponent<SkinnedMeshRenderer>() );
            }

        }


        public void UpdateSkinMaterials() {

            if ( rend ) {
                rend.SetPropertyBlock( null );
            }
            else {
                rend = GetComponent<SkinnedMeshRenderer>();
                return;
            }

#if UNITY_2018_1_OR_NEWER
            for ( int i = 0; i < rend.sharedMaterials.Length; i++ ) {
                rend.SetPropertyBlock( null, i );
            }
#endif

#if SKIN2_PRO
            if ( headBone && restPoseBones.Count != GetComponent<SkinnedMeshRenderer>().bones.Length ) {
                foreach ( Transform bone in GetComponent<SkinnedMeshRenderer>().bones ) {
                    restPoseBones.Add( new BoneTransformData( bone, headBone.InverseTransformPoint( bone.position ), bone.localRotation ) );
                }
            }
#endif
            Vector4[] lightsPosDir = new Vector4[4], lightsColor = new Vector4[4], lightsData = new Vector4[4];

            if ( deferredMode ) {
                for ( int i = 0; i < 4; i++ ) {
                    if ( deferredLights[i] ) {
                        lightsPosDir[i] = deferredLights[i].type == LightType.Directional ? -deferredLights[i].transform.forward : deferredLights[i].transform.position;
                        lightsPosDir[i].w = deferredLights[i].type == LightType.Directional ? 0 : 1;
                        lightsColor[i] = deferredLights[i].color;
                        lightsColor[i].w = deferredLights[i].intensity;
                        lightsData[i].x = deferredLights[i].range;
                        lightsData[i].y = deferredLights[i].shadows == LightShadows.None ? 0 : 1;

                        if ( deferredLights[i].type == LightType.Spot ) {
                            var lDot = Vector3.Dot( (deferredLights[i].transform.position - transform.position).normalized, deferredLights[i].transform.forward );
                            var spotAtten = lDot > (1 - (deferredLights[i].spotAngle / 90.0f)) ? 1 : 0;

                            lightsColor[i].x *= spotAtten;
                            lightsColor[i].y *= spotAtten;
                            lightsColor[i].z *= spotAtten;
                        }
                    }
                }
            }

            for ( int i = 0; i < skinProfiles.Length; i++ ) {
                if ( skinProfiles[i].isSkinMaterial ) {

                    if ( skinProfiles[i].wrinklesMode == WrinkleSetupMode.ExplicitRegions )
                        UpdateWrinklesBlend( i );

                    var mat = new MaterialPropertyBlock();
                    GetPropertyBlock( ref mat, i );

                    mat.SetColor( "_PSkinColor", skinProfiles[i].skinColor );

                    mat.SetTexture( "_PSkinMainTex", skinProfiles[i].colorMap ? skinProfiles[i].colorMap : Texture2D.whiteTexture );
                    mat.SetTexture( "_PSkinBaseNormals", skinProfiles[i].normalMap ? skinProfiles[i].normalMap : defaultBump ? defaultBump : Texture2D.blackTexture );

                    mat.SetTexture( "_PSkinOcclusionMap", skinProfiles[i].occlusionMap ? skinProfiles[i].occlusionMap : Texture2D.whiteTexture );
                    mat.SetFloat( "_PSkinOcclusionLevel", skinProfiles[i].occlusionStrength );

                    mat.SetTexture( "_PSkinSpecMap", skinProfiles[i].specGlossMap ? skinProfiles[i].specGlossMap : Texture2D.whiteTexture );
                    mat.SetColor( "_PSkinSpecColor", skinProfiles[i].specColor );
                    mat.SetFloat( "_PSkinGlossiness", skinProfiles[i].glossinessLevel );

                    mat.SetColor( "_PSkinSSSColor", skinProfiles[i].sssColor );

                    mat.SetTexture( "_PSkinMicroSkin", skinProfiles[i].microSkinMap ? skinProfiles[i].microSkinMap : Texture2D.whiteTexture );
                    mat.SetFloat( "_PSkinMicroSkinTiling", skinProfiles[i].microSkinUVScale );


                    mat.SetTexture( "_PSkinTranslucencyMap", skinProfiles[i].translucencyMap ? skinProfiles[i].translucencyMap : Texture2D.blackTexture );
                    mat.SetFloat( "_PSkinTranslucencyLevel", skinProfiles[i].translucencyStrength );

                    if ( headBone ) {

                        var rotEuler = (Quaternion.Inverse( headBone.rotation ) * hRotation);

                        mat.SetVector( "_PSkinHeadBonePos", headBone.position );
                        mat.SetVector( "_PSkinHeadBoneRotC", new Vector4( rotEuler.w, rotEuler.x, rotEuler.y, rotEuler.z ) );

                    }

                    if ( sharedMats.Length > i && sharedMats[i] != null ) {

                        var shName = sharedMats[i].shader.name;
                        if ( skinProfiles[i].enableWrinkles ) {
                            if ( shName.Contains( "Advanced Skin Shader 2" ) && shName.Contains( "Tension" ) ) {
                                mat.SetTexture( "_PSkinWrinklesMap", skinProfiles[i].wrinklesMap ? skinProfiles[i].wrinklesMap : defaultBump ? defaultBump : Texture2D.blackTexture );

                                mat.SetFloat( "_PSkinDebugTensionMap", skinProfiles[i].debugTensionMap );
                                mat.SetFloat( "_PSkinVertexMinDistance", skinProfiles[i].minVertexDistance );

                            }
                            else if ( shName.Contains( "Advanced Skin Shader 2" ) && shName.Contains( "Region" ) ) {

#if UNITY_EDITOR
                                if ( !wrinklesMaterial ) {
                                    wrinklesMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>( UnityEditor.AssetDatabase.GUIDToAssetPath( UnityEditor.AssetDatabase.FindAssets( "l:PSkin2_RegionsShader t:Material" )[0] ) );
                                }
#endif

                                if ( wrinklesMaterial && skinProfiles[i].wrinkleDataAsset && skinProfiles[i].regionsMap ) {

                                    if ( !skinProfiles[i].wrinklesNormalMap || skinProfiles[i].wrinklesNormalMap.width != skinProfiles[i].normalMap.width ) {
                                        if ( skinProfiles[i].wrinklesNormalMap ) {
                                            DestroyImmediate( skinProfiles[i].wrinklesNormalMap );
                                        }
                                        skinProfiles[i].wrinklesNormalMap = new RenderTexture( skinProfiles[i].normalMap.width, skinProfiles[i].normalMap.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
                                    }

                                    var tempRender = RenderTexture.GetTemporary( skinProfiles[i].normalMap.width, skinProfiles[i].normalMap.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );

                                    Graphics.Blit( skinProfiles[i].normalMap, tempRender );

                                    for ( int w = 0; w < skinProfiles[i].wrinkleDataSets.Length; w++ ) {
#if UNITY_EDITOR
                                        if ( !Application.isPlaying )
                                            skinProfiles[i].wrinkleDataSets[w].UpdateBlend( headBone, GetComponent<SkinnedMeshRenderer>() );
#endif
                                        wrinklesMaterial.SetTexture( "_RegionsMap", skinProfiles[i].wrinkleDataSets[w].regionsMapOverride ? skinProfiles[i].wrinkleDataSets[w].regionsMapOverride : skinProfiles[i].regionsMap );
                                        wrinklesMaterial.SetTexture( "_MixToMap", skinProfiles[i].wrinkleDataAsset.normalMaps[skinProfiles[i].wrinkleDataSets[w].normalMapID] );
                                        wrinklesMaterial.SetVector( "_RegionsBlendA", new Vector4( skinProfiles[i].wrinkleDataSets[w].regionsBlend[0], skinProfiles[i].wrinkleDataSets[w].regionsBlend[1], skinProfiles[i].wrinkleDataSets[w].regionsBlend[2], skinProfiles[i].wrinkleDataSets[w].regionsBlend[3] ) * skinProfiles[i].wrinkleDataSets[w].Blend );
                                        Graphics.Blit( tempRender, skinProfiles[i].wrinklesNormalMap, wrinklesMaterial );
                                        Graphics.Blit( skinProfiles[i].wrinklesNormalMap, tempRender );
                                    }
                                    RenderTexture.active = null;
                                    RenderTexture.ReleaseTemporary( tempRender );

                                    mat.SetTexture( "_PSkinBaseNormals", skinProfiles[i].wrinklesNormalMap );

                                }

                            }
                        }
                    }

                    mat.SetTexture( "_PSkinDecal0Tex", skinProfiles[i].enableDecals ? skinProfiles[i].decal0Tex ? skinProfiles[i].decal0Tex : Texture2D.blackTexture : Texture2D.blackTexture );
                    mat.SetTexture( "_PSkinDecal1Tex", skinProfiles[i].enableDecals ? skinProfiles[i].decal1Tex ? skinProfiles[i].decal1Tex : Texture2D.blackTexture : Texture2D.blackTexture );

                    mat.SetTexture( "_PSkinDecal0SpecMap", skinProfiles[i].enableDecals ? skinProfiles[i].decal0SpecMap ? skinProfiles[i].decal0SpecMap : Texture2D.whiteTexture : Texture2D.whiteTexture );
                    mat.SetTexture( "_PSkinDecal1SpecMap", skinProfiles[i].enableDecals ? skinProfiles[i].decal1SpecMap ? skinProfiles[i].decal1SpecMap : Texture2D.whiteTexture : Texture2D.whiteTexture );

                    mat.SetColor( "_PSkinDecal0Color", skinProfiles[i].decal0Color );
                    mat.SetColor( "_PSkinDecal0SpecColor", skinProfiles[i].decal0SpecCol );

                    mat.SetColor( "_PSkinDecal1Color", skinProfiles[i].decal1Color );
                    mat.SetColor( "_PSkinDecal1SpecColor", skinProfiles[i].decal1SpecCol );

                    mat.SetFloat( "_PSkinDecal0BlendMode", skinProfiles[i].decal0BlendMode );
                    mat.SetFloat( "_PSkinDecal1BlendMode", skinProfiles[i].decal1BlendMode );

                    mat.SetFloat( "_PSkinDecal0UV", skinProfiles[i].decal0UVSet );
                    mat.SetFloat( "_PSkinDecal1UV", skinProfiles[i].decal1UVSet );

                    mat.SetVector( "_PSkinDecal0UVTransform", skinProfiles[i].decal0Coords );
                    mat.SetVector( "_PSkinDecal1UVTransform", skinProfiles[i].decal1Coords );

                    if ( deferredMode ) {
                        mat.SetVector( "_PSkinWorldPos", transform.position );

                        if ( !srpMode && Camera.current )
                            mat.SetVector( "_PSkinViewDir", Camera.current.transform.forward );

                        mat.SetVectorArray( "_PSkinDefLightsPosDir", lightsPosDir );
                        mat.SetVectorArray( "_PSkinDefLightsColor", lightsColor );
                        mat.SetVectorArray( "_PSkinDefLightsData", lightsData );

                    }

                    if ( i < sharedMats.Length )
                        SetPropertyBlock( mat, i );
                }
                else {
                    var mat = new MaterialPropertyBlock();
                    mat.Clear();
                    if ( i < sharedMats.Length )
                        SetPropertyBlock( mat, i );
                }
            }

        }



        public void SavePoseSnapshot( int skinProfileIndex, int wrinkleDataIndex ) {
            if ( skinProfileIndex < skinProfiles.Length && wrinkleDataIndex < skinProfiles[skinProfileIndex].wrinkleDataSets.Length ) {

                if ( skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].blendshapeIDs != null ) {
                    skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].blendshapesSnapshot = new float[skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].blendshapeIDs.Length];

                    for ( int i = 0; i < skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].blendshapeIDs.Length; i++ ) {
                        skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].blendshapesSnapshot[i] = GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight( skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].blendshapeIDs[i] );
                    }
                }

                if ( skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones != null ) {

                    Debug.Log( "Saving Snapshot" );
                    skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].bonesSnapshot = new BoneTransformData[skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones.Length];
                    skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].bonesRestPose = new BoneTransformData[skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones.Length];
                    skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].distancesToRest = new float[0];
                    skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].anglesToRest = new float[0];
                    skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].directionsToRest = new Vector3[0];

                    for ( int i = 0; i < skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones.Length; i++ ) {
                        skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].bonesSnapshot[i].position = headBone.InverseTransformPoint( skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones[i].position );
                        skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].bonesSnapshot[i].rotation = skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones[i].localRotation;
                        for ( int b = 0; b < restPoseBones.Count; b++ ) {
                            if ( restPoseBones[b].bone == skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].trackedBones[i] ) {
                                skinProfiles[skinProfileIndex].wrinkleDataSets[wrinkleDataIndex].bonesRestPose[i] = restPoseBones[b];
                            }
                        }
                    }

                }




            }

        }


        public void GeneratePoseData( int skinProfileIndex, int wrinklePoseIndex ) {

            var buffPos = transform.root.position;
            var buffRot = transform.root.rotation;

            transform.root.position = Vector3.zero;
            transform.root.rotation = Quaternion.identity;

            if ( !patchedMesh ) {
                if ( GetComponent<MeshRenderer>() ) {

                    if ( !patchedMesh ) {
                        patchedMesh = GetComponent<MeshFilter>().mesh;
                        patchedMesh.name += "_PIDI_PATCHED";
                    }

                    if ( !patchedMesh ) {
                        return;
                    }
                }
                else if ( GetComponent<SkinnedMeshRenderer>() ) {
                    if ( !patchedMesh ) {
                        patchedMesh = Instantiate( GetComponent<SkinnedMeshRenderer>().sharedMesh );
                        patchedMesh.name = GetComponent<SkinnedMeshRenderer>().sharedMesh.name;
                        patchedMesh.name += "_PIDI_PATCHED";
                    }

                    if ( !patchedMesh ) {
                        return;
                    }
                }
            }

            Mesh mesh = new Mesh();

            if ( GetComponent<SkinnedMeshRenderer>() ) {

                if ( skinProfiles[skinProfileIndex].isSkinMaterial ) {


                    GetComponent<SkinnedMeshRenderer>().BakeMesh( mesh );

                    var vertices = mesh.vertices;

                    var uv1 = new List<Vector4>();
                    var uv2 = new List<Vector4>();
                    var uv3 = new List<Vector4>();

                    mesh.GetUVs( 1, uv1 );
                    mesh.GetUVs( 2, uv2 );
                    mesh.GetUVs( 3, uv3 );

                    if ( uv1.Count != vertices.Length ) {
                        uv1 = new List<Vector4>( new Vector4[vertices.Length] );

                        for ( int i = 0; i < uv1.Count; i++ ) {
                            uv1[i] = new Vector4( mesh.uv[i].x, mesh.uv[i].y, 0, 0 );
                        }
                    }

                    if ( uv2.Count != vertices.Length ) {
                        uv2 = new List<Vector4>( new Vector4[vertices.Length] );
                    }

                    if ( uv3.Count != vertices.Length ) {
                        uv3 = new List<Vector4>( new Vector4[vertices.Length] );
                    }

                    for ( int i = 0; i < vertices.Length; i++ ) {

                        vertices[i] = transform.TransformPoint( vertices[i] );

                        vertices[i] = vertices[i] - headBone.position;

                        switch ( wrinklePoseIndex ) {
                            case 0:
                                uv1[i] = new Vector4( uv1[i].x, uv1[i].y, vertices[i].x, vertices[i].y );
                                uv2[i] = new Vector4( vertices[i].z, vertices[i].x, vertices[i].y, vertices[i].z );
                                uv3[i] = new Vector4( vertices[i].x, vertices[i].y, vertices[i].z, 1 );
                                break;

                            case 1:
                                uv2[i] = new Vector4( uv2[i].x, vertices[i].x, vertices[i].y, vertices[i].z );
                                uv3[i] = new Vector4( vertices[i].x, vertices[i].y, vertices[i].z, 1 );
                                break;

                            case 2:
                                uv3[i] = new Vector4( vertices[i].x, vertices[i].y, vertices[i].z, 1 );
                                break;
                        }
                    }



                    patchedMesh.SetUVs( 1, uv1 );
                    patchedMesh.SetUVs( 2, uv2 );
                    patchedMesh.SetUVs( 3, uv3 );

                    GetComponent<SkinnedMeshRenderer>().sharedMesh = patchedMesh;

                    DestroyImmediate( mesh );

                }

            }

            transform.root.position = buffPos;
            transform.root.rotation = buffRot;

            patchState++;

        }


        public void UpdateMeshData() {

            if ( GetComponent<MeshRenderer>() ) {

                if ( !patchedMesh ) {
                    patchedMesh = GetComponent<MeshFilter>().mesh;
                    patchedMesh.name += "_PIDI_PATCHED";
                }

                if ( !patchedMesh ) {
                    return;
                }

                var uv = new List<Vector4>();

                patchedMesh.GetUVs( 0, uv );

                var uv2 = uv.ToArray();

                var sideSize = Mathf.ClosestPowerOfTwo( (int)Mathf.Sqrt( patchedMesh.vertexCount ) );

                for ( int i = 0; i < patchedMesh.vertexCount; i++ ) {
                    uv2[i].w = Mathf.FloorToInt( i / sideSize );
                    uv2[i].z = i - (sideSize * uv2[i].w);
                }

                patchedMesh.SetUVs( 0, new List<Vector4>( uv2 ) );


                GetComponent<MeshFilter>().sharedMesh = patchedMesh;


            }
            else if ( GetComponent<SkinnedMeshRenderer>() ) {

                if ( !patchedMesh ) {
                    patchedMesh = Instantiate( GetComponent<SkinnedMeshRenderer>().sharedMesh );
                    patchedMesh.name += "_PIDI_PATCHED";
                }

                if ( !patchedMesh ) {
                    return;
                }

                var uv = new List<Vector4>();

                patchedMesh.GetUVs( 0, uv );

                var uv2 = uv.ToArray();

                var sideSize = Mathf.ClosestPowerOfTwo( (int)Mathf.Sqrt( patchedMesh.vertexCount ) );

                for ( int i = 0; i < patchedMesh.vertexCount; i++ ) {
                    uv2[i].w = Mathf.FloorToInt( i / sideSize );
                    uv2[i].z = i - (sideSize * uv2[i].w);
                }

                patchedMesh.SetUVs( 0, new List<Vector4>( uv2 ) );

                GetComponent<SkinnedMeshRenderer>().sharedMesh = patchedMesh;
            }


        }



        void GetPropertyBlock( ref MaterialPropertyBlock block, int index ) {

            if ( !rend ) {
                rend = GetComponent<Renderer>();
            }

            if ( rend ) {
                if ( sharedMats.Length != rend.sharedMaterials.Length ) {
                    sharedMats = rend.sharedMaterials;
                    SetPropertyBlock( null, index );
                }

                sharedMats = rend.sharedMaterials;
            }

            if ( block == null || index < 0 || !rend || sharedMats.Length <= index ) {
                return;
            }
            else {
#if UNITY_2018_OR_NEWER
                rend.GetPropertyBlock(block, index);
#else
                sharedMats = rend.sharedMaterials;
                var t = sharedMats[0];
                sharedMats[0] = sharedMats[index];
                rend.sharedMaterials = sharedMats;
                rend.GetPropertyBlock( block );
                sharedMats[0] = t;
                rend.sharedMaterials = sharedMats;
#endif
            }
        }


        void SetPropertyBlock( MaterialPropertyBlock block, int index ) {

            if ( !rend ) {
                rend = GetComponent<Renderer>();
            }

            if ( rend ) {
                if ( sharedMats.Length != rend.sharedMaterials.Length ) {
                    var tempMats = rend.sharedMaterials;
                    rend.sharedMaterials = sharedMats;

                    rend.SetPropertyBlock( null );
#if UNITY_2018_1_OR_NEWER
                    for ( int i = 0; i < sharedMats.Length; i++ ) {
                        rend.SetPropertyBlock( null, i );
                    }
#endif
                    rend.sharedMaterials = tempMats;
                }

                sharedMats = rend.sharedMaterials;
            }


            if ( index < 0 || !rend || rend.sharedMaterials.Length <= index ) {
                return;
            }
            else {
                if ( skinProfiles[index].isSkinMaterial ) {
#if UNITY_2018_1_OR_NEWER
					rend.SetPropertyBlock( block, index );
#else
                    sharedMats = rend.sharedMaterials;
                    var t = sharedMats[0];
                    sharedMats[0] = sharedMats[index];
                    rend.sharedMaterials = sharedMats;
                    rend.SetPropertyBlock( block );
                    sharedMats[0] = t;
                    rend.sharedMaterials = sharedMats;
#endif
                }
                else {

#if UNITY_2018_1_OR_NEWER
					//rend.SetPropertyBlock( block, index );
#else
                    sharedMats = rend.sharedMaterials;
                    var t = sharedMats[0];
                    sharedMats[0] = sharedMats[index];
                    rend.sharedMaterials = sharedMats;
                    rend.SetPropertyBlock( block );
                    sharedMats[0] = t;
                    rend.sharedMaterials = sharedMats;

#endif
                }
            }
        }





    }



}