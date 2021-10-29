// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "" {
	Properties {
	}
	SubShader {
		Tags { "RenderType"="Translucent" }
		ZWrite Off
		ZTest Off
		Blend One One
		
		Pass
		{
			CGPROGRAM
		    #pragma vertex vert
		    #pragma fragment frag
			#pragma target 3.0
		    #include "UnityCG.cginc"
			
			float4 _edge[8];
			float4 _edgeV[8];
			float4 _color;
			float _exponent;
			float _strength;

			struct INPUT
			{
				float4 vertex:POSITION;
			};

			struct v2f
			{
				float4 vertex:POSITION;
				float3 localPos:TEXCOORD0;
			};

			v2f vert(INPUT IN)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(IN.vertex);
				o.localPos = IN.vertex.xyz;
				return o;
			}
		   
			fixed4 frag (v2f IN) : COLOR
			{
				float val = 0;
				
				for (int i = 0; i < 8; ++i)
				{
					float3 v0 = IN.localPos - _edge[i].xyz;

					float denom = dot(_edgeV[i].xyz, _edgeV[i].xyz);
					float t = clamp(dot(v0, _edgeV[i].xyz) / denom, 0.0, 1.0);

					v0 = _edge[i].xyz + _edgeV[i].xyz * t - IN.localPos;
					float size = mix(_edge[i].w, _edgeV[i].w, t);
					float s = max(1.0 - (length(v0) * size), 0.0);

					s = pow(s, _exponent) * _strength;

					val = max(val, s * step(0.0001, size));
				}

				return val * _color;
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
