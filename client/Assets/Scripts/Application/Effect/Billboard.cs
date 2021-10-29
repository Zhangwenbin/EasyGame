using UnityEngine;

namespace EG
{
    [AddComponentMenu("Rendering/Billboard")]
    public class Billboard : MonoBehaviour 
    {

        void OnEnable()
        {
            CameraHook.AddPreCullEventListener(PreCull);
        }

        void OnDisable()
        {
            CameraHook.RemovePreCullEventListener(PreCull);
        }

        void PreCull(Camera camera)
        {
            Transform tr = transform;
            Transform cameraTransform = camera.transform;
            tr.rotation = Quaternion.LookRotation(cameraTransform.forward, cameraTransform.up);
        }

    }
}
