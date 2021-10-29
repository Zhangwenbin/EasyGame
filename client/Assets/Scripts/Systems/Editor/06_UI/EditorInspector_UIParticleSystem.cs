using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace EG
{
    [UnityEditor.CustomEditor(typeof(UIParticleSystem))]
    class EditorInspector_UIParticleSystem : UnityEditor.Editor
    {
        readonly    GUIStyle LINE_STYLE = new GUIStyle();
        readonly    Color    LINE_COLOR = new Color(0, 0, 0, 0.35f);
        
        System.DateTime mLastUpdateTime;
        
        void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            
            mLastUpdateTime = System.DateTime.Now;
            
            if (!Application.isPlaying)
            {
                UIParticleSystem uiparticle = target as UIParticleSystem;
                uiparticle.PauseEmitters();
            }
        }
        
        void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            
            if( Application.isPlaying == false )
            {
                UIParticleSystem uiparticle = target as UIParticleSystem;
                if (uiparticle != null)
                {
                    uiparticle.ResetEmitters();
                }
            }
        }
        
        void OnUpdate()
        {
            System.DateTime currentTime = System.DateTime.Now;
            
            UIParticleSystem uiparticle = target as UIParticleSystem;
            if( uiparticle.IsPlaying && Application.isPlaying == false )
            {
                float dt = (float)(currentTime - mLastUpdateTime).TotalSeconds;
                
                UIParticleSystem[] particles = uiparticle.GetComponentsInChildren<UIParticleSystem>();
                for (int i = 0; i < particles.Length; ++i)
                {
                    particles[i].AdvanceTime(dt);
                }
                
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            
            mLastUpdateTime = currentTime;
        }
        
        void OnSceneGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            Rect viewRect = UnityEditor.SceneView.currentDrawingSceneView.position;
            
            UnityEditor.Handles.BeginGUI();
            
            const float     AREA_WIDTH              = 150f;
            const float     AREA_HEIGHT             = 90f;
            const float     AREA_RIGHT_MARGIN       = 10f;
            const float     AREA_BOTTOM_MARGIN      = 36f;  
            
            GUILayout.BeginArea(new Rect(viewRect.width - ( AREA_WIDTH + AREA_RIGHT_MARGIN ), viewRect.height - ( AREA_HEIGHT + AREA_BOTTOM_MARGIN ), AREA_WIDTH, AREA_HEIGHT ), "UIParticle", GUI.skin.window);
            {
                UIParticleSystem uiparticle = target as UIParticleSystem;
                
                string buttonName = uiparticle.PlaybackTime > 0.0f ? "Pause" : "Simulate";
                
                switch (GUILayout.Toolbar(-1, new string[] { buttonName, "Stop" }))
                {
                    case 0:
                        if (uiparticle.IsPlaying)
                        {
                            uiparticle.PauseEmitters();
                        }
                        else
                        {
                            uiparticle.ResumeEmitters();
                        }
                        break;
                        
                    case 1:
                        uiparticle.ResetEmitters();
                        break;
                }
                
                GUIStyle timeStyle = new GUIStyle(GUI.skin.label);
                timeStyle.alignment = TextAnchor.UpperRight;
                
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label( "Playback Time" );
                    GUILayout.Label(uiparticle.PlaybackTime.ToString("0.00"), timeStyle);
                }
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label( "Particles" );
                    GUILayout.Label( uiparticle.ParticleCount.ToString() + " / " + uiparticle.maxParticles.ToString(), timeStyle );
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
            UnityEditor.Handles.EndGUI();
        }

        public override void OnInspectorGUI()
        {
            UnityEditor.SerializedProperty itr;

            LINE_STYLE.normal.background = Texture2D.whiteTexture;

            serializedObject.Update();

            UnityEditor.SerializedProperty tex = serializedObject.FindProperty("SourceImage");
            if( tex != null )
            {
                UnityEditor.EditorGUILayout.PropertyField(tex);
            }
            
            UnityEditor.SerializedProperty mat = serializedObject.FindProperty("m_Material");
            UnityEditor.EditorGUILayout.PropertyField(mat);

            UnityEditor.SerializedProperty updateMode = serializedObject.FindProperty("updateMode");
            UnityEditor.EditorGUILayout.PropertyField(updateMode);
            
            // ----------------------------------------
            _DrawSeparateLine();
            // ----------------------------------------
            
            UnityEditor.SerializedProperty type = serializedObject.FindProperty("emitterType");
            UnityEditor.EditorGUILayout.PropertyField(type);

            if (!type.hasMultipleDifferentValues)
            {
                itr = null;

                switch ((UIParticleSystem.EmitterTypes)type.enumValueIndex)
                {
                    case UIParticleSystem.EmitterTypes.Sphere:
                        itr = serializedObject.FindProperty("sphereEmitter");
                        break;
                    case UIParticleSystem.EmitterTypes.Box:
                        itr = serializedObject.FindProperty("boxEmitter");
                        break;
                    case UIParticleSystem.EmitterTypes.Cone:
                        itr = serializedObject.FindProperty("coneEmitter");
                        break;
                }

                if (itr != null)
                {
                    string path = itr.propertyPath;

                    itr.NextVisible(true);
                    do
                    {
                        UnityEditor.EditorGUILayout.PropertyField(itr, true);
                    }
                    while (itr.NextVisible(false) && itr.propertyPath.StartsWith(path));
                }
            }

            UnityEditor.SerializedProperty renderMode = serializedObject.FindProperty("RenderMode");
            UnityEditor.EditorGUILayout.PropertyField(renderMode);

            UnityEditor.SerializedProperty simulateSpace = serializedObject.FindProperty( "SimulateSpace" );
            UnityEditor.EditorGUILayout.PropertyField( simulateSpace );
            
            if (!renderMode.hasMultipleDifferentValues)
            {
                itr = null;

                switch ((UIParticleSystem.ParticleRenderMode)renderMode.enumValueIndex)
                {
                    case UIParticleSystem.ParticleRenderMode.StretchBillboard:
                        itr = serializedObject.FindProperty("m_StretchBillboard");
                        break;
                }

                if (itr != null)
                {
                    string path = itr.propertyPath;

                    itr.NextVisible(true);
                    do
                    {
                        UnityEditor.EditorGUILayout.PropertyField(itr, true);
                    }
                    while (itr.NextVisible(false) && itr.propertyPath.StartsWith(path));
                }
            }

            // ----------------------------------------
            _DrawSeparateLine();
            // ----------------------------------------

            itr = serializedObject.GetIterator();

            // l.262 で追加されるtoggleと、Unity側で構造体のInspector展開用のボタンが重なるのでマージンを増やす
            float margin = 18;//8;
            UnityEditor.EditorGUIUtility.labelWidth -= margin;

            bool enterChildren = true;
            while (itr.NextVisible(enterChildren))
            {
                enterChildren = false;

                FieldInfo field = target.GetType().GetField(itr.name);

                if (field == null || field.GetCustomAttributes(typeof(UIParticleSystem.ParticleAttribute), true).Length <= 0)
                {
                    continue;
                }

                UnityEditor.SerializedProperty toggle = serializedObject.FindProperty(itr.name + "Enable");

                if (toggle != null && toggle.propertyType == UnityEditor.SerializedPropertyType.Boolean)
                {
                    GUI.enabled = toggle.boolValue;
                }
                else
                {
                    toggle = null;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(margin);
                UnityEditor.EditorGUILayout.PropertyField(itr, true);
                GUI.enabled = true;

                if (toggle != null && !toggle.hasMultipleDifferentValues)
                {
                    Rect rect = GUILayoutUtility.GetLastRect();

                    rect.x = 5;
                    rect.width = 16;
                    rect.height = 16;

                    toggle.boolValue = GUI.Toggle(rect, toggle.boolValue, "");
                }

                GUILayout.EndHorizontal();

            }

            serializedObject.ApplyModifiedProperties();
        }
        
        void _DrawSeparateLine()
        {
            GUI.backgroundColor = LINE_COLOR;
            GUILayout.Space( 4 );
            GUILayout.Box( "", LINE_STYLE, GUILayout.Height( 1 ) );
            GUILayout.Space( 4 );
            GUI.backgroundColor = Color.white;
        }
    }
    
    [UnityEditor.CustomPropertyDrawer( typeof( UIParticleSystem.ParticleBurst ) )]
    class ParticleBurstEditor : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, GUIContent label )
        {
            UnityEditor.SerializedProperty pointsArray = property.FindPropertyRelative( "Points" );
            
            return 32 + pointsArray.arraySize * 16;
        }
        
        public override void OnGUI( Rect position, UnityEditor.SerializedProperty property, GUIContent label )
        {
            GUI.Label( position, label );
            
            UnityEditor.SerializedProperty pointsArray = property.FindPropertyRelative( "Points" );
            
            if( pointsArray.hasMultipleDifferentValues )
            {
                return;
            }
            
            float buttonSize = 25;
            
            Rect col1 = position;
            col1.y += 16;
            col1.height = 16;
            col1.xMin = UnityEditor.EditorGUIUtility.labelWidth;
            col1.width = ( col1.width - buttonSize * 2 ) / 2;
            GUI.Label( col1, "Time" );
            
            Rect col2 = col1;
            col2.x += col1.width;
            GUI.Label( col2, "Count" );
            
            Rect rcButton = col2;
            rcButton.x = col2.xMax;
            rcButton.width = buttonSize;
            if( GUI.Button( rcButton, "+" ) )
            {
                pointsArray.InsertArrayElementAtIndex( pointsArray.arraySize );
            }
            
            bool guiIsEnabled = GUI.enabled;
            GUI.enabled = pointsArray.arraySize > 0;
            
            rcButton.x += rcButton.width;
            if( GUI.Button( rcButton, "-" ) )
            {
                pointsArray.DeleteArrayElementAtIndex( pointsArray.arraySize - 1 );
            }
            
            GUI.enabled = guiIsEnabled;
            
            for( int i = 0; i < pointsArray.arraySize; ++i )
            {
                UnityEditor.SerializedProperty point = pointsArray.GetArrayElementAtIndex( i );
                
                Rect rc;
                
                rc = col1;
                rc.y += ( i + 1 ) * 16;
                UnityEditor.SerializedProperty time = point.FindPropertyRelative( "Time" );
                UnityEditor.EditorGUI.PropertyField( rc, time, new GUIContent( "" ) );
                
                rc = col2;
                rc.y += ( i + 1 ) * 16;
                UnityEditor.SerializedProperty Count = point.FindPropertyRelative( "Count" );
                UnityEditor.EditorGUI.PropertyField( rc, Count, new GUIContent( "" ) );
            }
            
            GUI.enabled = true;
        }
    }
    
    [UnityEditor.CustomPropertyDrawer( typeof( UIParticleSystem.TextureSheetAnimation ) )]
    class TextureSheetAnimationEditor : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, GUIContent label )
        {
            return 150;
        }
        
        public override void OnGUI( Rect position, UnityEditor.SerializedProperty property, GUIContent label )
        {
            GUI.Label( position, label );
            
            string path = property.propertyPath + ".";
            property.NextVisible( true );
            
            Rect rc = position;
            rc.height = 16;
            
            do
            {
                rc.y += rc.height;
                UnityEditor.EditorGUI.PropertyField( rc, property );
            }
            while( property.NextVisible( false ) && property.propertyPath.StartsWith( path ) );
        }
    }
    
    [UnityEditor.CustomPropertyDrawer( typeof( UIParticleSystem.VelocityOverLifetime ) )]
    class VelocityOverLifetimeEditor : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, GUIContent label )
        {
            return 64;
        }
        
        public override void OnGUI( Rect position, UnityEditor.SerializedProperty property, GUIContent label )
        {
            GUI.Label( position, label );
            
            UnityEditor.SerializedProperty x = property.FindPropertyRelative( "X" );
            UnityEditor.SerializedProperty y = property.FindPropertyRelative( "Y" );
            
            Rect rc = position;
            rc.y += 16;
            rc.height = 16;
            
            rc.width /= 2;
            GUI.backgroundColor = new Color( 1.0f, 0.5f, 0.5f );
            UnityEditor.EditorGUI.PropertyField( rc, x, new GUIContent( "" ) );
            
            rc.x += rc.width;
            GUI.backgroundColor = new Color( 0.5f, 1.0f, 0.5f );
            UnityEditor.EditorGUI.PropertyField( rc, y, new GUIContent( "" ) );
            
            GUI.backgroundColor = Color.white;
            
            rc.x = position.x;
            rc.width = position.width;
            rc.y += rc.height;
            UnityEditor.EditorGUI.PropertyField( rc, property.FindPropertyRelative( "ScaleX" ) );
            
            rc.x = position.x;
            rc.width = position.width;
            rc.y += rc.height;
            UnityEditor.EditorGUI.PropertyField( rc, property.FindPropertyRelative( "ScaleY" ) );
        }
    }
    
    [UnityEditor.CustomPropertyDrawer( typeof( UIParticleSystem.LimitVelocityOverLifetime ) )]
    class LimitVelocityOverLifetimeEditor : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, GUIContent label )
        {
            return 64;
        }
        
        public override void OnGUI( Rect position, UnityEditor.SerializedProperty property, GUIContent label )
        {
            GUI.Label( position, label );
            
            Rect rc = position;
            rc.y += 16;
            rc.height = 16;
            
            GUI.backgroundColor = Color.white;
            
            rc.x = position.x;
            rc.width = position.width;
            rc.y += rc.height;
            UnityEditor.EditorGUI.PropertyField( rc, property.FindPropertyRelative( "Speed" ) );
            
            rc.x = position.x;
            rc.width = position.width;
            rc.y += rc.height;
            UnityEditor.EditorGUI.PropertyField( rc, property.FindPropertyRelative( "Dampen" ) );
        }
    }
    
    [UnityEditor.CustomPropertyDrawer( typeof( UIParticleSystem.FloatRange ) )]
    [UnityEditor.CustomPropertyDrawer( typeof( UIParticleSystem.ColorRange ) )]
    class FloatRangeEditor : UnityEditor.PropertyDrawer
    {
        public override float GetPropertyHeight( UnityEditor.SerializedProperty property, GUIContent label )
        {
            return 16;
        }
        
        public override void OnGUI( Rect position, UnityEditor.SerializedProperty property, GUIContent label )
        {
            GUI.Label( position, label );
            
            Rect rc = position;
            rc.xMin += UnityEditor.EditorGUIUtility.labelWidth;
            rc.width /= 2;
            
            UnityEditor.SerializedProperty min = property.FindPropertyRelative( "Min" );
            UnityEditor.EditorGUI.PropertyField( rc, min, new GUIContent( "" ) );
            
            rc.x += rc.width;
            UnityEditor.SerializedProperty max = property.FindPropertyRelative( "Max" );
            UnityEditor.EditorGUI.PropertyField( rc, max, new GUIContent( "" ) );
        }
    }
}
