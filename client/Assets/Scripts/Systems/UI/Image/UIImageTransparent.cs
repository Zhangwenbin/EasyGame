using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Image (透明)")]
    public class UIImageTransparent : Image
    {
        protected override void OnPopulateMesh( VertexHelper toFill )
        {
            if( sprite != null )
            {
                base.OnPopulateMesh( toFill );
            }
            else
            {
                toFill.Clear( );
            }
        }
    }
}
