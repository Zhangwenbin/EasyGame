using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/ImageArray")]
    public class UIImageArray : Image
    {
        [CustomFieldAttribute("ImageArray", CustomFieldAttribute.Type.UISprites)]
        public Sprite[] Images = new Sprite[0];
        
        private int mImageIndex = 0;        
        
        public int ImageIndex
        {
            get { return mImageIndex; }    
            set
            {
                if (0 <= value && value < Images.Length)
                {
                    sprite = Images[value];
                    mImageIndex = value;    
                }
                else
                {
                    Debug.LogError("index out");
                }
            }
        }
        
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (sprite == null)
            {
                toFill.Clear();
                return;
            }
            
            base.OnPopulateMesh(toFill);
        }
    }
}