// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Editor/LightProbe" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_color0 ("Direct Light Color", Color) = (1, 1, 1, 1)
		_color1 ("Indirect Light Color", Color) = (0.2, 0, 0.5, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
		    #include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float3 _color0, _color1;

			struct INPUT
			{
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};
			struct OUTPUT
			{
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
			};
			
		    OUTPUT vert(INPUT IN)
		    {
		        OUTPUT o;
		        o.vertex = UnityObjectToClipPos(IN.vertex);
				o.uv = IN.uv;
		        return o;
		    }
		   
		    fixed4 frag (OUTPUT IN) : COLOR
		    {
		    	float4 texColor = tex2D(_MainTex, IN.uv);

				float4 result;
				result.xyz = lerp(_color1, _color0, texColor.x);
				result.a = texColor.a;

				return result;
		    }
		    
			ENDCG
		}
	} 
}
