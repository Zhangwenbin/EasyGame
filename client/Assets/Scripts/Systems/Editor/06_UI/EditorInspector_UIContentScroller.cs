using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace EG
{
    [CustomEditor(typeof(UIContentScroller), true)]
    public class EditorInspector_UIContentScroller : UnityEditor.UI.ScrollRectEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            CustomFieldAttribute.OnInspectorGUI( target.GetType( ), serializedObject );
        }
    }
}
