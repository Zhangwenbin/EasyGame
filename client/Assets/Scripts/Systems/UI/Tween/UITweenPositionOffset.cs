using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITweenPositionOffset")]
    public class UITweenPositionOffset : UITween<Vector3>
    {
        RectTransform mRectTransform;
        Vector3 mPos;
        
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
            get { return cachedRectTransform.anchoredPosition;}
            set { cachedRectTransform.anchoredPosition = value;}
        }
        
#if UNITY_EDITOR
        public override void OnInspectorGUI()
        {
            from = UnityEditor.EditorGUILayout.Vector3Field( "From", from, null );
            to = UnityEditor.EditorGUILayout.Vector3Field( "To", to, null );
        }
#endif
        
        private void Awake()
        {
            mPos = value;
        }
        
        protected override void Start()
        {
            base.Start ();
        }
        
        protected override void OnUpdate( float factor, bool isFinished )
        {
            value = mPos + ( from + factor * ( to - from ) );
        }
    }
}
