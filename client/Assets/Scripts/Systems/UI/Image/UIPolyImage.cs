using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Poly Image")]
    public class UIPolyImage : Image
    {
        public Quad[] Quads = new Quad[0];
        public bool Transparent;
        
        RectTransform mRectTransform;
        
        protected override void Awake()
        {
            base.Awake();
            mRectTransform = GetComponent<RectTransform>();
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            
            if ((sprite == null && Transparent) || color.a <= 0)
            {
                return;
            }
            
            UIVertex vertex = new UIVertex();
            Color32 c = color;
            Rect rect = mRectTransform.rect;
            Rect uvRect;
            int n = 0;
            
            Sprite spr = sprite;
            if (spr != null)
            {
                float invW = 1.0f / spr.texture.width;
                float invH = 1.0f / spr.texture.height;
                Rect srcRect = spr.rect;
                
                uvRect = new Rect(
                    srcRect.x * invW,
                    srcRect.y * invH,
                    srcRect.width * invW,
                    srcRect.height * invH);
            }
            else
            {
                uvRect = new Rect(0, 0, 1, 1);
            }
            
            if (c.r == 255 && c.g == 255 && c.b == 255 && c.a == 255)
            {
                for (int i = Quads.Length - 1; i >= 0; --i)
                {
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v0.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v0.y);
                    vertex.color = Quads[i].c0;
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v0.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v0.y);
                    vh.AddVert(vertex);
                    
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v1.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v1.y);
                    vertex.color = Quads[i].c1;
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v1.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v1.y);
                    vh.AddVert(vertex);
                    
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v2.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v2.y);
                    vertex.color = Quads[i].c2;
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v2.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v2.y);
                    vh.AddVert(vertex);
                    
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v3.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v3.y);
                    vertex.color = Quads[i].c3;
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v3.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v3.y);
                    vh.AddVert(vertex);
                    
                    vh.AddTriangle(n + 0, n + 1, n + 2);
                    vh.AddTriangle(n + 2, n + 3, n + 0);
                    n += 4;
                }
            }
            else
            {
                for (int i = Quads.Length - 1; i >= 0; --i)
                {
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v0.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v0.y);
                    vertex.color.r = (byte)(Quads[i].c0.r * c.r / 255);
                    vertex.color.g = (byte)(Quads[i].c0.g * c.g / 255);
                    vertex.color.b = (byte)(Quads[i].c0.b * c.b / 255);
                    vertex.color.a = (byte)(Quads[i].c0.a * c.a / 255);
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v0.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v0.y);
                    vh.AddVert(vertex);
                    
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v1.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v1.y);
                    vertex.color.r = (byte)(Quads[i].c1.r * c.r / 255);
                    vertex.color.g = (byte)(Quads[i].c1.g * c.g / 255);
                    vertex.color.b = (byte)(Quads[i].c1.b * c.b / 255);
                    vertex.color.a = (byte)(Quads[i].c1.a * c.a / 255);
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v1.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v1.y);
                    vh.AddVert(vertex);
                    
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v2.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v2.y);
                    vertex.color.r = (byte)(Quads[i].c2.r * c.r / 255);
                    vertex.color.g = (byte)(Quads[i].c2.g * c.g / 255);
                    vertex.color.b = (byte)(Quads[i].c2.b * c.b / 255);
                    vertex.color.a = (byte)(Quads[i].c2.a * c.a / 255);
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v2.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v2.y);
                    vh.AddVert(vertex);
                    
                    vertex.position.x = Mathf.Lerp(rect.xMin, rect.xMax, Quads[i].v3.x);
                    vertex.position.y = Mathf.Lerp(rect.yMin, rect.yMax, Quads[i].v3.y);
                    vertex.color.r = (byte)(Quads[i].c3.r * c.r / 255);
                    vertex.color.g = (byte)(Quads[i].c3.g * c.g / 255);
                    vertex.color.b = (byte)(Quads[i].c3.b * c.b / 255);
                    vertex.color.a = (byte)(Quads[i].c3.a * c.a / 255);
                    vertex.uv0.x = Mathf.Lerp(uvRect.xMin, uvRect.xMax, Quads[i].v3.x);
                    vertex.uv0.y = Mathf.Lerp(uvRect.yMin, uvRect.yMax, Quads[i].v3.y);
                    vh.AddVert(vertex);
                    
                    vh.AddTriangle(n + 0, n + 1, n + 2);
                    vh.AddTriangle(n + 2, n + 3, n + 0);
                    n += 4;
                }
            }
        }
    }
}
