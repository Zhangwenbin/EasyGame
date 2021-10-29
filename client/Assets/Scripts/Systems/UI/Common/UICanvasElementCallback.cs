using System;
using UnityEngine;
using UnityEngine.UI;

namespace EG
{
    public class UICanvasElementCallback : MonoBehaviour, ICanvasElement
    {
        public Action OnGraphicUpdateComplete { get; set; }
        public Action OnLayoutComplete { get; set; }
        public Action<CanvasUpdate> OnRebuild { get; set; }

        public void SetDirty()
        {
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }

        public void GraphicUpdateComplete()
        {
            OnGraphicUpdateComplete?.Invoke();
        }

        public bool IsDestroyed()
        {
            return this == null;
        }

        public void LayoutComplete()
        {
            OnLayoutComplete?.Invoke();
        }

        public void Rebuild(CanvasUpdate executing)
        {
            OnRebuild?.Invoke(executing);
        }
    }
}
