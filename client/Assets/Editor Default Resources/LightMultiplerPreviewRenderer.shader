// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Editor/LightMultiplerPreviewRenderer" {
	Properties {
	}
	SubShader {
		Pass
		{
			CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
		    #include "UnityCG.cginc"
			  
			sampler2D _MainTex;
			float _radius;
			float _exponent;
			float3 _color;
			
			struct INPUT
			{
				float4 vertex:POSITION;
				float2 tex:TEXCOORD0;
			};
			struct OUTPUT
			{
				float4 vertex:POSITION;
				float2 tex:TEXCOORD0;
			};
			
		    OUTPUT vert(INPUT IN)
		    {
		        OUTPUT o;
		        o.vertex = UnityObjectToClipPos(IN.vertex);
		        o.tex = IN.tex;
		        return o;
		    }
		   
		    fixed4 frag (OUTPUT IN) : COLOR
		    {
		    	float2 dist = length(IN.tex - 0.5);
		    	
		    	float t = clamp(1.0 - dist / _radius, 0, 1);
		    	t = pow(t, _exponent);
		    	
		    	float4 texColor = tex2D(_MainTex, IN.tex);
		    	float3 multiplied = texColor.xyz * _color;
		    	
		    	return fixed4(lerp(texColor.xyz, multiplied, t), 1);
		    }
		    
			ENDCG
		}
	} 
}
