// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Player"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Velocity("Velocity", Vector) = (0,0,0,0)
		_FrontPoint("FrontPoint", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
			float4 _Velocity;
			float4 _FrontPoint;
			
			v2f vert (appdata v)
			{
				v2f o;

				float4 t1 = dot(_Velocity.xyz, float4(0., 1., 0.,0.));
				float4 t2 = dot(_Velocity.xyz, float4(1., 0., 0.,0.));

				t1 = lerp(t2, t1, step(0.05, length(t1)));

				t2 = float4(cross(t1, _Velocity).xyz, 0.);


				float4 vertexWorldPos = mul(unity_ObjectToWorld, v.vertex);
				float distanceToFront = pow( smoothstep(0.1, 2., distance(vertexWorldPos, _FrontPoint)), 3.);
				float4 offset = -_Velocity * distanceToFront + (t1+t2)* - length( _Velocity)* pow(distanceToFront,8.) /700.;
				o.vertex = UnityObjectToClipPos(v.vertex + offset/2000.);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
