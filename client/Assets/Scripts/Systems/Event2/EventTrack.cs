/**************************************************************************/
/*@brief  简要描述   
  @author zwb
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EG
{
    public class EventTrack : ScriptableObject
    {
        public static EventTrackStatus  CurrentStatus   = null;
        public static float             DeltaTime       = 0;
        
        public enum ECategory
        {
            None    = 0,    // 
            Common  ,       // 
            Unit    ,       // 
            UI      ,       // UI
        }
        
        public float        Start       = 0;
        public float        End         = 0;
        

        public virtual ECategory Category
        {
            get { return ECategory.None; }
        }
        
        
        public float GetTimeScale( float time )
        {
            float range = ( End - Start );
            if( range > 0 )
            {
                time = Mathf.Clamp( time, 0, End );
                return ( time - Start ) / range;
            }
            return 1;
        }
        
 
        public virtual EventTrackStatus CreateStatus( EventPlayerStatus owner )        { return new EventTrackStatus( owner ); }
        public virtual void UpdatePreview( GameObject gobj, float time )               { }
        public virtual void OnStart( EG.AppMonoBehaviour behaviour )                   { }
        public virtual void OnUpdate( EG.AppMonoBehaviour behaviour, float time )      { }
        public virtual void OnEnd( EG.AppMonoBehaviour behaviour )                     { }
        public virtual void OnBackground( EG.AppMonoBehaviour behaviour, float time )  { }
        

        public virtual bool CheckPreLoad()
        {
            return false;
        }
        

        public virtual bool IsDonePreLoad()
        {
            return true;
        }
        
  
        public virtual void StartPreLoad()
        {
        }
        
     
        public virtual void UnloadPreLoad()
        {
        }
        
        #if UNITY_EDITOR
        

        public virtual void EditorStartPreLoad()
        {
        }
        

        public virtual void EditorUnloadPreLoad()
        {
        }
        
        #endif
        

        
        #if UNITY_EDITOR
        
        bool          m_IsReqTrackStatusUpdate    = false;
        
        public virtual bool     isCustomInspector   { get { return true;                } }
        public virtual Color    TrackColor          { get { return Color.blue;          } }
        public virtual void     OnSceneGUI( SceneView sceneView, GameObject go, float time ){ }
        public virtual void     OnInspectorGUI( Rect position, SerializedObject serializeObject, float width )
        {
            CustomFieldAttribute.OnInspectorGUI( position, this.GetType(), serializeObject, width );
        }


        public virtual void EditorRelease()
        {
        }
        

        public virtual void EditorPreProcess( EG.AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
        }
        

        public virtual void EditorPostProcess( EG.AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
        }
        

        protected void RequestTrackStatusUpdate()
        {
            m_IsReqTrackStatusUpdate = true;
        }
        
        public void ResetTrackStatusUpdate()
        {
            m_IsReqTrackStatusUpdate = false;
        }
        
        public bool IsRequestTrackStatusUpdate()
        {
            return m_IsReqTrackStatusUpdate;
        }
        
        // 
        static readonly string[] CATEGORY_TEXT_TBL = new string[]
        {
            "none",
            "通用",
            "角色",
            "UI",
        };
        
        public static string GetCategoryText( EventTrack evTrack )
        {
            ECategory category = ECategory.None;
            if( evTrack != null )
            {
                category = evTrack.Category;
            }
            return CATEGORY_TEXT_TBL[ (int)category ];
        }
        

        public static bool IsDispPopupList( ECategory own, ECategory target )
        {
            if( own == ECategory.None
            ||  target == ECategory.None )
                return true;
            
            if( own == ECategory.Common
            ||  target == ECategory.Common )
                return true;
            
            return own == target;
        }
        
        #endif

        //=========================================================================
        //private var  私有变量
        //=========================================================================

        //=========================================================================
        //public var  公有变量
        //=========================================================================

        //=========================================================================
        //static var  静态变量
        //=========================================================================

        //=========================================================================
        //property  属性
        //=========================================================================


        //=========================================================================
        //init 初始化
        //=========================================================================
        #region 初始化
        
        #endregion
        
        //=========================================================================
        //update 更新
        //=========================================================================
        #region 更新
        
        #endregion
        
        //=========================================================================
        //get/set 设置/获取
        //=========================================================================
        #region 设置/获取

        #endregion

        //=========================================================================
        //Isxxx 确认 
        //=========================================================================
        #region 确认
        
        #endregion
        
        //=========================================================================
        //preload 预加载
        //=========================================================================
        #region 预加载
        
        #endregion

        //=========================================================================
        //Editor 编辑器
        //=========================================================================
        #region 编辑器
        #if UNITY_EDITOR

        #endif
        #endregion
    }
}