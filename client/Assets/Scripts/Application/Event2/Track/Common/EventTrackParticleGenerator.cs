using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackParticleGenerator : EventTrackWithTarget
    {
        public enum EDispCondition
        {
            None                = 0,        // 
            StealSuccess        ,           // 
        }
        
        public enum ERoot
        {
            Self                = 0,        // 
            Target              ,           // 
            SkillUsePos         ,           // 
        }
        
        public enum EOption
        {
            NotAttach              ,        // 
            CutinHeight            ,        // 
        }
        
        #if UNITY_EDITOR
        public static Utility.EnumArray.Element[] EDispConditionNames =
        {
            new Utility.EnumArray.Element( EDispCondition.None          , "None"          ),
            new Utility.EnumArray.Element( EDispCondition.StealSuccess  , "StealSuccess"    ),
        };
        public static Utility.EnumArray.Element[] ERootNames =
        {
            new Utility.EnumArray.Element( ERoot.Self                   , "Self"              ),
            new Utility.EnumArray.Element( ERoot.Target                 , "Target"        ),
            new Utility.EnumArray.Element( ERoot.SkillUsePos            , "SkillUsePos"    ),
        };
        public static Utility.EnumArray.Element[] EOptionNames =
        {
            new Utility.EnumArray.Element( EOption.NotAttach            , "NotAttach" ),
            new Utility.EnumArray.Element( EOption.CutinHeight          , "CutinHeight" ),
        };
        #endif
        
        
        [CustomFieldGroup("设置")]
        [CustomFieldDispCondAttribute("AttachMode","MainCamera",false)]
        [CustomFieldAttribute("释放位置",CustomFieldAttribute.Type.Enum,typeof(ERoot),typeof(EventTrackParticleGenerator))]
        public ERoot            Root            = ERoot.Self;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("释放条件",CustomFieldAttribute.Type.Enum,typeof(EDispCondition),typeof(EventTrackParticleGenerator))]
        public EDispCondition   DispCondition   = EDispCondition.None;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("选项",CustomFieldAttribute.Type.BitFlag,typeof(EOption),typeof(EventTrackParticleGenerator))]
        public BitFlag          Option          = new BitFlag( );
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("SetAliveTime", CustomFieldAttribute.Type.Bool )]
        public bool             IsSetAliveTime  = false;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("EffParam", CustomFieldAttribute.Type.Component,typeof( EffectParam ) )]
        public EffectParam      EffParam        = null;
        
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        private bool IsDispCondition( AppMonoBehaviour behaviour )
        {
            if( DispCondition == EDispCondition.None ) return true;
            
            #if UNITY_EDITOR
            if( Application.isPlaying == false ) return true;
            #endif

            return true;
        }
        
        private EffectData _CreateEffect( GameObject gobj, bool localPos, Vector3 pos, bool localRot, Vector3 rot )
        {
            if( EffParam == null )
            {
                return null;
            }
            
            EffectData.Param effParam = new EffectData.Param();
            effParam.PlayMode = EffParam.Param.PlayMode;
            effParam.GenOption = new BitFlag();
            
            if( gobj != null )
            {
                if( gobj.transform.lossyScale.x * gobj.transform.lossyScale.z < 0.0f )
                    effParam.GenOption.SetValue( EffectData.EGenerateOption.Mirror );
            }
            
            if( IsSetAliveTime )
            {
                effParam.AliveTime = ( End > Start ? End - Start : 0 );
            }
            
            GameUtility.AttachParam attachParam = new GameUtility.AttachParam();
            if( localPos )
            {
                attachParam.SetPosOffset( pos );
            }
            
            if( localRot )
            {
                attachParam.SetRotOffset( Quaternion.Euler( rot.x, rot.y, rot.z ) );
            }
             
            if( gobj != null )
            {
                attachParam.AttachTarget = GetAttachObject( gobj );
            }
            
            return EffParam.CreateEffectParticle( ref effParam, ref attachParam );
        }
        
 
        public override void OnStart( AppMonoBehaviour behaviour )
        {
            GameObject gobj = behaviour.gameObject;
            
            if( EffParam == null )
            {
                return;
            }
            
            if( IsDispCondition( behaviour ) == false ) return;
            
            GameObject[] gobjs = null;
            if( Root == ERoot.Self )
            {
                gobjs = new GameObject[] { gobj };
            }
            else if( Root == ERoot.Target )
            {
                if( BattleCore.HasInstance( ) == false )
                {
                    gobjs = new GameObject[] { gobj };
                }
                else
                {
                    UnitController unitController = behaviour as UnitController;
                    if( unitController == null ) return;
                    List<UnitController> targets = unitController.SkillActuator.Targets;
                    if( targets != null )
                    {
                        gobjs = new GameObject[ targets.Count ];
                        for( int i = 0; i < targets.Count; ++i )
                        {
                            gobjs[i] = targets[i].gameObject;
                        }
                    }
                }
            }
            else if( Root == ERoot.SkillUsePos )
            {
                if( BattleCore.HasInstance( ) == false )
                {
                    gobjs = new GameObject[] { gobj };
                }
                else
                {
                    UnitController unitController = behaviour as UnitController;
                    if( unitController == null ) return;
                    
                    SkillActuator actuator = unitController.SkillActuator;
                    if( actuator == null ) return;
                    
                    EffectData effData = _CreateEffect( null, true, actuator.UsePosition, false, Vector3.zero );
                    if( effData != null )
                    {

                        effData.Play();
                    }
                    
                    return;
                }
            }
            
            for( int i = 0; i < gobjs.Length; ++i )
            {
                EffectData effData =_CreateEffect( gobjs[i], LocalOffset, Offset, LocalRotation, Rotation );
                if( effData != null )
                {
                    if( Option.HasValue( EOption.NotAttach ) )
                    {
                        effData.ResetAttach( );
                    }
                    
                    if( Option.HasValue( EOption.CutinHeight ) )
                    {
                       
                    }
                    
                    #if UNITY_EDITOR
                    EventPlayer evPlayer = behaviour as EventPlayer;
                    if( evPlayer != null && evPlayer.IsManualUpdate )
                    {
                        EditorCacheEffect( effData );
                    }
                    #endif
                    
                    effData.Play();
                }
            }
        }
        

        protected virtual void OnGenerate( EffectBase effect )
        {
        }

        #if UNITY_EDITOR
        
        new public static string    ClassName               { get { return "Particle generator";              } }
        public override Color       TrackColor              { get { return new Color32( 64, 240, 32, 0xff );        } }
        
        EffectData                  m_EditorEffectData      = null;
        Transform                   m_EditorCacheTrans      = null;
        Transform                   m_EditorParent          = null;
        Vector3                     m_EditorCacheLPos       = Vector3.zero;
        Quaternion                  m_EditorCacheRot        = Quaternion.identity;
        
        public bool HaveEffectParam
        {
            get { return EffParam != null; }
        }
        
        public override void OnInspectorGUI( Rect position, SerializedObject serializeObject, float width )
        {
            base.OnInspectorGUI( position, serializeObject, width );
            
            if( Root == ERoot.SkillUsePos )
            {
                UnityEditor.EditorGUILayout.HelpBox( "显示对象是技能发动场所的情况下连接设定无效", MessageType.Info );
            }
            
            GUI.enabled = ( EffParam != null );
            if( GUILayout.Button( "长度自动调整" ) )
            {
                GameObject gobj = EffParam?.m_Prefab;
                if( gobj != null )
                {
                    var particles = gobj.GetComponentsInChildren<ParticleSystem>( true );
                    
                    float calcTime = 0f;
                    foreach( var part in particles )
                    {
                        calcTime = Mathf.Max( calcTime, part.main.duration + part.main.startDelay.constant );
                    }
                    End = Start;
                    End += calcTime;
                    
                    EditorUtility.SetDirty( this );
                }
            }
        }
        
        public override void EditorRelease()
        {
            base.EditorRelease();
            
            _EditorReleaseEffectObject();
        }
        
        void _EditorReleaseEffectObject()
        {
            if( m_EditorEffectData != null )
            {
                m_EditorEffectData.Release();
                m_EditorCacheTrans = null;
                m_EditorParent = null;
                DestroyImmediate( m_EditorEffectData.gameObject );
                m_EditorEffectData = null;
            }
        }
        
        public override void EditorPreProcess( AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
            base.EditorPreProcess( behaviour, time, dt, isLooped, isEnded );
            
            // ----------------------------------------
        }
        
        public override void EditorPostProcess( AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
            base.EditorPostProcess( behaviour, time, dt, isLooped, isEnded );
            
            if( time < Start || ( EffParam != null && EffParam.Param.PlayMode == EffectData.EPlayMode.Loop && End <= time ) )
            {
                _EditorReleaseEffectObject();
            }
            
            if( m_EditorEffectData != null )
            {
                if( m_EditorEffectData.SimulateFromEventEditor( time, dt, Start, End, false, dt == 0, this ) )
                {
                    _EditorReleaseEffectObject();
                    return;
                }
                
                m_EditorCacheTrans.SetParent( m_EditorParent );
                m_EditorCacheTrans.localPosition = m_EditorCacheLPos;
                m_EditorCacheTrans.localRotation = m_EditorCacheRot;
                m_EditorCacheTrans.SetParent( null, true );
            }
        }
        
        void EditorCacheEffect( EffectData effData )
        {
            _EditorReleaseEffectObject();
            
            m_EditorEffectData = effData;
            m_EditorEffectData.EditorAwake();
            
            m_EditorCacheTrans = m_EditorEffectData.transform;
            m_EditorParent = m_EditorCacheTrans.parent;
            
            m_EditorCacheLPos = m_EditorCacheTrans.localPosition;
            m_EditorCacheRot = m_EditorCacheTrans.localRotation;
            
            m_EditorCacheTrans.SetParent( null, true );
        }
        
        public EffectData Instantiate( GameObject attachObj )
        {
            EffectData effData = _CreateEffect( attachObj, LocalOffset, Offset, LocalRotation, Rotation );
            if( effData != null )
            {
                effData.EditorAwake();
            }
            
            return effData;
        }
        
        #endif
    }
}

