using UnityEngine;

namespace EG
{

    public class CameraShakeEffect : MonoBehaviour
    {
        float mSeedX;
        float mSeedY;
        float mTime;
        
        public float Duration           = 0.3f;
        public float FrequencyX         = 10;
        public float FrequencyY         = 10;
        public float AmplitudeX         = 1;
        public float AmplitudeY         = 1;
        public bool  IsStrengthDecline  = false; // 

        void Awake()
        {
            mSeedX = Random.value;
            mSeedY = Random.value;
        }
        
        void Update()
        {
            mTime += Time.deltaTime;
            if (mTime >= Duration)
            {
                Destroy(this);
                return;
            }
        }
        
        void OnPreCull()
        {
            float t = Mathf.Clamp01(mTime / Duration);
            float strength = IsStrengthDecline ? t : 1.0f - t;
            
            float x = Mathf.Sin((Time.time + mSeedX) * FrequencyX * Mathf.PI) * AmplitudeX * strength;
            float y = Mathf.Sin((Time.time + mSeedY) * FrequencyY * Mathf.PI) * AmplitudeY * strength;
            
            Quaternion shake = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
            transform.rotation = transform.rotation * shake;
        }
    }
}
