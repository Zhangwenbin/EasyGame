using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITweenColor")]
    public class UITweenColor : UITween<Color>
    {
        [HideInInspector]
        public GameObject target;
        
        [HideInInspector]
        public bool includeChildren = false;
        
        Graphic[] mGraphics;
        
        Color mColor = Color.white;
        
        Graphic[] cachedGraphics
        {
            get
            {
                if( mGraphics == null )
                {
                    if( target == null )
                    {
                        target = gameObject;
                    }
                    mGraphics = includeChildren ? target.GetComponentsInChildren<Graphic>() : target.GetComponents<Graphic>();
                }
                return mGraphics;
            }
        }
        
        public override Color value
        {
            get
            {
                return mColor;
            }
            set
            {
                SetColor( value );
                mColor = value;
            }
        }
        
#if UNITY_EDITOR

        public override void OnInspectorGUI()
        {
            target = UnityEditor.EditorGUILayout.ObjectField( "Target", target, typeof(GameObject), true ) as GameObject;
            includeChildren = UnityEditor.EditorGUILayout.Toggle( "inChildren", includeChildren );
            
            UnityEditor.EditorGUILayout.BeginHorizontal();
            {
                from = UnityEditor.EditorGUILayout.ColorField( "From", from, null );
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.BeginHorizontal();
            {
                to = UnityEditor.EditorGUILayout.ColorField( "To", to, null );
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
        }
#endif
        
        protected override void Start()
        {
            base.Start ();
        }
        
        protected override void OnUpdate( float factor, bool isFinished )
        {
            value = Color.Lerp( from, to, factor );
        }
        

        void SetColor( Color _color )
        {
            foreach( var item in cachedGraphics )
            {
                item.color = _color;
            }
        }
    }
}
