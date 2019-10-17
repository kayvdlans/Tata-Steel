Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		[NoScaleOffset] _MainTex("Deriv (AG) Height (B)", 2D) = "black" {}
		[NoScaleOffset] _FlowMap("Flow (RG)", 2D) = "black" {}
		[Toggle(_DUAL_GRID)] _DualGrid("Dual Grid", Int) = 0
		_Tiling("Tiling, Constant", Float) = 1
		_TilingModulated("Tiling, Modulated", Float) = 1
		_GridResolution("Grid Resolution", Float) = 10
		_Speed("Speed", Float) = 1
		_FlowStrength("Flow Strength", Float) = 1
		_HeightScale("Height Scale, Constant", Float) = 0.25
		_HeightScaleModulated("Height Scale, Modulated", Float) = 0.75
		_WaterFogColor("Water Fog Color", Color) = (0, 0, 0, 0)
		_WaterFogDensity("Water Fog Density", Range(0, 2)) = 0.1
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

		GrabPass { "_WaterBackground" }

		LOD 200

		Stencil 
		{
			Ref 1
			Comp Equal
		}

		CGPROGRAM
		#pragma surface surf Standard alpha
		#pragma target 3.0
		#pragma shader_feature _DUAL_GRID

		sampler2D _CameraDepthTexture , _WaterBackground;
		float4 _CameraDepthTexture_TexelSize;
		float3 _WaterFogColor;
		float _WaterFogDensity;

		sampler2D _MainTex, _FlowMap;
		float _Tiling, _TilingModulated;
		float _GridResolution, _Speed, _FlowStrength;
		float _HeightScale, _HeightScaleModulated;

		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float2 directionalFlowUV(float2 uv, float3 flowVectorAndSpeed, float tiling, float time, out float2x2 rotation) 
		{
			float2 dir = normalize(flowVectorAndSpeed.xy);
			
			rotation = float2x2(dir.y, dir.x, -dir.x, dir.y);

			uv = mul(float2x2(dir.y, -dir.x, dir.x, dir.y), uv);
			uv.y -= time * flowVectorAndSpeed.z;

			return uv * tiling;
		}

		float3 unpackDerivativeHeight(float4 textureData) 
		{
			float3 dh = textureData.agb;
			dh.xy = dh.xy * 2 - 1;
			return dh;
		}

		float3 flowCell(float2 uv, float2 offset, float time, bool gridB) 
		{
			float2 shift = 1 - offset;

			shift *= 0.5;
			offset *= 0.5;

			if (gridB) 
			{
				offset += 0.25;
				shift -= 0.25;
			}

			float2x2 derivRotation;

			float2 uvTiled = (floor(uv * _GridResolution + offset) + shift) / _GridResolution;

			float3 flow = tex2D(_FlowMap, uvTiled).rgb;
			flow.xy = flow.xy * 2 - 1;
			flow.z *= _FlowStrength;

			float tiling = flow.z * _TilingModulated + _Tiling;

			float2 uvFlow = directionalFlowUV(uv + offset, flow, tiling, time, derivRotation);

			float3 dh = unpackDerivativeHeight(tex2D(_MainTex, uvFlow));
			dh.xy = mul(derivRotation, dh.xy);
			dh *= flow.z * _HeightScaleModulated + _HeightScale;

			return dh;
		}

		float3 flowGrid(float2 uv, float time, bool gridB) 
		{
			float3 dhA = flowCell(uv, float2(0, 0), time, gridB);
			float3 dhB = flowCell(uv, float2(1, 0), time, gridB);
			float3 dhC = flowCell(uv, float2(0, 1), time, gridB);
			float3 dhD = flowCell(uv, float2(1, 1), time, gridB);

			float2 t = uv * _GridResolution;
			
			if (gridB)
			{
				t += 0.25;
			}

			t = abs(2 * frac(t) - 1);

			float wA = (1 - t.x) * (1 - t.y);
			float wB = t.x * (1 - t.y);
			float wC = (1 - t.x) * t.y;
			float wD = t.x * t.y;

			return dhA * wA + dhB * wB + dhC * wC + dhD * wD;
		}

		float3 colorBelowWater(float4 screenPos) 
		{
			float2 uv = screenPos.xy / screenPos.w;

			#if UNITY_UV_STARTS_AT_TOP
				if (_CameraDepthTexture_TexelSize.y < 0)
				{
					uv.y = 1 - uv.y;
				}
			#endif
			float backgroundDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv));
			float surfaceDepth = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenPos.z);

			float depthDifference = backgroundDepth - surfaceDepth;

			float3 backgroundColor = tex2D(_WaterBackground, uv).rgb;
			float fogFactor = exp2(-_WaterFogDensity * depthDifference);

			return lerp(_WaterFogColor, backgroundColor, fogFactor);
		}

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float2 uv = IN.uv_MainTex;
			float time = _Time.y * _Speed;

			float3 dh = flowGrid(uv, time, false);
			#if defined(_DUAL_GRID)
				dh = (dh + flowGrid(uv, time, true)) * 0.5;
			#endif
			fixed4 c = dh.z * dh.z * _Color;

			o.Albedo = c.rgb;
			o.Normal = normalize(float3(-dh.xy, 1));

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			o.Emission = colorBelowWater(IN.screenPos) * (1 - c.a);
		}
		ENDCG
    }
}
