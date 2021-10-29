using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace EG
{
    [CustomEditor(typeof(UIRawImageTransparent), true)]
    public class EditorInspector_UIRawImageTransparent: UnityEditor.UI.RawImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(10);
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Preview"));
            serializedObject.ApplyModifiedProperties();
        }
    }
    
    [CustomEditor(typeof(UIPolyImage), true)]
    public class EditorInspector_PolyImage : UnityEditor.UI.ImageEditor
    {
        const int VERTEX_START = 1000;

        int mSelection = -1;
        static bool mAutoSnap;
        static bool mMerge;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        Vector2 CalcPositionInRect(Vector2 normalizedPos, Rect rect)
        {
            Vector2 p = new Vector2(
                        Mathf.Lerp(rect.xMin, rect.xMax, normalizedPos.x),
                        Mathf.Lerp(rect.yMin, rect.yMax, normalizedPos.y));

            return p;
        }

        void MoveVertex(SerializedProperty quadArray, int quadIndex, int vertex, RectTransform transform)
        {
            SerializedProperty quad = quadArray.GetArrayElementAtIndex(quadIndex);
            SerializedProperty pos = quad.FindPropertyRelative("v" + vertex);
            Rect rect = transform.rect;

            Vector2 normalizedPosOld = pos.vector2Value;
            Vector2 localPos = CalcPositionInRect(pos.vector2Value, rect);

            Vector3 worldPos = transform.TransformPoint(localPos);

            worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            
            if (mAutoSnap)
            {
                Camera cam = Camera.current;
                Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
                screenPos.z = 0;

                for (int i = 0; i < quadArray.arraySize; ++i)
                {
                    SerializedProperty other = quadArray.GetArrayElementAtIndex(i);
                    if (i != quadIndex)
                    {
                        for (int j = 0; j < 4; ++j)
                        {
                            SerializedProperty v = other.FindPropertyRelative("v" + j);

                            if (mMerge && v.vector2Value == normalizedPosOld)
                            {
                                continue;
                            }

                            Vector2 lp = CalcPositionInRect(v.vector2Value, rect);
                            Vector3 wp = transform.TransformPoint(lp);
                            Vector3 sp = cam.WorldToScreenPoint(wp);

                            sp.z = 0;

                            if ((screenPos - sp).magnitude <= 5.0f)
                            {
                                worldPos = wp;
                                break;
                            }
                        }
                    }
                }
            }

            localPos = transform.InverseTransformPoint(worldPos);
            
            Vector2 normalizedPos = new Vector2(
                (localPos.x - rect.xMin) / rect.width,
                (localPos.y - rect.yMin) / rect.height);

            pos.vector2Value = normalizedPos;
            
            if (mMerge)
            {
                Vector2 delta = normalizedPos - normalizedPosOld;

                ForEachVerticesAt(quadArray, normalizedPosOld, (v, c) =>
                {
                    v.vector2Value += delta;
                });
            }

        }

        delegate void VertexOp(SerializedProperty vertex, SerializedProperty color);

        void ForEachVerticesAt(SerializedProperty quadArray, Vector2 at, VertexOp op)
        {
            for (int i = 0; i < quadArray.arraySize; ++i)
            {
                SerializedProperty quad = quadArray.GetArrayElementAtIndex(i);

                for (int j = 0; j < 4; ++j)
                {
                    SerializedProperty v = quad.FindPropertyRelative("v" + j);
                    if (v.vector2Value == at)
                    {
                        SerializedProperty c = quad.FindPropertyRelative("c" + j);
                        op(v, c);
                    }
                }

            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Transparent"));

            SerializedProperty quadArray = serializedObject.FindProperty("Quads");
            if (!quadArray.hasMultipleDifferentValues)
            {
                //RectTransform polyTransform = (serializedObject.targetObjects[0] as Component).transform as RectTransform;
                //Rect rect = polyTransform.rect;

                GUILayout.Space(10);
                GUILayout.BeginVertical("Selection", GUI.skin.window, GUILayout.Height(60));

                if (0 <= mSelection && mSelection < quadArray.arraySize * 4)
                {
                    SerializedProperty propQuad = quadArray.GetArrayElementAtIndex(mSelection / 4);
                    //int quadIndex = mSelection / 4;
                    int index = mSelection % 4;
                    
                    SerializedProperty vertex = propQuad.FindPropertyRelative("v" + index);

                    Vector2 posOld = vertex.vector2Value;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(vertex, true);
                    
                    if (EditorGUI.EndChangeCheck() && mMerge)
                    {
                        ForEachVerticesAt(quadArray, posOld, (v, c) =>
                        {
                            v.vector2Value = vertex.vector2Value;
                        });
                    }
                    
                    SerializedProperty color = propQuad.FindPropertyRelative("c" + index);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(color, true);
                    
                    if (EditorGUI.EndChangeCheck() && mMerge)
                    {
                        ForEachVerticesAt(quadArray, posOld, (v, c) =>
                        {
                            c.colorValue = color.colorValue;
                        });
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.Vector2Field("v0", Vector2.zero);
                    EditorGUILayout.ColorField("c0", Color.black);
                    GUI.enabled = true;
                }

                GUILayout.EndVertical();
                GUILayout.Space(10);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            SerializedProperty propQuads = serializedObject.FindProperty("Quads");
            if (propQuads.hasMultipleDifferentValues)
            {
                return;
            }

            //Vector3[] quad = new Vector3[4];

            serializedObject.Update();

            //SerializedProperty positionProperty = null;
            //SerializedProperty colorProperty = null;

            int lastVertexID = -1;

            RectTransform polyTransform = (serializedObject.targetObjects[0] as Component).GetComponent<RectTransform>();

            Rect rect = polyTransform.rect;

            for (int i = 0; i < propQuads.arraySize; ++i)
            {
                SerializedProperty propQuad = propQuads.GetArrayElementAtIndex(i);
                SerializedProperty qv0 = propQuad.FindPropertyRelative("v0");
                SerializedProperty qv1 = propQuad.FindPropertyRelative("v1");
                SerializedProperty qv2 = propQuad.FindPropertyRelative("v2");
                SerializedProperty qv3 = propQuad.FindPropertyRelative("v3");

                Vector2 lv0 = CalcPositionInRect(qv0.vector2Value, rect);
                Vector2 lv1 = CalcPositionInRect(qv1.vector2Value, rect);
                Vector2 lv2 = CalcPositionInRect(qv2.vector2Value, rect);
                Vector2 lv3 = CalcPositionInRect(qv3.vector2Value, rect);

                Vector3 v0 = polyTransform.TransformPoint(lv0);
                Vector3 v1 = polyTransform.TransformPoint(lv1);
                Vector3 v2 = polyTransform.TransformPoint(lv2);
                Vector3 v3 = polyTransform.TransformPoint(lv3);

                bool isActiveQuad = (i * 4 <= mSelection && mSelection < i * 4 + 4);

                if (isActiveQuad)
                {
                    Handles.color = Color.yellow;
                }
                else
                {
                    Handles.color = Color.white;
                }

                Handles.DrawLine(v0, v1);
                Handles.DrawLine(v1, v2);
                Handles.DrawLine(v2, v3);
                Handles.DrawLine(v3, v0);

                HandleUtility.AddControl(VERTEX_START + i * 4, HandleUtility.DistanceToCircle(v0, 0.1f));
                HandleUtility.AddControl(VERTEX_START + i * 4 + 1, HandleUtility.DistanceToCircle(v1, 0.1f));
                HandleUtility.AddControl(VERTEX_START + i * 4 + 2, HandleUtility.DistanceToCircle(v2, 0.1f));
                HandleUtility.AddControl(VERTEX_START + i * 4 + 3, HandleUtility.DistanceToCircle(v3, 0.1f));

                Handles.RectangleHandleCap(VERTEX_START + i * 4, v0, Quaternion.identity, 1,EventType.Repaint);
                Handles.RectangleHandleCap(VERTEX_START + i * 4 + 1, v1, Quaternion.identity, 1,EventType.Repaint);
                Handles.RectangleHandleCap(VERTEX_START + i * 4 + 2, v2, Quaternion.identity, 1,EventType.Repaint);
                Handles.RectangleHandleCap(VERTEX_START + i * 4 + 3, v3, Quaternion.identity, 1,EventType.Repaint);

                if (isActiveQuad)
                {
                    //int index = mSelection % 4;
                    //positionProperty = propQuad.FindPropertyRelative("v" + index);
                    //colorProperty = propQuad.FindPropertyRelative("c" + index);
                    MoveVertex(propQuads, mSelection / 4, mSelection % 4, polyTransform);
                }

                lastVertexID = VERTEX_START + i * 4 + 3;
            }

            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(10, 10, 143, 100), "PolyImage", GUI.skin.window);
                {
                    float halfWidth = 65;

                    GUILayout.BeginVertical();
                    {

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("追加", GUILayout.Width(halfWidth)))
                            {
                                propQuads.InsertArrayElementAtIndex(propQuads.arraySize);

                                SerializedProperty newQuad = propQuads.GetArrayElementAtIndex(propQuads.arraySize - 1);

                                SerializedProperty v0 = newQuad.FindPropertyRelative("v0");
                                SerializedProperty c0 = newQuad.FindPropertyRelative("c0");
                                v0.vector2Value = Vector2.zero;
                                c0.colorValue = Color.white;

                                SerializedProperty v1 = newQuad.FindPropertyRelative("v1");
                                SerializedProperty c1 = newQuad.FindPropertyRelative("c1");
                                v1.vector2Value = new Vector2(0, 1);
                                c1.colorValue = Color.white;

                                SerializedProperty v2 = newQuad.FindPropertyRelative("v2");
                                SerializedProperty c2 = newQuad.FindPropertyRelative("c2");
                                v2.vector2Value = new Vector2(1, 1);
                                c2.colorValue = Color.white;

                                SerializedProperty v3 = newQuad.FindPropertyRelative("v3");
                                SerializedProperty c3 = newQuad.FindPropertyRelative("c3");
                                v3.vector2Value = new Vector2(1, 0);
                                c3.colorValue = Color.white;
                            }

                            GUI.enabled = (0 <= mSelection && mSelection <= lastVertexID - VERTEX_START);

                            if (GUILayout.Button("Delete", GUILayout.Width(halfWidth)))
                            {
                                propQuads.DeleteArrayElementAtIndex(mSelection / 4);
                                mSelection = (mSelection / 4 - 1) * 4;
                            }

                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("选择前面", GUILayout.Width(halfWidth)))
                            {
                                mSelection = Mathf.Clamp(mSelection - 1, 0, lastVertexID - VERTEX_START);
                            }

                            if (GUILayout.Button("选择下一个", GUILayout.Width(halfWidth)))
                            {
                                mSelection = Mathf.Clamp(mSelection + 1, 0, lastVertexID - VERTEX_START);
                            }
                        }
                        GUILayout.EndHorizontal();

                        mAutoSnap = GUILayout.Toggle(mAutoSnap, "顶点的抓拍");
                        mMerge = GUILayout.Toggle(mMerge, "总结编辑");

                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();

            if (Event.current.type == EventType.MouseDown)
            {
                int nearestControl = HandleUtility.nearestControl;

                if (VERTEX_START <= nearestControl && nearestControl <= lastVertexID)
                {
                    mSelection = nearestControl - VERTEX_START;
                    Event.current.Use();
                    Repaint();
                }
                else
                {
                    mSelection = -1;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

    }
    
    [CustomEditor(typeof(UIRawPolyImage), true)]
    public class EditorInspector_RawPolyImage : UnityEditor.UI.RawImageEditor
    {
        const int VERTEX_START = 1000;

        int mSelection = -1;
        static bool mAutoSnap;
        static bool mMerge;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        Vector2 CalcPositionInRect(Vector2 normalizedPos, Rect rect)
        {
            Vector2 p = new Vector2(
                        Mathf.Lerp(rect.xMin, rect.xMax, normalizedPos.x),
                        Mathf.Lerp(rect.yMin, rect.yMax, normalizedPos.y));

            return p;
        }

        void MoveVertex(SerializedProperty quadArray, int quadIndex, int vertex, RectTransform transform)
        {
            SerializedProperty quad = quadArray.GetArrayElementAtIndex(quadIndex);
            SerializedProperty pos = quad.FindPropertyRelative("v" + vertex);
            Rect rect = transform.rect;

            Vector2 normalizedPosOld = pos.vector2Value;
            Vector2 localPos = CalcPositionInRect(pos.vector2Value, rect);

            Vector3 worldPos = transform.TransformPoint(localPos);

            worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            
            if (mAutoSnap)
            {
                Camera cam = Camera.current;
                Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
                screenPos.z = 0;

                for (int i = 0; i < quadArray.arraySize; ++i)
                {
                    SerializedProperty other = quadArray.GetArrayElementAtIndex(i);
                    if (i != quadIndex)
                    {
                        for (int j = 0; j < 4; ++j)
                        {
                            SerializedProperty v = other.FindPropertyRelative("v" + j);

                            if (mMerge && v.vector2Value == normalizedPosOld)
                            {
                                continue;
                            }

                            Vector2 lp = CalcPositionInRect(v.vector2Value, rect);
                            Vector3 wp = transform.TransformPoint(lp);
                            Vector3 sp = cam.WorldToScreenPoint(wp);

                            sp.z = 0;

                            if ((screenPos - sp).magnitude <= 5.0f)
                            {
                                worldPos = wp;
                                break;
                            }
                        }
                    }
                }
            }

            localPos = transform.InverseTransformPoint(worldPos);
            
            Vector2 normalizedPos = new Vector2(
                (localPos.x - rect.xMin) / rect.width,
                (localPos.y - rect.yMin) / rect.height);

            pos.vector2Value = normalizedPos;
            
            if (mMerge)
            {
                Vector2 delta = normalizedPos - normalizedPosOld;

                ForEachVerticesAt(quadArray, normalizedPosOld, (v, c) =>
                    {
                        v.vector2Value += delta;
                    });
            }

        }

        delegate void VertexOp(SerializedProperty vertex, SerializedProperty color);

        void ForEachVerticesAt(SerializedProperty quadArray, Vector2 at, VertexOp op)
        {
            for (int i = 0; i < quadArray.arraySize; ++i)
            {
                SerializedProperty quad = quadArray.GetArrayElementAtIndex(i);

                for (int j = 0; j < 4; ++j)
                {
                    SerializedProperty v = quad.FindPropertyRelative("v" + j);
                    if (v.vector2Value == at)
                    {
                        SerializedProperty c = quad.FindPropertyRelative("c" + j);
                        op(v, c);
                    }
                }

            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Transparent"));

            SerializedProperty quadArray = serializedObject.FindProperty("Quads");
            if (!quadArray.hasMultipleDifferentValues)
            {
                //RectTransform polyTransform = (serializedObject.targetObjects[0] as Component).transform as RectTransform;
                //Rect rect = polyTransform.rect;

                GUILayout.Space(10);
                GUILayout.BeginVertical("Selection", GUI.skin.window, GUILayout.Height(60));

                if (0 <= mSelection && mSelection < quadArray.arraySize * 4)
                {
                    SerializedProperty propQuad = quadArray.GetArrayElementAtIndex(mSelection / 4);
                    //int quadIndex = mSelection / 4;
                    int index = mSelection % 4;
                    
                    SerializedProperty vertex = propQuad.FindPropertyRelative("v" + index);

                    Vector2 posOld = vertex.vector2Value;
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(vertex, true);
                    
                    if (EditorGUI.EndChangeCheck() && mMerge)
                    {
                        ForEachVerticesAt(quadArray, posOld, (v, c) =>
                        {
                            v.vector2Value = vertex.vector2Value;
                        });
                    }
                    
                    SerializedProperty color = propQuad.FindPropertyRelative("c" + index);

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(color, true);
                    
                    if (EditorGUI.EndChangeCheck() && mMerge)
                    {
                        ForEachVerticesAt(quadArray, posOld, (v, c) =>
                            {
                                c.colorValue = color.colorValue;
                            });
                    }
                }
                else
                {
                    GUI.enabled = false;
                    EditorGUILayout.Vector2Field("v0", Vector2.zero);
                    EditorGUILayout.ColorField("c0", Color.black);
                    GUI.enabled = true;
                }

                GUILayout.EndVertical();
                GUILayout.Space(10);
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Preview"));
            }

            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            SerializedProperty propQuads = serializedObject.FindProperty("Quads");
            if (propQuads.hasMultipleDifferentValues)
            {
                return;
            }

            //Vector3[] quad = new Vector3[4];

            serializedObject.Update();

            //SerializedProperty positionProperty = null;
            //SerializedProperty colorProperty = null;

            int lastVertexID = -1;

            RectTransform polyTransform = (serializedObject.targetObjects[0] as Component).GetComponent<RectTransform>();

            Rect rect = polyTransform.rect;

            for (int i = 0; i < propQuads.arraySize; ++i)
            {
                SerializedProperty propQuad = propQuads.GetArrayElementAtIndex(i);
                SerializedProperty qv0 = propQuad.FindPropertyRelative("v0");
                SerializedProperty qv1 = propQuad.FindPropertyRelative("v1");
                SerializedProperty qv2 = propQuad.FindPropertyRelative("v2");
                SerializedProperty qv3 = propQuad.FindPropertyRelative("v3");

                Vector2 lv0 = CalcPositionInRect(qv0.vector2Value, rect);
                Vector2 lv1 = CalcPositionInRect(qv1.vector2Value, rect);
                Vector2 lv2 = CalcPositionInRect(qv2.vector2Value, rect);
                Vector2 lv3 = CalcPositionInRect(qv3.vector2Value, rect);

                Vector3 v0 = polyTransform.TransformPoint(lv0);
                Vector3 v1 = polyTransform.TransformPoint(lv1);
                Vector3 v2 = polyTransform.TransformPoint(lv2);
                Vector3 v3 = polyTransform.TransformPoint(lv3);

                bool isActiveQuad = (i * 4 <= mSelection && mSelection < i * 4 + 4);

                if (isActiveQuad)
                {
                    Handles.color = Color.yellow;
                }
                else
                {
                    Handles.color = Color.white;
                }

                Handles.DrawLine(v0, v1);
                Handles.DrawLine(v1, v2);
                Handles.DrawLine(v2, v3);
                Handles.DrawLine(v3, v0);

                HandleUtility.AddControl(VERTEX_START + i * 4, HandleUtility.DistanceToCircle(v0, 0.1f));
                HandleUtility.AddControl(VERTEX_START + i * 4 + 1, HandleUtility.DistanceToCircle(v1, 0.1f));
                HandleUtility.AddControl(VERTEX_START + i * 4 + 2, HandleUtility.DistanceToCircle(v2, 0.1f));
                HandleUtility.AddControl(VERTEX_START + i * 4 + 3, HandleUtility.DistanceToCircle(v3, 0.1f));

                Handles.RectangleHandleCap(VERTEX_START + i * 4, v0, Quaternion.identity, 1,EventType.Repaint);
                Handles.RectangleHandleCap(VERTEX_START + i * 4 + 1, v1, Quaternion.identity, 1,EventType.Repaint);
                Handles.RectangleHandleCap(VERTEX_START + i * 4 + 2, v2, Quaternion.identity, 1,EventType.Repaint);
                Handles.RectangleHandleCap(VERTEX_START + i * 4 + 3, v3, Quaternion.identity, 1,EventType.Repaint);

                if (isActiveQuad)
                {
                    //int index = mSelection % 4;
                    //positionProperty = propQuad.FindPropertyRelative("v" + index);
                    //colorProperty = propQuad.FindPropertyRelative("c" + index);
                    MoveVertex(propQuads, mSelection / 4, mSelection % 4, polyTransform);
                }

                lastVertexID = VERTEX_START + i * 4 + 3;
            }

            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(10, 10, 143, 100), "PolyImage", GUI.skin.window);
                {
                    float halfWidth = 65;

                    GUILayout.BeginVertical();
                    {

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("追加", GUILayout.Width(halfWidth)))
                            {
                                propQuads.InsertArrayElementAtIndex(propQuads.arraySize);

                                SerializedProperty newQuad = propQuads.GetArrayElementAtIndex(propQuads.arraySize - 1);

                                SerializedProperty v0 = newQuad.FindPropertyRelative("v0");
                                SerializedProperty c0 = newQuad.FindPropertyRelative("c0");
                                v0.vector2Value = Vector2.zero;
                                c0.colorValue = Color.white;

                                SerializedProperty v1 = newQuad.FindPropertyRelative("v1");
                                SerializedProperty c1 = newQuad.FindPropertyRelative("c1");
                                v1.vector2Value = new Vector2(0, 1);
                                c1.colorValue = Color.white;

                                SerializedProperty v2 = newQuad.FindPropertyRelative("v2");
                                SerializedProperty c2 = newQuad.FindPropertyRelative("c2");
                                v2.vector2Value = new Vector2(1, 1);
                                c2.colorValue = Color.white;

                                SerializedProperty v3 = newQuad.FindPropertyRelative("v3");
                                SerializedProperty c3 = newQuad.FindPropertyRelative("c3");
                                v3.vector2Value = new Vector2(1, 0);
                                c3.colorValue = Color.white;
                            }

                            GUI.enabled = (0 <= mSelection && mSelection <= lastVertexID - VERTEX_START);

                            if (GUILayout.Button("Delete", GUILayout.Width(halfWidth)))
                            {
                                propQuads.DeleteArrayElementAtIndex(mSelection / 4);
                                mSelection = (mSelection / 4 - 1) * 4;
                            }

                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("选择前面", GUILayout.Width(halfWidth)))
                            {
                                mSelection = Mathf.Clamp(mSelection - 1, 0, lastVertexID - VERTEX_START);
                            }

                            if (GUILayout.Button("选择后面", GUILayout.Width(halfWidth)))
                            {
                                mSelection = Mathf.Clamp(mSelection + 1, 0, lastVertexID - VERTEX_START);
                            }
                        }
                        GUILayout.EndHorizontal();

                        mAutoSnap = GUILayout.Toggle(mAutoSnap, "顶点snap");
                        mMerge = GUILayout.Toggle(mMerge, "merge");

                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();

            if (Event.current.type == EventType.MouseDown)
            {
                int nearestControl = HandleUtility.nearestControl;

                if (VERTEX_START <= nearestControl && nearestControl <= lastVertexID)
                {
                    mSelection = nearestControl - VERTEX_START;
                    Event.current.Use();
                    Repaint();
                }
                else
                {
                    mSelection = -1;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        
    }
}
