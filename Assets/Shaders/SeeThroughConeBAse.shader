Shader "Unlit/SeeThroughConeBAse"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	_Perlin("Perlin", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		//ZWrite Off
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
		float3 worldPos : TEXCOORD1;
	};

	sampler2D _MainTex;
	sampler2D _Perlin;
	float4 _MainTex_ST;
	uniform float4 _PlayerPos;
	uniform float4 _CamDir;
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.screenPos = ComputeScreenPos(o.vertex);
		o.worldPos =  mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		float2 screenPos = i.screenPos.xy / i.screenPos.w;
		fixed4 col = tex2D(_MainTex, i.uv);
		fixed noise = tex2D(_Perlin, screenPos);

		UNITY_APPLY_FOG(i.fogCoord, col);

		
		float3 dir = normalize(_WorldSpaceCameraPos - _PlayerPos);
		float3 fragToPlayer = i.worldPos.xyz - _PlayerPos;
		float distanceToPlayer = length(fragToPlayer);
		fragToPlayer = normalize(fragToPlayer);
		float toUse=  dot(dir, fragToPlayer);
		toUse = saturate(toUse);
		float checker = toUse;
		toUse = 1. - toUse;

		
		distanceToPlayer = smoothstep(1.7, 7., distanceToPlayer);

		toUse = lerp(0.0, 1.0, smoothstep(0.04, noise /100. + 0.08 , toUse + noise/18.));


		col.a = toUse ;

		return  col;
	}
		ENDCG
	}
	}
}
