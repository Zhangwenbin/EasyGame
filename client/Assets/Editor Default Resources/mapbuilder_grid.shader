// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Editor/MapBuilder/Grid" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZTest Less

		Pass
		{
			CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
		    #include "UnityCG.cginc"

			struct INPUT
			{
				float4 vertex:POSITION;
				float4 color:COLOR;
			};
			struct v2f
			{
				float4 vertex:POSITION;
				float4 color:COLOR;
			};

		    v2f vert(INPUT IN)
		    {
		        v2f o;
		        o.vertex = UnityObjectToClipPos(IN.vertex);
		        o.color = IN.color;
		        return o;
		    }
		   
		    fixed4 frag (v2f IN) : COLOR
		    {
		    	return IN.color;
		    }
		    
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
