using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    public class UIEmptyClicker : MaskableGraphic, IPointerClickHandler
    {
        public event Action OnClickEvent;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent?.Invoke();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}