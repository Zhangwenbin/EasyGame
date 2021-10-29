using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITweenAlpha")]
    public class UITweenAlpha : UITween<float>
    {
        [HideInInspector]
        public GameObject target;
        
        [HideInInspector]
        public bool isCanvasGroup = false;
        
        Graphic[] mGraphics;
        CanvasGroup mCanvasGroup;
        
        float mAlpha = 0f;
        
        CanvasGroup canvasGroup
        {
            get
            {
                if( mCanvasGroup == null )
                {
                    if( target == null )
                    {
                        target = gameObject;
                    }
                    mCanvasGroup = target.GetComponent<CanvasGroup>();
                }
                return mCanvasGroup;
            }
        }
        
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
                    mGraphics = target.GetComponentsInChildren<Graphic>();
                }
                return mGraphics;
            }
        }
        
        public override float value
        {
            get
            {
                return mAlpha;
            }
            set
            {
                SetAlpha( value );
                mAlpha = value;
            }
        }
        
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            target = UnityEditor.EditorGUILayout.ObjectField( "Target", target, typeof(GameObject), true ) as GameObject;
            isCanvasGroup = UnityEditor.EditorGUILayout.Toggle( "isCanvasGroup", isCanvasGroup );
            
            UnityEditor.EditorGUILayout.BeginHorizontal();
            {
                from = UnityEditor.EditorGUILayout.Slider( "From", from, 0.0f, 1.0f, null );
            }
            UnityEditor.EditorGUILayout.EndHorizontal();
            UnityEditor.EditorGUILayout.BeginHorizontal();
            {
                to = UnityEditor.EditorGUILayout.Slider( "To", to, 0.0f, 1.0f, null );
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
            value = Mathf.Lerp( from, to, factor );
        }
        
        void SetAlpha( float _alpha )
        {
            if( isCanvasGroup )
            {
                canvasGroup.alpha = _alpha;
            }
            else
            {
                foreach( var item in cachedGraphics )
                {
                    Color color = item.color;
                    color.a = _alpha;
                    item.color = color;
                }
            }
        }
    }
}
