using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackPlaySE : EventTrack
    {
        public enum ELoadType
        {
            None,
            lb,
        }

        [FormerlySerializedAs("Type")]
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("AudioLayer", CustomFieldAttribute.Type.Enum, typeof(EAudioLayer))]
        public EAudioLayer layer;

        [CustomFieldGroup("设置")] [CustomFieldAttribute("Key", CustomFieldAttribute.Type.String)]
        public string Key;

        [CustomFieldGroup("设置")] [CustomFieldAttribute("音量", CustomFieldAttribute.Type.Float)]
        public float Volume = 1.0f;

        [CustomFieldGroup("设置")] [CustomFieldAttribute("Pitch", CustomFieldAttribute.Type.Float)]
        public float Pitch = 0f;

        [CustomFieldGroup("设置")] [CustomFieldAttribute("Loop", CustomFieldAttribute.Type.Bool)]
        public bool Loop = false;

        [CustomFieldGroup("设置")] [CustomFieldAttribute("StopSec", CustomFieldAttribute.Type.Float)]
        public float StopSec;

        [CustomFieldGroup("加载")] [CustomFieldAttribute("LoadType", CustomFieldAttribute.Type.Enum, typeof(ELoadType))]
        public ELoadType LoadType = ELoadType.None;

        string m_SheetName = "";

        static public bool s_IsStopPlay = false;


        public override ECategory Category
        {
            get { return ECategory.Common; }
        }

        private EventInstance _instance;

        public override void OnStart(AppMonoBehaviour behaviour)
        {
            if (behaviour.gameObject.activeInHierarchy == false)
                return;

            if (string.IsNullOrEmpty(Key))
            {
                return;
            }

            if (s_IsStopPlay)
                return;

            _instance = FmodManager.Instance.PlayInLayer(layer, Key, Loop);
            _instance.setVolume(Volume);
            _instance.setPitch(Pitch);
        }

        public override void OnEnd(AppMonoBehaviour behaviour)
        {
            ReleaseSEPlayer(StopSec);
        }

        void ReleaseSEPlayer(float stopSec)
        {
            if (_instance.isValid())
            {
                _instance.stop(stopSec > 0 ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            }
        }


        public override bool CheckPreLoad()
        {
            return LoadType != ELoadType.None;
        }

        public override bool IsDonePreLoad()
        {
            if (string.IsNullOrEmpty(m_SheetName))
                return true;

            return true;
        }

        public override void StartPreLoad()
        {
            // 

        }

        public override void UnloadPreLoad()
        {

        }

#if UNITY_EDITOR

        public override void EditorPreProcess(AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded)
        {

        }

        public override void EditorStartPreLoad()
        {
            // 

        }

        public override void EditorUnloadPreLoad()
        {

        }

        public static string ClassName
        {
            get { return "播放音效"; }
        }

        public override Color TrackColor
        {
            get { return Color.yellow; }
        }

        public override void OnInspectorGUI(Rect position, SerializedObject serializeObject, float width)
        {
            CustomFieldAttribute.OnInspectorGUI(position, this.GetType(), serializeObject, width);

            if (CheckPreLoad() == false)
                return;

            GUILayout.BeginVertical("box");
            {
                EditorEventRef editorEvent = EventManager.EventFromPath(Key);
                var tmpSheetName = editorEvent.Banks.Select(x => x.Name).ToArray().First();
                if (string.IsNullOrEmpty(tmpSheetName))
                {
                    GUI.contentColor = Color.red;
                    GUILayout.Label("没有找到bank");
                    GUI.contentColor = Color.white;
                }
                else
                {
                    GUILayout.Label("bank >> " + tmpSheetName);
                }
            }
            GUILayout.EndVertical();
        }

        public override void EditorRelease()
        {
            ReleaseSEPlayer(0);

            // ----------------------------------------

            base.EditorRelease();
        }

#endif
    }
}
