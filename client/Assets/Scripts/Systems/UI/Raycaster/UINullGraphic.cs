using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EG
{
    
    [AddComponentMenu("Scripts/System/UI/Raycaster/UINullGraphic")]
    [ExecuteInEditMode]
    public class UINullGraphic : Graphic
    {
    #if !UNITY_4_6
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    #else
        protected override void OnFillVBO(List<UIVertex> vbo)
        {
        }
    #endif
    }
}
