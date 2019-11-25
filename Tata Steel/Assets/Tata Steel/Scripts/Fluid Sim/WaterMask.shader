Shader "Custom/WaterMask"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Position("World Position", Vector) = (0,0,0,0)
		_Radius("Sphere Radius", Range(0, 100)) = 0
		_Softness("Sphere Softness", Range(0, 100)) = 0
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
		[HDR]_CutoffColor("CutoffColor", Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		//Cull Off

		CGPROGRAM
        #pragma surface surf Standard fullforwardshadows //alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float3 worldPos;
			float facing : VFACE;
        };

		half _Glossiness, _Metallic;
		fixed4 _Color;
		float4 _Emission, _CutoffColor;
			
		float4 _Plane0, _Plane1, _Plane2, _Plane3, _Plane4;

		/*void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			
			half d0 = dot(IN.worldPos, _Plane0.xyz) + _Plane0.w;
			half d1 = dot(IN.worldPos, _Plane1.xyz) + _Plane1.w;
			half d2 = dot(IN.worldPos, _Plane2.xyz) + _Plane2.w;
			half d3 = dot(IN.worldPos, _Plane3.xyz) + _Plane3.w;
			half d4 = dot(IN.worldPos, _Plane4.xyz) + _Plane4.w;
			clip(-d0);
			clip(-d1);
			clip(-d2);
			clip(-d3);
			clip(-d4);

			half facing = IN.facing * 0.5 + 0.5;

			o.Albedo = c.rgb * facing;
			o.Metallic = _Metallic * facing;
			o.Smoothness = _Glossiness * facing;
			o.Emission = lerp(_CutoffColor, _Emission, facing);
			o.Alpha = c.a;
		}*/

		half _Radius, _Softness;
		float4 _Position;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			half d = distance(_Position, IN.worldPos);
			half sum = (d - _Radius) / -_Softness;

			bool a = d > _Radius;
			//clip(-a);

			float facing = IN.facing * 0.5 + 0.5;

			o.Albedo = c * facing;
            o.Metallic = _Metallic * facing;
            o.Smoothness = _Glossiness * facing;
			o.Alpha = c.a;//sum * facing * c.a;
			//o.Emission = lerp(_CutoffColor, _Emission, facing);
        }

        ENDCG
    }
    FallBack "Standard"
}
