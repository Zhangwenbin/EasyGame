using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG
{

    [AddComponentMenu("Scripts/Application/Common/Effect/Other/AbsorbEffect")]
    public class AbsorbEffect : AppMonoBehaviour
    {

        
        [CustomFieldAttribute("Resource",CustomFieldAttribute.Type.Component,typeof(AbsorbEffectElement))]
        public AbsorbEffectElement          Resource;
        [CustomFieldAttribute("Hit",CustomFieldAttribute.Type.Component,typeof(EffectParam))]
        public EffectParam                  Hit;
        
        [CustomFieldAttribute("Num",CustomFieldAttribute.Type.Int)]
        public int                          Num;
        
        private List<AbsorbEffectElement>   m_FreeElements  = new List<AbsorbEffectElement>( );
        private List<AbsorbEffectElement>   m_UseElements   = new List<AbsorbEffectElement>( );
        private bool                        m_AutoDestroy   = false;
        

        
        public int                          UseCount        { get { return m_UseElements.Count; } }
        public int                          FreeCount       { get { return m_FreeElements.Count; } }


        public override void Initialize( )
        {
            base.Initialize( );
            
            // ----------------------------------------
            
            if( Resource != null )
            {
                Resource.gameObject.SetActive( false );
            }
            
            CreateArray( AssetManager.Instance.gameObject, Resource, Num );
        }
        

        public override void Release( )
        {
            if( isInitialized == false ) return;
            
            for( int i = 0; i < m_FreeElements.Count; i++ )
            {
                m_FreeElements[ i ].SafeDestroy( );
            }
            m_FreeElements.Clear( );

            for( int i = 0; i < m_UseElements.Count; i++ )
            {
                m_UseElements[ i ].SafeDestroy( );
            }
            m_UseElements.Clear( );
            
            // ----------------------------------------
            
            base.Release( );
        }
        

        private void Update( )
        {
            if( m_UseElements.Count > 0 )
            {
                for( int i = m_UseElements.Count-1; i >= 0; --i )
                {
                    AbsorbEffectElement element = m_UseElements[i];
                    if( element.gameObject.activeInHierarchy == false )
                    {
                        m_UseElements.RemoveAt( i );
                        Free( element );
                    }
                }
            }
            
            if( m_AutoDestroy )
            {
                if( m_UseElements.Count == 0 )
                {
                    gameObject.SafeDestroy( );
                    m_AutoDestroy = false;
                }
            }
            
            if( m_UseElements.Count == 0 )
            {
                gameObject.SetActive( false );
            }
        }
        


        private void CreateArray( GameObject parent, AbsorbEffectElement res, int num )
        {
            if( res != null )
            {
                for( int i = 0; i < num; i++ )
                {
                    AbsorbEffectElement element = Instantiate( res ) as AbsorbEffectElement;
                    
                    element.gameObject.SetActive( false );
                    element.transform.SetParent( parent.transform, false );
                    
                    element.SetResource( res );
                    
                    m_FreeElements.Add( element );
                }
            }
        }
        

        public AbsorbEffectElement Alloc( )
        {
            if( m_FreeElements.Count > 0 )
            {
                AbsorbEffectElement element = m_FreeElements[0];
                m_FreeElements.RemoveAt( 0 );
                m_UseElements.Add( element );
                return element;
            }
            return null;
        }
        

        public void Free( AbsorbEffectElement element )
        {
            if( element != null )
            {
                element.Release( );
                m_FreeElements.Add( element );
            }
        }
        

        public void Play( Transform self, Transform target, int num, bool isAutoDestroy = false )
        {

            Transform   startTransform  = target;
            Transform   goalTransform   = self;
            Vector3     dir             = startTransform.position - goalTransform.position;
            Vector3     startPos        = target.position;
            Vector3     offset          = self.position - goalTransform.position;
            
            for( int i = 0; i < num; ++i )
            {
                AbsorbEffectElement element = Alloc( );
                if( element != null )
                {
                    Quaternion rot = Quaternion.AngleAxis( Random.Range( 135, 135 + 90 ), dir ) * Quaternion.AngleAxis( Random.Range( 70, 110 ), Vector3.right );
                    
                    element.Initialize( );
                    element.Play( startPos, offset, rot, goalTransform );
                    
                    if( i == 0 )
                    {
                        GameUtility.AttachParam attachParam = new GameUtility.AttachParam( );
                        attachParam.AttachTarget = self.FindChildAll( "Hips" );
                        if( attachParam.AttachTarget == null ) attachParam.PosOffset = goalTransform.position;
                        element.SetHitEffect( Hit, attachParam );
                    }
                }
            }
            
            m_AutoDestroy = isAutoDestroy;
            
            gameObject.SetActive( true );
        }
        
        
        public bool IsEnd( )
        {
            return m_UseElements.Count == 0;
        }
        
        
        private void OnEnd( AbsorbEffectElement element, object value )
        {
        }
        
  
    }
    

    #if UNITY_EDITOR
    
    [UnityEditor.CustomEditor(typeof(AbsorbEffect), true)]
    public class EditorInspactor_AbsorbEffect : UnityEditor.Editor
    {

        public override void OnInspectorGUI( )
        {
            CustomFieldAttribute.OnInspectorGUI( typeof( AbsorbEffect ), serializedObject );
            
            // -----------------------------------------
            
            AbsorbEffect effect = target as AbsorbEffect;
            
            UnityEditor.EditorGUILayout.LabelField( "FreeCount", effect.FreeCount.ToString( ) );
            UnityEditor.EditorGUILayout.LabelField( "使用中", effect.UseCount.ToString( ) );
        }
    }
    
    #endif
}
