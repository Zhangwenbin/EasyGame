// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/HiddenObject (EditorOnly)" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		ZWrite Off
		ColorMask 0
		Cull Off
		
		Pass
		{
			CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    float4 vert(float4 vertex:POSITION):POSITION
		    {
		        return UnityObjectToClipPos(vertex);
		    }
		   
		    fixed4 frag () : COLOR
		    {
		    	return 0.0;
		    }
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
