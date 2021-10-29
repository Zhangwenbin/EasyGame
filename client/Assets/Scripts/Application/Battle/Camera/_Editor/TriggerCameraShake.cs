using UnityEngine;

namespace EG
{
    
    public class TriggerCameraShake : MonoBehaviour
    {
        public float Duration   = 0.3f;
        public float FrequencyX = 10;
        public float FrequencyY = 10;
        public float AmplitudeX = 1;
        public float AmplitudeY = 1;
        
        public bool IsRepeat    = false;
        
        static public bool s_IsStopAutoPlay = false;
        
        void OnEnable()
        {
            if( s_IsStopAutoPlay )
                return;
            
            _Shake();
            
            if( IsRepeat == false )
            {
                Destroy( this );
            }
        }
        
        void _Shake()
        {
            Camera mainCamera = Camera.main;
            
            if( mainCamera != null )
            {
                CameraShakeEffect effect = mainCamera.gameObject.AddComponent<CameraShakeEffect>();
                effect.Duration = Duration;
                effect.FrequencyX = FrequencyX;
                effect.FrequencyY = FrequencyY;
                effect.AmplitudeX = AmplitudeX;
                effect.AmplitudeY = AmplitudeY;
            }
        }
    }

}
