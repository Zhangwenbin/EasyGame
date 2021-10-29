// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Editor/EditorPrimitive" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
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
			struct OUTPUT
			{
				float4 vertex:POSITION;
				float4 color:COLOR;
			};
			
		    OUTPUT vert(INPUT IN)
		    {
		        OUTPUT o;
		        o.vertex = UnityObjectToClipPos(IN.vertex);
				o.color = IN.color;
		        return o;
		    }
		   
		    fixed4 frag (OUTPUT IN) : COLOR
		    {
		    	return IN.color;
		    }
		    
			ENDCG
		}
	} 
}
