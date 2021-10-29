using UnityEngine;


namespace EG
{
    public class EventTrackMapCameraShake : EventTrack
    {

        [CustomFieldGroup("相机")]
        [CustomFieldAttribute("周波 X",CustomFieldAttribute.Type.Float)]
        public float FrequencyX = 10;
        
        [CustomFieldGroup("相机")]
        [CustomFieldAttribute("周波 Y",CustomFieldAttribute.Type.Float)]
        public float FrequencyY = 10;
        
        [CustomFieldGroup("相机")]
        [CustomFieldAttribute("振幅 X",CustomFieldAttribute.Type.Float)]
        public float AmplitudeX = 1;
        
        [CustomFieldGroup("相机")]
        [CustomFieldAttribute("振幅 Y",CustomFieldAttribute.Type.Float)]
        public float AmplitudeY = 1;
        
        static public bool s_IsStopAutoPlay = false;


        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        
        public override void OnStart( AppMonoBehaviour behaviour )
        {
            if( s_IsStopAutoPlay )
                return;
            
            Camera mainCamera = Camera.main;
            
            if( mainCamera != null )
            {
                CameraShakeEffect effect = mainCamera.gameObject.AddComponent<CameraShakeEffect>();
                effect.Duration = End - Start;
                effect.FrequencyX = FrequencyX;
                effect.FrequencyY = FrequencyY;
                effect.AmplitudeX = AmplitudeX;
                effect.AmplitudeY = AmplitudeY;
            }
        }
        
        public override void OnUpdate( AppMonoBehaviour behaviour, float time )
        {
        }
        
        public override void OnEnd( AppMonoBehaviour behaviour )
        {
        }
        

        #if UNITY_EDITOR
        
        public static string        ClassName  { get { return "Camera shake (for map)";            } }
        public override Color       TrackColor { get { return new Color32( 0x80, 255, 0x80, 0xff );  } }
        
        #endif
    }
}
