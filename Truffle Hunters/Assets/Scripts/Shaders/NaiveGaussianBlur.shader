Shader "hidden/naive_gaussian_blur" 
{ 
	Properties 
	{ 
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Sigma ("Sigma", float) = 0.8
	}
	SubShader 
	{
		Tags { "Queue" = "Overlay" }
		Lighting Off 
		Cull Off 
		ZWrite Off 
		ZTest Always 

	    Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma multi_compile LITTLE_KERNEL MEDIUM_KERNEL BIG_KERNEL
			
			#include "UnityCG.cginc"
			#include "GaussianBlur.cginc"
			
			#pragma vertex vert_img
			#pragma fragment frag
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			uniform float _Sigma;
			uniform float4 _DirectionPass;

			float4 frag (v2f_img i) : COLOR
			{				
				float4 o = 0;
				float sum = 0;
				float2 uvOffset;
				float weight;

				
				for(int x = -35 / 2; x <= 35 / 2; ++x)
					for(int y = -35 / 2; y <= 35 / 2; ++y)
					{
						uvOffset = i.uv;
						uvOffset.x += x * _MainTex_TexelSize.x;
						uvOffset.y += y * _MainTex_TexelSize.y;
						weight = gauss(x, y, _Sigma);
						o += tex2D(_MainTex, uvOffset) * weight;
						sum += weight;
					}
				o *= (1.0f / sum);
				
				o = o.rrrr;
				//o = o * 2.0 - 1.0;
				return o;
			}
			
			ENDCG
		}
	}
}