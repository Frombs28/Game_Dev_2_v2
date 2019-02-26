Shader "MK/Glow/Selective/Normal/DiffuseBumped" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		
		_MKGlowColor ("Glow Color", Color) = (1,1,1,1)
		_MKGlowPower ("Glow Power", Range(0.0,5.0)) = 2.5
		_MKGlowTex ("Glow Texture", 2D) = "black" {}
		_MKGlowTexColor ("Glow Texture Color", Color) = (1,1,1,1)
		_MKGlowTexStrength ("Glow Texture Strength ", Range(0.0,10.0)) = 1.0

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineThickness("Outline Thickness", Range(0,1)) = 0.3
		
	}
SubShader {
	Tags { "RenderType"="MKGlow" }
	LOD 300

	//Outline pass
		Pass {
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			half4 _OutlineColor;
			half _OutlineThickness;

			struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
			            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex+normalize(v.normal)*(_OutlineThickness/100));
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
			ENDCG
		}

	CGPROGRAM
	#pragma surface surf Lambert
	#pragma target 2.0

	sampler2D _MainTex;
	sampler2D _BumpMap;
	fixed4 _Color;

	sampler2D _MKGlowTex;
	half _MKGlowTexStrength;
	fixed4 _MKGlowTexColor;

	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv_MKGlowTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		fixed3 d = tex2D(_MKGlowTex, IN.uv_MKGlowTex) * _MKGlowTexColor;
		c.rgb += (d.rgb * _MKGlowTexStrength);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	}
ENDCG  
}

FallBack "Legacy Shaders/Diffuse"
}
