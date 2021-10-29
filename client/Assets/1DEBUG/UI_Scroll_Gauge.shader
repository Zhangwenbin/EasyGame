// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:33229,y:32719,varname:node_1873,prsc:2|emission-2104-OUT,alpha-603-OUT;n:type:ShaderForge.SFN_Tex2d,id:4805,x:32489,y:32720,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex_copy,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:True,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:5376,x:32489,y:32937,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Multiply,id:603,x:32772,y:32992,cmnt:A,varname:node_603,prsc:2|A-4805-A,B-5376-A;n:type:ShaderForge.SFN_Blend,id:120,x:32772,y:32775,cmnt:RGB,varname:node_120,prsc:2,blmd:10,clmp:False|SRC-4805-RGB,DST-5376-RGB;n:type:ShaderForge.SFN_Panner,id:1979,x:32554,y:32512,varname:node_1979,prsc:2,spu:0,spv:1|UVIN-5980-UVOUT,DIST-7705-OUT;n:type:ShaderForge.SFN_Tex2d,id:9713,x:32697,y:32387,ptovrint:False,ptlb:Effect_tex,ptin:_Effect_tex,varname:node_9713,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:1,isnm:False|UVIN-1979-UVOUT;n:type:ShaderForge.SFN_Add,id:2104,x:33028,y:32661,varname:node_2104,prsc:2|A-120-OUT,B-1393-OUT;n:type:ShaderForge.SFN_Panner,id:5980,x:32329,y:32512,varname:node_5980,prsc:2,spu:1,spv:0|UVIN-3970-UVOUT,DIST-2589-OUT;n:type:ShaderForge.SFN_Time,id:733,x:31573,y:32568,varname:node_733,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2589,x:32005,y:32583,varname:node_2589,prsc:2|A-733-T,B-9512-OUT;n:type:ShaderForge.SFN_Multiply,id:7705,x:32272,y:32682,varname:node_7705,prsc:2|A-733-T,B-213-OUT;n:type:ShaderForge.SFN_Slider,id:9512,x:31586,y:32737,ptovrint:False,ptlb:Speed_U,ptin:_Speed_U,varname:_Speed_U_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_TexCoord,id:3970,x:32045,y:32409,varname:node_3970,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:213,x:31909,y:32827,ptovrint:False,ptlb:Speed_V,ptin:_Speed_V,varname:_Speed_V_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:1393,x:33001,y:32452,varname:node_1393,prsc:2|A-9713-RGB,B-7511-RGB;n:type:ShaderForge.SFN_Color,id:7511,x:32850,y:32309,ptovrint:False,ptlb:Effect Color,ptin:_EffectColor,varname:node_7511,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:4805-7511-9713-9512-213;pass:END;sub:END;*/

Shader "LapisWars/UI/UI-Scroll_gauge" {
    Properties {
        [HideInInspector][PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _EffectColor ("Effect Color", Color) = (0.5,0.5,0.5,1)
        _Effect_tex ("Effect_tex", 2D) = "gray" {}
        _Speed_U ("Speed_U", Range(-5, 5)) = 0
        _Speed_V ("Speed_V", Range(-5, 5)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d11 gles3 metal 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Effect_tex; uniform float4 _Effect_tex_ST;
            uniform float _Speed_U;
            uniform float _Speed_V;
            uniform float4 _EffectColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_733 = _Time;
                float2 node_1979 = ((i.uv0+(node_733.g*_Speed_U)*float2(1,0))+(node_733.g*_Speed_V)*float2(0,1));
                float4 _Effect_tex_var = tex2D(_Effect_tex,TRANSFORM_TEX(node_1979, _Effect_tex));
                float3 emissive = (( i.vertexColor.rgb > 0.5 ? (1.0-(1.0-2.0*(i.vertexColor.rgb-0.5))*(1.0-_MainTex_var.rgb)) : (2.0*i.vertexColor.rgb*_MainTex_var.rgb) )+(_Effect_tex_var.rgb*_EffectColor.rgb));
                float3 finalColor = emissive;
                return fixed4(finalColor,(_MainTex_var.a*i.vertexColor.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
