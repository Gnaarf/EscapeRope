Shader "Unlit/Band"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_EmissionColor("Emmision", Color) = (1,1,1,1)
	}
	SubShader

	{ 
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _EmissionColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			col = _EmissionColor* 2.;
			float2 uv = i.uv;
			uv.x = frac(uv.x *10.);
			float dist = distance(float2(0.5, 0.5), uv);
			dist = smoothstep(0.04, 0.1, dist);
			dist = 1. - dist;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = 0.9;
				col.a *= dist;
				return col;
			}
			ENDCG
		}
	}
}
