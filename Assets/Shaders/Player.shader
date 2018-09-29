// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Player"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Velocity("Velocity", Vector) = (0,0,0,0)
		_FrontPoint("FrontPoint", Vector) = (0,0,0,0)
		_PlayerColor("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest off 
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
				float3 worldPos : TEXCOORD2;
				half3 worldNormal : TEXCOORD1;

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Velocity;
			float4 _FrontPoint;
			float4 _PlayerColor;
			uniform float4 _PlayerPos;

			v2f vert (appdata v)
			{
				v2f o;
				float4 clampedVelocity;
				if(length(_Velocity) != 0) clampedVelocity = normalize(_Velocity) * clamp(length(_Velocity), 0., 18.);
				
				else clampedVelocity = _Velocity;

				float4 t1 = dot(clampedVelocity.xyz, float4(0., 1., 0.,0.));
				float4 t2 = dot(clampedVelocity.xyz, float4(1., 0., 0.,0.));

				t1 = lerp(t2, t1, step(0.05, length(t1)));

				t2 = float4(cross(t1, clampedVelocity).xyz, 0.);


				float4 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
				float distanceToFront = pow( smoothstep(0.1, 2., distance(vertexWorldPos, _FrontPoint)), 3.);
				float4 offset = -clampedVelocity * distanceToFront + (t1+t2)* - length(clampedVelocity)* pow(distanceToFront,8.) /700.;
				o.vertex = UnityObjectToClipPos(v.vertex + offset/2000.);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);


				o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz + offset.xyz / 2000., 1)).xyz;

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float3 dir = normalize(_WorldSpaceCameraPos - i.worldPos);
				float rim = dot(dir, i.worldNormal.xyz);
				rim = abs(rim);
				rim = 1. - rim;

				float upLighting = dot(float3(0., 1., 0.), i.worldNormal.xyz);
				upLighting = saturate(upLighting);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.xyz = _PlayerColor + i.worldNormal.xyz*0.4 + upLighting*0.4;
				col.a = 0.1 +rim;
				return col;
			}
			ENDCG
		}
	}
}
