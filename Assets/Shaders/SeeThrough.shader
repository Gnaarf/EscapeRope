// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/SeeThrough"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Perlin("Perlin", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		ZWrite Off
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			sampler2D _Perlin;
			float4 _MainTex_ST;
			uniform float4 _PlayerScreenPos;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 screenPos = i.screenPos.xy / i.screenPos.w;
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed noise = tex2D(_Perlin, screenPos);
				float dis = smoothstep( 0.3, 0.4, distance(screenPos, _PlayerScreenPos) + noise/4.);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = dis;
				return float4(col);
			}
			ENDCG
		}
	}
}
