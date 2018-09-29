Shader "Unlit/MushRoomsMask"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Perlin("Perlin", 2D) = "white" {}
		_lightness("lightness", Float) = 1
		_OverlayColor("OverlayColor", Color) = (1,1,1,1)
		_SeconderlyColor("SeconderyColor", Color) = (1,1,1,1)
		_UpLightingStrengh("UpLightingStrenght", Float) = 0.2

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
		float3 normal : NORMAL;

	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float4 screenPos : TEXCOORD2;
		float3 worldPos : TEXCOORD4;
		half3 worldNormal : TEXCOORD3;
	};

	sampler2D _MainTex;
	sampler2D _Perlin;
	float4 _MainTex_ST;
	uniform float4 _PlayerPos;
	uniform float4 _PlayerPos2;
	uniform float4 _CamDir;
	float _lightness;
	float4 _OverlayColor;
	float4 _SeconderlyColor;
	float _UpLightingStrengh;
	v2f vert(appdata v)
	{
		v2f o;
		float4 vertexGlobal = mul(unity_ObjectToWorld, v.vertex);
		float distanceToPlayerOne = distance(vertexGlobal, _PlayerPos );
		distanceToPlayerOne = smoothstep(0., 3., distanceToPlayerOne);

		distanceToPlayerOne = 1. - distanceToPlayerOne;
		//distanceToPlayerOne = pow(distanceToPlayerOne, 2.);
		float4 playerToVertex = normalize(vertexGlobal - _PlayerPos);

		float distanceToPlayerTwo = distance(vertexGlobal, _PlayerPos2);
		distanceToPlayerTwo = smoothstep(0., 3., distanceToPlayerTwo);

		distanceToPlayerTwo = 1. - distanceToPlayerTwo;
		float4 playerToVertexTwo = normalize(vertexGlobal - _PlayerPos2);

		float4 offset = playerToVertex * distanceToPlayerOne * 0.005 + playerToVertexTwo * distanceToPlayerTwo * 0.005;
		o.vertex = UnityObjectToClipPos(v.vertex + offset);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.screenPos = ComputeScreenPos(o.vertex);
		o.worldPos =  mul(unity_ObjectToWorld, float4(v.vertex.xyz + +offset, 1)).xyz;
		UNITY_TRANSFER_FOG(o,o.vertex);
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
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


		float3 dir2 = normalize(_WorldSpaceCameraPos - _PlayerPos2);
		float3 fragToPlayer2 = i.worldPos.xyz - _PlayerPos2;
		float distanceToPlayer2 = length(fragToPlayer2);
		fragToPlayer2 = normalize(fragToPlayer2);
		float toUse2 = dot(dir2, fragToPlayer2);
		toUse2 = saturate(toUse2);
		float checker2 = toUse2;
		toUse2 = 1. - toUse2;

		

		toUse = lerp(0.0, 1.0, smoothstep(0.04, noise /100. + 0.08 , toUse + noise/18.));

		toUse2 = lerp(0.0, 1.0, smoothstep(0.04, noise / 100. + 0.08, toUse2 + noise / 18.));


		col.a = toUse * toUse2;
		col.xyz *= _lightness ;
		col.xyz *= _OverlayColor;
		col.xyz =lerp(col.xyz, _SeconderlyColor, step(0.9, (col.x + col.y + col.z) / 3.));

		float upLighting = dot(float3(0., 1., 0.), i.worldNormal.xyz);
		upLighting = saturate(upLighting);

		col.xyz += upLighting*_UpLightingStrengh;
		return  col;
	}
		ENDCG
	}
	}
}
