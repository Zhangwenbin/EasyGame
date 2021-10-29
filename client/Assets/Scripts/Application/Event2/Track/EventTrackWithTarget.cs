using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public abstract class EventTrackWithTarget : EventTrack
    {
        // 
        public enum EAttachMode
        {
            Bone                    ,           // 
            MainCamera              ,           // 
            // MainWeapon              ,           // 
            // SubWeapon               ,           // 
        }
        
        // 
        public enum EBoneName
        {
            Root                    ,
            Hips                    ,
            RightFoot               ,
            LeftFoot                ,
            Spine                   ,
            Chest                   ,
            Neck                    ,
            Head                    ,
            HeadNub                 ,
            RightHand               ,
            RightWeapon             ,
            LeftHand                ,
            LeftWeapon              ,
        }
        
        #if UNITY_EDITOR
        public static Utility.EnumArray.Element[] EAttachModeNames =
        {
            new Utility.EnumArray.Element( EAttachMode.MainCamera           , "MainCamera" ),
            new Utility.EnumArray.Element( EAttachMode.Bone                 , "Bone" ),
            // new Utility.EnumArray.Element( EAttachMode.MainWeapon           , "MainWeapon" ),
            // new Utility.EnumArray.Element( EAttachMode.SubWeapon            , "SubWeapon" ),
        };
        #endif
        
        
        [CustomFieldGroup("附加设定")]
        [CustomFieldAttribute("AttachMode",CustomFieldAttribute.Type.Enum,typeof(EAttachMode),typeof(EventTrackWithTarget))]
        public EAttachMode  AttachMode      = EAttachMode.Bone;
        
        [CustomFieldGroup("附加设定")]
        [CustomFieldAttribute("BoneName",CustomFieldAttribute.Type.StringAndEnum,typeof(EBoneName))]
        public string       BoneName        = "";
        
        [CustomFieldGroup("附加设定")]
        [CustomFieldAttribute("Offset",CustomFieldAttribute.Type.Vector3)]
        public Vector3      Offset          = Vector3.zero;
        
        [CustomFieldGroup("附加设定")]
        [CustomFieldAttribute("角度",CustomFieldAttribute.Type.Vector3)]
        public Vector3      Rotation        = Vector3.zero;
        
        [CustomFieldGroup("附加设定")]
        [CustomFieldAttribute("LocalOffset",CustomFieldAttribute.Type.Bool)]
        public bool         LocalOffset     = true;
        
        [CustomFieldGroup("附加设定")]
        [CustomFieldAttribute("LocalRotation",CustomFieldAttribute.Type.Bool)]
        public bool         LocalRotation   = true;
        

        public GameObject GetAttachObject( GameObject gobj )
        {
            GameObject attach = null;
            
            switch( AttachMode )
            {
            // 
            case EAttachMode.MainCamera:
                attach = Camera.main != null ? Camera.main.gameObject: gobj;
                #if UNITY_EDITOR
                if( Application.isPlaying == false )
                {
                    // 
                    GameObject camObj = GameObject.Find( "evCamera" );
                    if( camObj != null )
                    {
                        attach = camObj;
                    }
                }
                #endif
                break;
            // 
            case EAttachMode.Bone:
                if( string.IsNullOrEmpty( BoneName ) == false )
                {
                    attach = gobj.FindChildAll( BoneName );
                }
                else
                {
                    attach = gobj;
                }
                break;
            // 
            // case EAttachMode.MainWeapon:
            //     {
            //         TacticsUnitController unitController = gobj.GetComponent<TacticsUnitController>( );
            //         if( unitController != null )
            //         {
            //             gobj= unitController.GetCurrentWeaponMainRoot( ).gameObject;
            //         }
            //         if( string.IsNullOrEmpty( BoneName ) == false )
            //         {
            //             attach = gobj.FindChildAll( BoneName );
            //         }
            //         else
            //         {
            //             attach = gobj;
            //         }
            //     }
            //     break;
            // // 
            // case EAttachMode.SubWeapon:
            //     {
            //         TacticsUnitController unitController = gobj.GetComponent<TacticsUnitController>( );
            //         if( unitController != null )
            //         {
            //             gobj= unitController.GetCurrentWeaponSubRoot( ).gameObject;
            //         }
            //         if( string.IsNullOrEmpty( BoneName ) == false )
            //         {
            //             attach = gobj.FindChildAll( BoneName );
            //         }
            //         else
            //         {
            //             attach = gobj;
            //         }
            //     }
            //     break;
            }
            
            return attach;
        }
        
        public void CalcPosition( GameObject gobj, Vector3 localPosition, Quaternion localRotation, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            EffectBase.GenerateOption   option  = EffectBase.GenerateOption.None;
            
            // 
            if( LocalOffset )           option |= EffectBase.GenerateOption.AttachOffset;
            if( LocalRotation )         option |= EffectBase.GenerateOption.AttachRotation;
            
            // 
            EffectBase.CalcPosition( 
                GetAttachObject( gobj ), 
                Offset, Quaternion.Euler( Rotation.x, Rotation.y, Rotation.z ), 
                localPosition, localRotation,
                option,
                out spawnPos, out spawnRot );
        }
        

        public void CalcPosition( GameObject gobj, GameObject prefab, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            CalcPosition( gobj, prefab.transform.localPosition, prefab.transform.localRotation, out spawnPos, out spawnRot );
        }
        
        
        #if UNITY_EDITOR
        
        public static string        ClassName           { get { return "WithTarget"; } }
        

        public override void OnSceneGUI( SceneView sceneView, GameObject gobj, float time )
        {
            GameObject attach = GetAttachObject( gobj );
            
            if( attach != null )
            {
                Transform target = attach.transform;
                
                Vector3 worldPos;
                if( LocalOffset )
                {
                    worldPos = target.localToWorldMatrix.MultiplyPoint( Offset );
                }
                else
                {
                    worldPos = gobj.transform.position + Offset;
                }
                
                Quaternion rot = Quaternion.Euler( Rotation );
                if( LocalRotation )
                {
                    rot = target.rotation * rot;
                }
                
                if( Tools.current == Tool.Move )
                {
                    if( LocalOffset )
                    {
                        Vector3 newPos = Handles.PositionHandle( worldPos, target.rotation );
                        newPos = target.worldToLocalMatrix.MultiplyPoint( newPos );
                        Offset = newPos;
                    }
                    else
                    {
                        Vector3 newPos = Handles.PositionHandle( worldPos, Quaternion.identity );
                        newPos = newPos - gobj.transform.position;
                        Offset = newPos;
                    }
                }
                else if( Tools.current == Tool.Rotate )
                {
                    if( LocalRotation )
                    {
                        Quaternion newRot = Handles.RotationHandle( Quaternion.Inverse( target.rotation ) * rot, worldPos );
                        Rotation = newRot.eulerAngles;
                    }
                    else
                    {
                        Quaternion newRot = Handles.RotationHandle( rot, worldPos );
                        Rotation = newRot.eulerAngles;
                    }
                }
                
                // 
                Handles.color = Color.white;
                Handles.DrawLine( target.position, worldPos );
                Handles.DotHandleCap( 0, target.position, Quaternion.identity, 0.02f, EventType.Repaint );
                
                // 
                float axisSize = 0.25f;
                Handles.color = Color.red;
                Handles.DrawLine( worldPos, worldPos + rot * Vector3.right * axisSize );
                Handles.color = Color.green;
                Handles.DrawLine( worldPos, worldPos + rot * Vector3.up * axisSize );
                Handles.color = Color.blue;
                Handles.DrawLine( worldPos, worldPos + rot * Vector3.forward * axisSize );
            }
        }
        
        #endif
    }

}
