#define SKIN2_PRO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace AdvancedSkin2 {

    [CustomEditor( typeof( PIDI_AdvancedSkinShader ) )]
    public class PIDI_AdvancedSkinEditor : Editor {

        public Texture2D logo;

        public GUISkin pidiSkin2;

        public PIDI_AdvancedSkinShader skinShader;

        public bool[] folds = new bool[0];

        public bool[] snapFolds = new bool[0];

        public void OnEnable() {
            skinShader = (PIDI_AdvancedSkinShader)target;
            if ( skinShader.GetComponent<SkinnedMeshRenderer>() && !skinShader.originalMesh ) {
                    skinShader.originalMesh = skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }
            if ( skinShader.GetComponent<SkinnedMeshRenderer>() && !skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh ) {
                skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh = skinShader.originalMesh;
            }
        }



        void GeneralSettingsUI() {
            if ( BeginCenteredGroup( "GENERAL SETTINGS", ref skinShader.folds[0] ) ) {



                if ( skinShader.skinProfiles[skinShader.currentSkinProfile].isSkinMaterial ) {
                    GUILayout.Space( 16 );
#if SKIN2_PRO
                    skinShader.skinProfiles[skinShader.currentSkinProfile].enableWrinkles = EnableDisableToggle( new GUIContent( "DYNAMIC WRINKLES", "Enables or disables the dynamic wrinkles support for this skin shader" ), skinShader.skinProfiles[skinShader.currentSkinProfile].enableWrinkles );
#endif
                    if ( skinShader.skinProfiles[skinShader.currentSkinProfile].isSkinMaterial && (!skinShader.GetComponent<Renderer>().sharedMaterials[skinShader.currentSkinProfile].shader.name.Contains( "Mobile" ) || !skinShader.GetComponent<Renderer>().sharedMaterials[skinShader.currentSkinProfile].shader.name.Contains( "TensionMap" )) ) {
                        skinShader.skinProfiles[skinShader.currentSkinProfile].enableDecals = EnableDisableToggle( new GUIContent( "DECAL / OVERLAY EFFECTS", "Enables or disables the skin decals and overlay effects" ), skinShader.skinProfiles[skinShader.currentSkinProfile].enableDecals );
                    }

                    GUILayout.Space( 16 );
                }
                else {
                    GUILayout.Space( 8 );
                    HelpBox( "This material slot does not have a PIDI Advanced Skin material assigned to it, thus none of the skin rendering properties are enabled", MessageType.Warning );
                    GUILayout.Space( 8 );
                }
            }
            EndCenteredGroup();
        }


        void DeferredLightsUI() {
            if ( skinShader.deferredMode ) {
                if ( BeginCenteredGroup( "DEFERRED LIGHTS", ref skinShader.folds[1] ) ) {

                    GUILayout.Space( 16 );

                    for ( int i = 0; i < skinShader.deferredLights.Length; i++ ) {
                        skinShader.deferredLights[i] = ObjectField<Light>( new GUIContent( "LIGHT OBJECT " + (i + 1), "The light object that will be tracked and have influence over the scattering and translucency of this object in deferred mode" ), skinShader.deferredLights[i] );
                    }

                    GUILayout.Space( 16 );

                }
                EndCenteredGroup();
            }

        }


        void SkinSurfaceSettingsUI() {


            if ( BeginCenteredGroup( "SKIN SURFACE SETTINGS", ref skinShader.folds[2] ) ) {

                GUILayout.Space( 16 );



                CenteredLabel( "PBR RENDERING" );
                GUILayout.Space( 16 );

                skinShader.skinProfiles[skinShader.currentSkinProfile].colorMap = ObjectField<Texture2D>( new GUIContent( "MAIN TEXTURE", "The main texture input for this material" ), skinShader.skinProfiles[skinShader.currentSkinProfile].colorMap, false );


                skinShader.skinProfiles[skinShader.currentSkinProfile].normalMap = ObjectField<Texture2D>( new GUIContent( "NORMALS MAP", "The main normal map for this object. If dynamic wrinkles are being used, it should be the 'resting pose' normal map" ), skinShader.skinProfiles[skinShader.currentSkinProfile].normalMap );

                GUILayout.Space( 8 );

                skinShader.skinProfiles[skinShader.currentSkinProfile].specColor = skinShader.skinProfiles[skinShader.currentSkinProfile].specGlossMap ? Color.white : ColorField( new GUIContent( "SPECULAR COLOR" ), skinShader.skinProfiles[skinShader.currentSkinProfile].specColor );
                skinShader.skinProfiles[skinShader.currentSkinProfile].glossinessLevel = SliderField( new GUIContent( "GLOSSINESS" ), skinShader.skinProfiles[skinShader.currentSkinProfile].glossinessLevel );
                skinShader.skinProfiles[skinShader.currentSkinProfile].specGlossMap = ObjectField<Texture2D>( new GUIContent( "SPECULAR GLOSS MAP", "The main specular/gloss map for this object." ), skinShader.skinProfiles[skinShader.currentSkinProfile].specGlossMap );

                GUILayout.Space( 8 );

                skinShader.skinProfiles[skinShader.currentSkinProfile].occlusionStrength = SliderField( new GUIContent( "OCCLUSION STRENGTH" ), skinShader.skinProfiles[skinShader.currentSkinProfile].occlusionStrength );
                skinShader.skinProfiles[skinShader.currentSkinProfile].occlusionMap = ObjectField<Texture2D>( new GUIContent( "OCCLUSION MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].occlusionMap );

                GUILayout.Space( 16 );
                CenteredLabel( "SKIN RENDERING" );
                GUILayout.Space( 16 );

                skinShader.skinProfiles[skinShader.currentSkinProfile].skinColor = ColorField( new GUIContent( "SKIN SURFACE COLOR", "The overall tint to be applied to the skin" ), skinShader.skinProfiles[skinShader.currentSkinProfile].skinColor );
                skinShader.skinProfiles[skinShader.currentSkinProfile].sssColor = ColorField( new GUIContent( "SUBSURFACE COLOR", "The color used for the subsurface scattering and the skin's translucency" ), skinShader.skinProfiles[skinShader.currentSkinProfile].sssColor );

                if ( skinShader.skinProfiles[skinShader.currentSkinProfile].enableTranslucency ) {
                    GUILayout.Space( 8 );
                    skinShader.skinProfiles[skinShader.currentSkinProfile].translucencyStrength = SliderField( new GUIContent( "TRANSLUCENCY STRENGTH", "The strength of the translucency effect" ), skinShader.skinProfiles[skinShader.currentSkinProfile].translucencyStrength, 0.05f, 3.0f );
                    skinShader.skinProfiles[skinShader.currentSkinProfile].translucencyMap = ObjectField<Texture2D>( new GUIContent( "TRANSLUCENCY MAP", "A texture map that controls the amount of light that goes through every part of the mesh. A depth map." ), skinShader.skinProfiles[skinShader.currentSkinProfile].translucencyMap );
                }

                GUILayout.Space( 8 );

                skinShader.skinProfiles[skinShader.currentSkinProfile].microSkinMap = ObjectField<Texture2D>( new GUIContent( "MICRO SKIN MAP", "The micro details map that contains the finer details of the skin surface" ), skinShader.skinProfiles[skinShader.currentSkinProfile].microSkinMap );
                if ( skinShader.skinProfiles[skinShader.currentSkinProfile].microSkinMap ) {
                    skinShader.skinProfiles[skinShader.currentSkinProfile].microSkinUVScale = SliderField( new GUIContent( "MICRO SKIN UV SCALE", "The scale/tiling of the micro skin texture" ), skinShader.skinProfiles[skinShader.currentSkinProfile].microSkinUVScale, 0.1f, 32 );
                }

                GUILayout.Space( 24 );

            }
            EndCenteredGroup();

        }


        void WrinkleSettingsUI() {

            if (skinShader.patchState == 0 ) {
                skinShader.originalMesh = skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh ? skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh : skinShader.originalMesh;
            }

            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].isSkinMaterial && skinShader.skinProfiles[skinShader.currentSkinProfile].enableWrinkles ) {

                if ( BeginCenteredGroup( "DYNAMIC WRINKLE SETTINGS", ref skinShader.folds[3] ) ) {

                    GUILayout.Space( 16 );

                    var buffBone = skinShader.headBone;

                    skinShader.skinProfiles[skinShader.currentSkinProfile].wrinklesMode = skinShader.GetComponent<Renderer>().sharedMaterials[skinShader.currentSkinProfile].shader.name.Contains( "Tension" ) ? WrinkleSetupMode.TensionMap : WrinkleSetupMode.ExplicitRegions;

                    skinShader.headBone = ObjectField<Transform>( new GUIContent( "HEAD BONE" ), skinShader.headBone );

                    if ( buffBone != skinShader.headBone && skinShader.headBone ) {
                        var tempRot = skinShader.transform.root.rotation;
                        skinShader.transform.root.rotation = Quaternion.identity;
                        skinShader.hRotation = skinShader.headBone.rotation;
                        skinShader.transform.root.rotation = tempRot;
                    }

                    if ( skinShader.headBone ) {

                        GUILayout.Space( 8 );

                        if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinklesMode == WrinkleSetupMode.ExplicitRegions ) {

                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataAsset = ObjectField<WrinkleDataAsset>( new GUIContent( "WRINKLE DATA ASSET", "The wrinkle data asset containing the actual texture arrays used by the shader to compute dynamic wrinkles" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataAsset, false );

                            skinShader.skinProfiles[skinShader.currentSkinProfile].regionsMap = ObjectField<Texture2D>( new GUIContent( "WRINKLE REGIONS MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].regionsMap );

                            GUILayout.Space( 16 );

                            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataAsset ) {

                                if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets == null ) {
                                    skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets = new WrinklesSet[0];
                                }

                                if ( folds.Length != skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets.Length ) {
                                    folds = new bool[skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets.Length];
                                }

                                if ( snapFolds.Length != skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets.Length * 2 ) {
                                    snapFolds = new bool[skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets.Length * 2];
                                }

                                var bshapesNames = new List<string>();

                                for ( int b = 0; b < skinShader.originalMesh.blendShapeCount; b++ ) {
                                    bshapesNames.Add( skinShader.originalMesh.GetBlendShapeName( b ) );
                                }

                                if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets.Length > 0 ) {

                                    for ( int w = 0; w < skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets.Length; w++ ) {
                                        GUILayout.Space( 4 );
                                        if ( BeginSmallFold( "WRINKLE DATA SET " + w, ref folds[w], new Color( 0.95f, 0.95f, 1.0f ) ) ) {
                                            GUILayout.Space( 16 );

                                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].normalMapID = PopupField( new GUIContent( "NORMAL MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].normalMapID, skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataAsset.NormalMapNames );
                                            //skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].occlusionMapID = PopupField( new GUIContent( "OCCLUSION MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].occlusionMapID, skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataAsset.OcclusionMapNames );
                                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsMapOverride = ObjectField<Texture2D>( new GUIContent( "REGIONS MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsMapOverride );

                                            GUILayout.Space( 16 );

                                            CenteredLabel( "REGIONS INFLUENCE" );

                                            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend == null || skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend.Length < 20 ) {
                                                skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend = new float[20];
                                            }


                                            GUILayout.Space( 8 );

                                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[0] = SliderField( new GUIContent( "RED AREA BLEND" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[0], 0, 1 );
                                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[1] = SliderField( new GUIContent( "GREEN AREA BLEND" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[1], 0, 1 );
                                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[2] = SliderField( new GUIContent( "BLUE AREA BLEND" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[2], 0, 1 );
                                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[3] = SliderField( new GUIContent( "INVERSE ALPHA BLEND" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].regionsBlend[3], 0, 1 );

                                            GUILayout.Space( 16 );
                                            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs.Length < skinShader.originalMesh.blendShapeCount ) {
                                                if ( BeginSmallFold( "TRACKED BLENDSHAPES", ref snapFolds[w], Color.white ) ) {

                                                    GUILayout.Space( 8 );

                                                    if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs == null ) {
                                                        skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs = new int[0];
                                                    }

                                                    for ( int b = 0; b < skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs.Length; b++ ) {

                                                        GUILayout.BeginHorizontal();

                                                        skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs[b] = PopupField( new GUIContent( "BLENDSHAPE " + b + " ID" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs[b], bshapesNames.ToArray() );

                                                        GUILayout.Space( 8 );

                                                        if ( Button( "X", 24 ) ) {
                                                            ArrayUtility.RemoveAt<int>( ref skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs, b );
                                                            GUILayout.EndHorizontal();
                                                            break;
                                                        }

                                                        GUILayout.EndHorizontal();

                                                        GUILayout.Space( 8 );
                                                    }

                                                    if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs.Length < skinShader.originalMesh.blendShapeCount ) {
                                                        if ( CenteredButton( "NEW BLENDSHAPE TRACK", 200 ) ) {
                                                            ArrayUtility.Add<int>( ref skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].blendshapeIDs, 0 );
                                                        }
                                                    }
                                                    GUILayout.Space( 8 );

                                                }
                                                EndSmallFold( snapFolds[w] );

                                                GUILayout.Space( 12 );
                                            }

                                            if ( BeginSmallFold( "TRACKED BONES", ref snapFolds[w + 1], Color.white ) ) {

                                                GUILayout.Space( 8 );

                                                if ( skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones == null ) {
                                                    skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones = new Transform[0];
                                                }


                                                for ( int b = 0; b < skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones.Length; b++ ) {

                                                    GUILayout.BeginHorizontal();

                                                    skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones[b] = ObjectField<Transform>( new GUIContent( "TRACKED BONE " + b ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones[b] );

                                                    GUILayout.Space( 8 );

                                                    if ( Button( "X", 32 ) ) {
                                                        ArrayUtility.RemoveAt<Transform>( ref skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones, b );
                                                        GUILayout.EndHorizontal();
                                                        break;
                                                    }

                                                    GUILayout.EndHorizontal();

                                                    GUILayout.Space( 8 );
                                                }

                                                if ( CenteredButton( "NEW BONE TRACK", 200 ) ) {
                                                    ArrayUtility.Add<Transform>( ref skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets[w].trackedBones, null );
                                                }

                                                GUILayout.Space( 8 );

                                            }
                                            EndSmallFold( snapFolds[w + 1] );

                                            GUILayout.Space( 16 );

                                            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

                                            if ( Button( "SAVE POSE SNAPSHOT", 200 ) ) {
                                                skinShader.SavePoseSnapshot( skinShader.currentSkinProfile, w );
                                            }

                                            GUILayout.Space( 16 );

                                            if ( Button( "REMOVE WRINKLES SET", 200 ) ) {
                                                ArrayUtility.RemoveAt<WrinklesSet>( ref skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets, w );
                                                EndSmallFold( folds[w] );
                                                break;
                                            }

                                            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

                                            GUILayout.Space( 16 );
                                        }
                                        EndSmallFold( folds[w] );
                                        GUILayout.Space( 4 );
                                    }

                                    GUILayout.Space( 16 );
                                }

                                if ( CenteredButton( "ADD NEW WRINKLES SNAPSHOT", 400 ) ) {
                                    ArrayUtility.Add<WrinklesSet>( ref skinShader.skinProfiles[skinShader.currentSkinProfile].wrinkleDataSets, new WrinklesSet( 0 ) );
                                }

                            }
                        }
                        else {
                            skinShader.skinProfiles[skinShader.currentSkinProfile].debugTensionMap = EnableDisableToggle( new GUIContent( "DEBUG MODE", "Enables the debug mode for this shader, which will show the tension map as it is dynamically generated by the GPU" ), skinShader.skinProfiles[skinShader.currentSkinProfile].debugTensionMap == 1 ) ? 1 : 0;

                            skinShader.skinProfiles[skinShader.currentSkinProfile].minVertexDistance = SliderFieldPrecise( new GUIContent( "VERTEX MIN. THRESHOLD", "The threshold for vertex distance detection used to generate the tension map" ), skinShader.skinProfiles[skinShader.currentSkinProfile].minVertexDistance, 0.0005f, 0.01f );

                            GUILayout.Space( 8 );

                            skinShader.skinProfiles[skinShader.currentSkinProfile].wrinklesMap = ObjectField<Texture2D>( new GUIContent( "WRINKLES NORMAL MAP", "The normal map containing all the wrinkles for this mesh" ), skinShader.skinProfiles[skinShader.currentSkinProfile].wrinklesMap );

                            GUILayout.Space( 16 );

                            switch ( skinShader.patchState ) {
                                case 0:
                                    if ( CenteredButton( "SAVE REST POSE", 200 ) ) {
                                        skinShader.GeneratePoseData( skinShader.currentSkinProfile, 0 );
                                    }
                                    break;

                                case 1:
                                    if ( CenteredButton( "SAVE POSE ONE", 200 ) ) {
                                        skinShader.GeneratePoseData( skinShader.currentSkinProfile, 1 );
                                    }
                                    break;

                                case 2:
                                    if ( CenteredButton( "SAVE POSE TWO", 200 ) ) {
                                        skinShader.GeneratePoseData( skinShader.currentSkinProfile, 2 );
                                    }
                                    break;

                                case 3:
                                    if ( CenteredButton( "RESTORE TO ORIGINAL", 200 ) ) {
                                        if ( skinShader.GetComponent<MeshFilter>() ) {
                                            skinShader.GetComponent<MeshFilter>().sharedMesh = skinShader.originalMesh;
                                        }
                                        else if ( skinShader.GetComponent<SkinnedMeshRenderer>() ) {
                                            skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh = skinShader.originalMesh;
                                        }
                                        skinShader.patchState = 0;

                                        if ( skinShader.patchedMesh ) {
                                            DestroyImmediate( skinShader.patchedMesh );
                                        }

                                    }
                                    GUILayout.Space( 8 );
                                    if ( CenteredButton( "SAVE PATCHED MESH", 200 ) ) {
                                        var path = EditorUtility.SaveFilePanelInProject( "Export Mesh", "New Patched Mesh.asset", "asset", "Export the patched mesh to the project. This is necessary if you plan to use it as a prefab" );
                                        if ( !string.IsNullOrEmpty( path ) ) {
                                            var m = Instantiate( skinShader.patchedMesh );
                                            m.name = skinShader.patchedMesh.name;
                                            AssetDatabase.CreateAsset( m, path );
                                            AssetDatabase.SaveAssets();
                                            AssetDatabase.Refresh();
                                            Debug.Log( "Mesh Exported" );
                                            skinShader.originalMesh = m;
                                            skinShader.GetComponent<SkinnedMeshRenderer>().sharedMesh = m;
                                            DestroyImmediate( skinShader.patchedMesh );
                                            skinShader.patchedMesh = m;
                                            skinShader.patchState = 0;

                                        }
                                    }

                                    break;
                            }


                        }


                    }
                    GUILayout.Space( 16 );
                }

                EndCenteredGroup();
            }

        }


        void SkinFXUI() {
            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].isSkinMaterial && skinShader.skinProfiles[skinShader.currentSkinProfile].enableDecals ) {

                if ( BeginCenteredGroup( "SKIN FX SETTINGS", ref skinShader.folds[4] ) ) {

                    GUILayout.Space( 16 );

                    //TODO : INTEGRATED RAIN SIMULATION
                    BeginSmallGroup( "OVERLAY TEXTURE 0" );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal0BlendMode = PopupField( new GUIContent( "BLENDING MODE" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal0BlendMode, new string[] { "MASKED", "ADDITIVE", "MULTIPLIED" } );

                    GUILayout.Space( 8 );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Color = ColorField( new GUIContent( "COLOR" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Color );
                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Tex = ObjectField<Texture2D>( new GUIContent( "COLOR MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Tex );

                    GUILayout.Space( 8 );
                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal0SpecCol = ColorField( new GUIContent( "SPEC COLOR (RGB) GLOSS(A)" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal0SpecCol );
                    
                    if (!skinShader.skinProfiles[skinShader.currentSkinProfile].mobileMode)
                        skinShader.skinProfiles[skinShader.currentSkinProfile].decal0SpecMap = ObjectField<Texture2D>( new GUIContent( "SPEC GLOSS MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal0SpecMap );

                    GUILayout.Space( 8 );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal0UVSet = PopupField( new GUIContent( "UV SET" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal0UVSet, new string[] { "UV0", "UV1" } );

                    var scale = new Vector2( skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Coords.x, skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Coords.y );
                    var offset = new Vector2( skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Coords.z, skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Coords.w );

                    scale = Vector2Field( new GUIContent( "SCALE" ), scale );
                    offset = Vector2Field( new GUIContent( "OFFSET" ), offset );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal0Coords = new Vector4( scale.x, scale.y, offset.x, offset.y );

                    GUILayout.Space( 12 );
                    EndSmallGroup();
                    GUILayout.Space( 16 );

                    BeginSmallGroup( "OVERLAY TEXTURE 1" );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal1BlendMode = PopupField( new GUIContent( "BLENDING MODE" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal1BlendMode, new string[] { "MASKED", "ADDITIVE", "MULTIPLIED" } );

                    GUILayout.Space( 8 );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Color = ColorField( new GUIContent( "COLOR" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Color );
                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Tex = ObjectField<Texture2D>( new GUIContent( "COLOR MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Tex );

                    GUILayout.Space( 8 );
                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal1SpecCol = ColorField( new GUIContent( "SPEC COLOR (RGB) GLOSS(A)" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal1SpecCol );
                    
                    if ( !skinShader.skinProfiles[skinShader.currentSkinProfile].mobileMode )
                        skinShader.skinProfiles[skinShader.currentSkinProfile].decal1SpecMap = ObjectField<Texture2D>( new GUIContent( "SPEC GLOSS MAP" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal1SpecMap );

                    GUILayout.Space( 8 );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal1UVSet = PopupField( new GUIContent( "UV SET" ), skinShader.skinProfiles[skinShader.currentSkinProfile].decal1UVSet, new string[] { "UV0", "UV1" } );

                    var scale1 = new Vector2( skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Coords.x, skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Coords.y );
                    var offset1 = new Vector2( skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Coords.z, skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Coords.w );

                    scale1 = Vector2Field( new GUIContent( "SCALE" ), scale1 );
                    offset1 = Vector2Field( new GUIContent( "OFFSET" ), offset1 );

                    skinShader.skinProfiles[skinShader.currentSkinProfile].decal1Coords = new Vector4( scale1.x, scale1.y, offset1.x, offset1.y );

                    GUILayout.Space( 12 );
                    EndSmallGroup();
                    GUILayout.Space( 16 );

                }
                EndCenteredGroup();

            }
        }


        void HelpAndSupportUI() {

            if ( BeginCenteredGroup( "HELP & SUPPORT", ref skinShader.folds[5] ) ) {

                GUILayout.Space( 16 );
                CenteredLabel( "SUPPORT AND ASSISTANCE" );
                GUILayout.Space( 10 );

                HelpBox( "Please make sure to include the following information with your request :\n - Invoice number\n - Screenshots of the PIDI_AdvancedSkinShader component and its settings\n - Steps to reproduce the issue.\n\nOur support service usually takes 1-2 business days to reply, so please be patient. We always reply to all emails.", MessageType.Info );

                GUILayout.Space( 8 );
                GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
                GUILayout.Label( "For support, contact us at : support@irreverent-software.com", pidiSkin2.label );
                GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

                GUILayout.Space( 8 );

                GUILayout.Space( 16 );
                CenteredLabel( "ONLINE TUTORIALS" );
                GUILayout.Space( 10 );
                if ( CenteredButton( "INSTALLATION", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#quick_start_guide" );
                }
                if ( CenteredButton( "LWRP & URP LIMITS", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#lwrp_universal_rp_vs_built_in_forward_vs_built_in_deferred" );
                }
                if ( CenteredButton( "BASIC SKIN RENDERING", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#setting_up_the_skin_shader_on_a_character_model" );
                }
                if ( CenteredButton( "DEFERRED LIGHTING", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#deferred_setup" );
                }
                if ( CenteredButton( "TENSION MAP WRINKLES", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#tension_map_wrinkles" );
                }
                if ( CenteredButton( "REGION MAP WRINKLES", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#region_map_wrinkles" );
                }

                if ( CenteredButton( "DECALS / OVERLAYS", 250 ) ) {
                    Help.BrowseURL( "https://pidiwiki.irreverent-software.com/wiki/doku.php?id=pidi_advanced_skin_shader_2#texture_overlay_fx" );
                }

                GUILayout.Space( 24 );
                CenteredLabel( "ABOUT PIDI : ADVANCED SKIN SHADER™ 2" );
                GUILayout.Space( 12 );

                HelpBox( "PIDI : Advanced Skin Shader 2 has been integrated in dozens of projects by hundreds of users since 2018.\nYour use and support to this tool is what keeps it growing, evolving and adapting to better suit your needs and keep providing you with the best quality assets for Unity.\n\nIf this tool has been useful for your project, please consider taking a minute to rate and review it, to help us to continue its development for a long time.", MessageType.Info );

                GUILayout.Space( 8 );
                if ( CenteredButton( "REVIEW ADVANCED SKIN SHADER 2", 250 ) ) {
                    Help.BrowseURL( "https://assetstore.unity.com/packages/tools/particles-effects/pidi-advanced-skin-shader-2-standard-edition-148546" );
                }
                GUILayout.Space( 8 );
                if ( CenteredButton( "ABOUT THIS VERSION", 250 ) ) {
                    Help.BrowseURL( "https://assetstore.unity.com/packages/tools/particles-effects/pidi-advanced-skin-shader-2-standard-edition-148546" );
                }
                GUILayout.Space( 8 );
            }
            EndCenteredGroup();

        }


        public override void OnInspectorGUI() {

            skinShader = (PIDI_AdvancedSkinShader)target;


            Undo.RecordObject( skinShader, "ADVSKN2" + skinShader.GetInstanceID() );

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical( pidiSkin2 ? pidiSkin2.box : null );

            AssetLogoAndVersion();

#if UNITY_2019_2_OR_NEWER
            if ( UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset != null ) {
                skinShader.srpMode = true;
            }
#endif

            if ( skinShader.srpMode ) {
                HelpBox( "SRP Mode has been enabled. Skin rendering in Lightweight / Universal SRP has some limitations when compared to the Standard / Built-in Rendering pipeline. Please read the documentation for more information", MessageType.Info );
            }

            if ( !skinShader.srpMode && Camera.main && Camera.main.actualRenderingPath == RenderingPath.DeferredShading ) {
                HelpBox( "Deferred Shading Rendering has been detected. Skin rendering in deferred has some limitations when compared to the Forward variants, please select the appropriate variant for your project. Please read the documentation for more information about each variant's pros and cons.", MessageType.Info );
            }

                        

            var slots = new string[skinShader.GetComponent<Renderer>().sharedMaterials.Length];

            for ( int i = 0; i < slots.Length; i++ ) {
                slots[i] = "MATERIAL SLOT " + i;
            }

            skinShader.InitialSetup();
            
            
            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].isSkinMaterial && skinShader.skinProfiles[skinShader.currentSkinProfile].mobileMode && skinShader.skinProfiles[skinShader.currentSkinProfile].wrinklesMode == WrinkleSetupMode.TensionMap ) {
                HelpBox( "Mobile skin shaders can have either tension maps or decals, not both. If you need decals and wrinkles please use the regions map method instead", MessageType.Warning );
                skinShader.skinProfiles[skinShader.currentSkinProfile].enableDecals = false;
            }

            
            GUILayout.BeginHorizontal(); GUILayout.Space( 20 );
            GUILayout.BeginVertical();
            skinShader.currentSkinProfile = PopupField( new GUIContent( "CURRENT MATERIAL SLOT", "The material slot whose properties we are currently editing." ), skinShader.currentSkinProfile, slots );
            GUILayout.Space( 8 );
            
            if ( skinShader.skinProfiles[skinShader.currentSkinProfile].isSkinMaterial && !skinShader.skinProfiles[skinShader.currentSkinProfile].mobileMode && !skinShader.srpMode ) {
                skinShader.deferredMode = EnableDisableToggle( new GUIContent( "DEFERRED COMPATIBILITY", "Enables the deferred compatibility mode and allows you to add a list of up to 4 lights that can affect this object" ), skinShader.deferredMode );
            }
            else {
                skinShader.deferredMode = false;
            }


            skinShader.updateDynamically = EnableDisableToggle( new GUIContent( "AUTO-UPDATE MATERIALS", "Automatically apply any changes to the materials and update their appearance" ), skinShader.updateDynamically );

            if ( skinShader.updateDynamically ) {
                skinShader.updateTime = SliderField( new GUIContent( "UPDATE FREQUENCY", "The time in seconds between each automatic update" ), skinShader.updateTime, 0.0f, 5.0f );
            }

            GUILayout.Space( 12 );
            GUILayout.EndVertical();
            GUILayout.Space( 20 ); GUILayout.EndHorizontal();


            GeneralSettingsUI();
            DeferredLightsUI();
            SkinSurfaceSettingsUI();
            WrinkleSettingsUI();
            SkinFXUI();
            HelpAndSupportUI();


            GUILayout.Space( 16 );

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            var lStyle = new GUIStyle();
            lStyle.fontStyle = FontStyle.Italic;
            lStyle.normal.textColor = Color.white;
            lStyle.fontSize = 8;

            GUILayout.Label( "Copyright© 2017-2019,   Jorge Pinal N.", lStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

            GUILayout.Space( 24 );
            GUILayout.EndVertical();

            GUILayout.Space( 8 );
            GUILayout.EndHorizontal();

            if ( skinShader.srpMode )
                skinShader.UpdateSkinMaterials();

        }





        #region PIDI INSPECTOR 2020


        public Color ColorField( GUIContent label, Color currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.ColorField( currentValue );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }



        /// <summary>
        /// Draws a standard object field in the PIDI 2020 style
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="inputObject"></param>
        /// <param name="allowSceneObjects"></param>
        /// <returns></returns>
        public T ObjectField<T>( GUIContent label, T inputObject, bool allowSceneObjects = true ) where T : UnityEngine.Object {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            GUI.color = Color.gray;
            inputObject = (T)EditorGUILayout.ObjectField( inputObject, typeof( T ), allowSceneObjects );
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return inputObject;
        }


        /// <summary>
        /// Draws a centered button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool CenteredButton( string label, float width ) {
            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            var tempBool = GUILayout.Button( label, pidiSkin2.customStyles[0], GUILayout.MaxWidth( width ) );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return tempBool;
        }

        /// <summary>
        /// Draws a button in the standard PIDI 2020 editor style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public bool Button( string label, float width = 0 ) {


            var tempBool = false;

            if ( width > 0 ) {
                tempBool = GUILayout.Button( label, pidiSkin2.customStyles[0], GUILayout.MaxWidth( width ), GUILayout.Height( EditorGUIUtility.singleLineHeight ) );
            }
            else {
                tempBool = GUILayout.Button( label, pidiSkin2.customStyles[0] );
            }
            return tempBool;
        }


        /// <summary>
        /// Draws the asset's logo and its current version
        /// </summary>
        public void AssetLogoAndVersion() {

            GUILayout.BeginVertical( logo, pidiSkin2 ? pidiSkin2.customStyles[1] : null );
            GUILayout.Space( 45 );
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( skinShader.Version, pidiSkin2.customStyles[2] );
            GUILayout.Space( 6 );
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a label centered in the Editor window
        /// </summary>
        /// <param name="label"></param>
        public void CenteredLabel( string label ) {

            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            GUILayout.Label( label, pidiSkin2.label );
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Begins a custom centered group similar to a foldout that can be expanded with a button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="groupFoldState"></param>
        /// <returns></returns>
        public bool BeginCenteredGroup( string label, ref bool groupFoldState ) {

            if ( GUILayout.Button( label, pidiSkin2.customStyles[0] ) ) {
                groupFoldState = !groupFoldState;
            }
            GUILayout.BeginHorizontal(); GUILayout.Space( 12 );
            GUILayout.BeginVertical();
            return groupFoldState;
        }


        /// <summary>
        /// Finishes a centered group
        /// </summary>
        public void EndCenteredGroup() {
            GUILayout.EndVertical();
            GUILayout.Space( 12 );
            GUILayout.EndHorizontal();
        }


        public bool BeginSmallFold( string label, ref bool fold, Color color ) {
            GUILayout.BeginHorizontal(); GUILayout.Space( 12 );
            GUI.color = color;
            GUILayout.BeginVertical( pidiSkin2.customStyles[0] );
            GUI.color = Color.white;
            if ( GUILayout.Button( label, pidiSkin2.customStyles[0], GUILayout.Height( EditorGUIUtility.singleLineHeight ) ) ) {
                fold = !fold;
            }
            if ( fold ) {
                GUILayout.BeginHorizontal(); GUILayout.Space( 12 );
                GUILayout.BeginVertical();
            }

            return fold;
        }

        public void EndSmallFold( bool fold ) {
            if ( fold ) {
                GUILayout.EndVertical();
                GUILayout.Space( 12 ); GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.Space( 12 );
            GUILayout.EndHorizontal();
        }



        /// <summary>
        /// Custom integer field following the PIDI 2020 editor skin
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public int IntField( GUIContent label, int currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue = EditorGUILayout.IntField( currentValue, pidiSkin2.customStyles[4] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;
        }


        public Vector2 Vector2Field( GUIContent label, Vector2 currentValue ) {

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            currentValue.x = EditorGUILayout.FloatField( currentValue.x, pidiSkin2.customStyles[4] );
            GUILayout.Space( 8 );
            currentValue.y = EditorGUILayout.FloatField( currentValue.y, pidiSkin2.customStyles[4] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );

            return currentValue;

        }


        /// <summary>
        /// Custom slider using the PIDI 2020 editor skin and adding a custom suffix to the float display
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currentValue"></param>
        /// <param name="minSlider"></param>
        /// <param name="maxSlider"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public float SliderField( GUIContent label, float currentValue, float minSlider = 0.0f, float maxSlider = 1.0f, string suffix = "" ) {

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            GUI.color = Color.gray;
            currentValue = GUILayout.HorizontalSlider( currentValue, minSlider, maxSlider, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb );
            GUI.color = Color.white;
            GUILayout.Space( 12 );
            currentValue = Mathf.Clamp( EditorGUILayout.FloatField( float.Parse( currentValue.ToString( "n2" ) ), pidiSkin2.customStyles[4], GUILayout.MaxWidth( 65 ) ), minSlider, maxSlider );
            GUILayout.EndHorizontal();
            GUILayout.Space( 4 );

            return currentValue;
        }

        public float SliderFieldPrecise( GUIContent label, float currentValue, float minSlider = 0.0f, float maxSlider = 1.0f, string suffix = "" ) {

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            GUI.color = Color.gray;
            currentValue = GUILayout.HorizontalSlider( currentValue, minSlider, maxSlider, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb );
            GUI.color = Color.white;
            GUILayout.Space( 12 );
            currentValue = Mathf.Clamp( EditorGUILayout.FloatField( float.Parse( currentValue.ToString( "n5" ) ), pidiSkin2.customStyles[4], GUILayout.MaxWidth( 65 ) ), minSlider, maxSlider );
            GUILayout.EndHorizontal();
            GUILayout.Space( 4 );

            return currentValue;
        }


        /// <summary>
        /// Draw a custom popup field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public int PopupField( GUIContent label, int selected, string[] options ) {


            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            selected = EditorGUILayout.Popup( selected, options, pidiSkin2.customStyles[0] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return selected;
        }



        /// <summary>
        /// Draw a custom toggle that instead of using a check box uses an Enable/Disable drop down menu
        /// </summary>
        /// <param name="label"></param>
        /// <param name="toggleValue"></param>
        /// <returns></returns>
        public bool EnableDisableToggle( GUIContent label, bool toggleValue ) {

            int option = toggleValue ? 1 : 0;

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            option = EditorGUILayout.Popup( option, new string[] { "DISABLED", "ENABLED" }, pidiSkin2.customStyles[0] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return option == 1;
        }


        /// <summary>
        /// Draw an enum field but changing the labels and names of the enum to Upper Case fields
        /// </summary>
        /// <param name="label"></param>
        /// <param name="userEnum"></param>
        /// <returns></returns>
        public int UpperCaseEnumField( GUIContent label, System.Enum userEnum ) {

            var names = System.Enum.GetNames( userEnum.GetType() );

            for ( int i = 0; i < names.Length; i++ ) {
                names[i] = System.Text.RegularExpressions.Regex.Replace( names[i], "(\\B[A-Z])", " $1" ).ToUpper();
            }

            GUILayout.Space( 2 );
            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );
            var result = EditorGUILayout.Popup( System.Convert.ToInt32( userEnum ), names, pidiSkin2.customStyles[0] );
            GUILayout.EndHorizontal();
            GUILayout.Space( 2 );
            return result;
        }


        /// <summary>
        /// Draw a layer mask field in the PIDI 2020 style
        /// </summary>
        /// <param name="label"></param>
        /// <param name="selected"></param>
        public LayerMask LayerMaskField( GUIContent label, LayerMask selected ) {

            List<string> layers = null;
            string[] layerNames = null;

            if ( layers == null ) {
                layers = new List<string>();
                layerNames = new string[4];
            }
            else {
                layers.Clear();
            }

            int emptyLayers = 0;
            for ( int i = 0; i < 32; i++ ) {
                string layerName = LayerMask.LayerToName( i );

                if ( layerName != "" ) {

                    for ( ; emptyLayers > 0; emptyLayers-- ) layers.Add( "Layer " + (i - emptyLayers) );
                    layers.Add( layerName );
                }
                else {
                    emptyLayers++;
                }
            }

            if ( layerNames.Length != layers.Count ) {
                layerNames = new string[layers.Count];
            }
            for ( int i = 0; i < layerNames.Length; i++ ) layerNames[i] = layers[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label( label, pidiSkin2.label, GUILayout.Width( EditorGUIUtility.labelWidth ) );

            selected.value = EditorGUILayout.MaskField( selected.value, layerNames, pidiSkin2.customStyles[0] );

            GUILayout.EndHorizontal();

            return selected;
        }



        public void HelpBox( string message, MessageType messageType ) {
            GUILayout.Space( 8 );
            GUILayout.BeginHorizontal(); GUILayout.Space( 8 );
            GUILayout.BeginVertical( pidiSkin2.customStyles[5] );

            GUILayout.Space( 4 );
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();

            var mType = "INFO";

            switch ( messageType ) {
                case MessageType.Error:
                    mType = "ERROR";
                    break;

                case MessageType.Warning:
                    mType = "WARNING";
                    break;
            }

            var tStyle = new GUIStyle();
            tStyle.fontSize = 11;
            tStyle.fontStyle = FontStyle.Bold;
            tStyle.normal.textColor = Color.black;

            GUILayout.Label( mType, tStyle );

            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.Space( 4 );

            GUILayout.BeginHorizontal(); GUILayout.Space( 8 ); GUILayout.BeginVertical();
            tStyle.fontSize = 9;
            tStyle.fontStyle = FontStyle.Normal;
            tStyle.wordWrap = true;
            GUILayout.TextArea( message, tStyle );

            GUILayout.Space( 8 );
            GUILayout.EndVertical(); GUILayout.Space( 8 ); GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space( 8 ); GUILayout.EndHorizontal();
            GUILayout.Space( 8 );
        }



        public void BeginSmallGroup( string label = "" ) {
            GUILayout.BeginHorizontal(); GUILayout.Space( 20 );
            GUI.color = new Color( 0.7f, 0.7f, 0.8f, 1 );
            GUILayout.BeginVertical( pidiSkin2.customStyles[1] );
            GUI.color = Color.white;

            if ( !string.IsNullOrEmpty( label ) ) {
                GUILayout.Space( 8 );
                CenteredLabel( label );
            }
            GUILayout.BeginHorizontal(); GUILayout.Space( 12 );
            GUILayout.BeginVertical();
            GUILayout.Space( 8 );

        }


        public void EndSmallGroup() {
            GUILayout.EndVertical();
            GUILayout.Space( 12 ); GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space( 20 ); GUILayout.EndHorizontal();
        }

        #endregion
    }

}