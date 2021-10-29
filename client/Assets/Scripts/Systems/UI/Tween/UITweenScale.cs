using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITweenScale")]
    public class UITweenScale : UITween<Vector3>
    {
        RectTransform mRectTransform;
        
        Vector3 mScale = Vector3.one;
        
        public RectTransform cachedRectTransform
        {
            get
            {
                if( mRectTransform == null ) mRectTransform = GetComponent<RectTransform>();
                return mRectTransform;
            }
        }
        
        public override Vector3 value
        {
            get
            {
                return mScale;
            }
            set
            {
                SetScale( value );
                mScale = value;
            }
        }
        
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            from = UnityEditor.EditorGUILayout.Vector3Field( "From", from, null );
            to = UnityEditor.EditorGUILayout.Vector3Field( "To", to, null );
        }
#endif
        
        protected override void OnUpdate ( float factor, bool isFinished )
        {
            value = from + ( to - from ) * factor;
//          value = Vector3.Lerp( from, to, factor );
        }
        

        void SetScale( Vector3 _value )
        {
            cachedRectTransform.localScale= _value;
        }
        
        [ContextMenu("Set Sin Curve ( 0-180 )")]
        void SinCurve180()
        {
            animationCurve  = new AnimationCurve();
            animationCurve.AddKey( new Keyframe( 0.0f, 0.0f, 0.0f, 0.0f ) );
            animationCurve.AddKey( new Keyframe( 0.5f, 1.0f, 0.0f, 0.0f ) );
            animationCurve.AddKey( new Keyframe( 1.0f, 0.0f, 0.0f, 0.0f ) );
        }
        
        [ContextMenu("Set Sin Curve ( 0-360 )")]
        void SinCurve360()
        {
            animationCurve  = new AnimationCurve();
            animationCurve.AddKey( new Keyframe( 0.0f, 0.0f, 5.0f, 5.0f ) );
            animationCurve.AddKey( new Keyframe( 0.25f, 1.0f, 0.0f, 0.0f ) );
            animationCurve.AddKey( new Keyframe( 0.5f, 0.0f, -5.0f, -5.0f ) );
            animationCurve.AddKey( new Keyframe( 0.75f, -1.0f, 0.0f, 0.0f ) );
            animationCurve.AddKey( new Keyframe( 1.0f, 0.0f, 5.0f, 5.0f ) );
        }
        
        [ContextMenu("Set Button Scale")]
        void ButtonScale()
        {
            from            = value;
            to              = from * 1.1f;
            style           = Style.Once;
            method          = UIEaseType.none;
            duration        = 0.2f;
            delay           = 0.0f;
            tweenGroupType  = GroupType.Event;
            tweenGroup      = EventGroup.PointerClick.ToString( );
            
            animationCurve  = new AnimationCurve();
            animationCurve.AddKey( new Keyframe( 0.0f, 0.0f, 5.6f, 5.6f ) );
            animationCurve.AddKey( new Keyframe( 0.25f, 1.0f, 0.0f, 0.0f ) );
            animationCurve.AddKey( new Keyframe( 0.5f, 0.0f, -3.24f, -3.24f ) );
            animationCurve.AddKey( new Keyframe( 0.75f, -0.35f, 0.0f, 0.0f ) );
            animationCurve.AddKey( new Keyframe( 1.0f, 0.0f, 3.28f, 3.28f ) );
        }
        
        [ContextMenu("Set Button Min Scale")]
        void ButtonMinScale()
        {
            from            = value;
            to              = from * 0.9f;
            style           = Style.Once;
            method          = UIEaseType.easeOutSine;
            duration        = 0.1f;
            delay           = 0.0f;
            tweenGroupType  = GroupType.Event;
            tweenGroup      = EventGroup.PointerClick.ToString( );
        }
    }
}
