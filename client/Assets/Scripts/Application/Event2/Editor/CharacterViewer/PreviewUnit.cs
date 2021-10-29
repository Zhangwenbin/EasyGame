using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using EG;

public class PreviewUnit
        {
            const float CAMERA_INTERP_RATE = 1.0f / 0.25f;


            GameObject m_BodyObject = null;

            AnimationClip m_AnmClip = null;
            EditorAnimationPlayer m_AnmPlayer = null;
            EventPlayer m_EvPlayer = null;
            int m_AnmIdx = 0;
            string m_CurrentKey = "";
            

            bool m_IsAnmPlay = false;
            // 
            Camera m_DirectionCamera = null;
            int m_CameraId = -1;
            private GUIStyle s_BoldStyle;

            public GameObject BodyObj
            {
                get { return m_BodyObject; }
            }
            

            public void Setup(GameObject go)
            {
                DeleteObject();
                m_BodyObject = GameObject.Instantiate(go);
                m_BodyObject.hideFlags = HideFlags.DontSave;
                
                    m_AnmPlayer = m_BodyObject.AddComponent<EditorAnimationPlayer>();

                    var animator = m_BodyObject.GetComponent<Animator>();
                    if (animator != null) {
                        m_AnmPlayer.CreateAnimation(animator, 3);
                    }
                    
                m_IsAnmPlay = false;
            }
            

            public void SetupAnmEvent(EventParam evParam)
            {
                if (evParam != null && m_EvPlayer != null) {
                    m_EvPlayer.AddEvent(evParam.name, evParam);
                    m_EvPlayer.PlayEvent(evParam.name);
                }
            }


            public void Update(float deltaTime)
            {

            }


            public void Release()
            {
                DeleteObject();
            }


            public void PlayHead()
            {
                m_IsAnmPlay = true;
                if (m_AnmPlayer != null) {
                    m_AnmPlayer.SetPlayableTime(0);
                }
            }


            public void DrawGUIAnmController(ref float spdRate)
            {
                if( s_BoldStyle == null )
                {
                    s_BoldStyle = new GUIStyle( GUI.skin.label );
                    s_BoldStyle.normal.textColor = GUI.skin.label.normal.textColor;
                    s_BoldStyle.fontStyle = FontStyle.Bold;
                }
                GUILayout.BeginVertical("box");
                {
                    GUILayout.BeginHorizontal();
                    {

                            Color clr = Color.green;
                            string btnTxt = "再生";
                            if (m_IsAnmPlay) {
                                btnTxt = "停止";
                                clr = Color.red;
                            

                            GUI.backgroundColor = clr;
                            if (GUILayout.Button(btnTxt, GUILayout.Width(80))) {
                                m_IsAnmPlay = !m_IsAnmPlay;
                            }
                            GUI.backgroundColor = Color.white;

                            if (GUILayout.Button("start", GUILayout.Width(80))) {
                                if (m_AnmPlayer != null) {
                                    m_AnmPlayer.SetPlayableTime(0);
                                }
                            }
                        }

                        GUILayout.Label("speed :", GUILayout.Width(60));
                        spdRate = EditorGUILayout.Slider(spdRate, 0.1f, 3.0f);
                    }
                    GUILayout.EndHorizontal();
                    
                        GUILayout.Space(5);

                        GUI.enabled = m_AnmClip != null;
                        {
                            if (m_AnmClip != null && m_AnmPlayer != null) {
                                float length = m_AnmClip.length;
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label("clip", s_BoldStyle, GUILayout.Width(50));
                                    GUILayout.Label(m_AnmClip.name, GUILayout.Width(200));
                                    GUILayout.Label("[ length: " + length.ToString("F2") + "]", GUILayout.Width(80));

                                    GUI.contentColor = Color.cyan;
                                    GUILayout.Label("loop: " + ((m_AnmClip.isLooping) ? "あり" : "なし"), GUILayout.Width(90));
                                    GUI.contentColor = Color.white;
                                }
                                GUILayout.EndHorizontal();

                                float elapse = m_AnmPlayer.GetElapseTime(m_CurrentKey);
                                float slideVal = EditorGUILayout.Slider(elapse, 0f, length);
                                if (elapse != slideVal) {
                                    m_IsAnmPlay = false;
                                    m_AnmPlayer.SetPlayableTime(slideVal);
                                }
                            } else {
                                GUILayout.Label("clip : none");
                                EditorGUILayout.Slider(0, 0f, 1f);
                            }
                        }
                        GUI.enabled = true;
                    }
                GUILayout.EndVertical();
            }
            

            public EditorWindow_Event DrawGUIEventList()
            {
                if (m_EvPlayer == null)
                    return null;

                var dict = m_EvPlayer.LoadedEvents;
                if (dict.Count == 0)
                    return null;

                EditorWindow_Event window = null;

                foreach (var pair in dict) {
                    if (pair.Value != null) {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(pair.Value.name, GUILayout.Width(120));
                            if (GUILayout.Button("Open in editor", GUILayout.Width(90))) {
                                window = EditorWindow_Event.Init(pair.Value);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                return window;
            }

            public void AddEvent(string key, EventParam evParam)
            {
                if (m_EvPlayer != null) {
                    m_EvPlayer.AddEvent(key, evParam);
                }
            }

            public void PlayEvent(string key)
            {
                if (m_EvPlayer != null) {
                    m_EvPlayer.PlayEvent(key);
                }
            }


            public bool IsEventPlaying(string key)
            {
                if (m_EvPlayer != null) {
                    return m_EvPlayer.IsEventPlaying(key);
                }
                return false;
            }

            public bool HaveEvent(string key)
            {
                if (m_EvPlayer != null) {
                    return m_EvPlayer.FindEvent(key) != null;
                }

                return false;
            }

            public void SetCancelLoop()
            {
                if (m_EvPlayer != null) {
                    m_EvPlayer.SetOnSkillSeq();

                #if UNITY_EDITOR
                    if (m_EvPlayer.LoadedEvents.Count == 1) {
                        m_EvPlayer.StopEvent();
                        m_EvPlayer.AnimationPlayer.StopAll();
                    }
                #endif
                }
            }

            public void RemoveEvent(string key)
            {
                if (m_EvPlayer != null) {
                    m_EvPlayer.RemoveEvent(key);
                }
            }

            public void DeleteObject()
            {

                m_AnmClip = null;
                
                if (m_AnmPlayer != null) {
                    GameObject.DestroyImmediate(m_AnmPlayer);
                    m_AnmPlayer = null;
                }

                if (m_EvPlayer != null) {
                    GameObject.DestroyImmediate(m_EvPlayer);
                    m_EvPlayer = null;
                }

                if (m_BodyObject != null) {
                    GameObject.DestroyImmediate(m_BodyObject);
                    m_BodyObject = null;
                }
            }

        }



