using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Raw Poly Image")]
    public class UIRawPolyImage : RawImage
    {
        public Quad[] Quads = new Quad[0];
        public bool Transparent;
        
        public string Preview;
        
        RectTransform mRectTransform;
        
        protected override void Awake()
        {
#if UNITY_EDITOR
            if (UnityEditor.BuildPipeline.isBuildingPlayer)
            {
                Preview = null;
                return;
            }
#endif
            
            base.Awake();
            mRectTransform = GetComponent<RectTransform>();
        }
        
#if UNITY_EDITOR
        public override Texture mainTexture
        {
            get
            {
                if (!Application.isPlaying && !string.IsNullOrEmpty(Preview))
                {
                    return EditorHelp.Load<Texture>(Preview + ".png");
                }
                return base.mainTexture;
            }
        }
#endif
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            
            Texture tex = texture;
            
#if UNITY_EDITOR
            if (!Application.isPlaying && tex == null && !string.IsNullOrEmpty(Preview))
            {
                tex = mainTexture;
            }
#endif
            
            if ((tex == null && Transparent) || color.a <= 0)
            {
                return;
            }
            
            UIVertex vertex = new UIVertex();
            Rect rect = mRectTransform.rect;
            Rect uvRect = this.uvRect;
            Color32 c = color;
            int n = 0;
            
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
