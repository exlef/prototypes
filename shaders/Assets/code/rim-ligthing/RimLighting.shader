Shader "Custom/RimLightingLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _AmbientColor ("Ambient Color", Color) = (0.1, 0.1, 0.1, 1)
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(1.0, 8.0)) = 3.0
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _Tint;
        float4 _AmbientColor;
        float4 _RimColor;
        float _RimPower;
        float _EmissionIntensity;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct VertexInput
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 worldNormal : TEXCOORD1;
            float3 worldPos : TEXCOORD2;
        };

        ENDHLSL

        Pass
        {
            Name "ForwardLit"
		    Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                o.worldNormal = TransformObjectToWorldNormal(i.normal);
                o.worldPos = TransformObjectToWorld(i.position.xyz);
                return o;
            }

            float3 ClampFloat3(float3 value, float3 minValue, float3 maxValue)
            {
                return min(max(value, minValue), maxValue);
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                // Sample Base Texture
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _Tint;

                // Normalize directions
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normal = normalize(i.worldNormal);

                // Rim Lighting using Fresnel Effect
                float rimFactor = 1.0 - saturate(dot(normal, viewDir));
                rimFactor = pow(rimFactor, _RimPower);
                float4 rimLight = _RimColor * rimFactor * _EmissionIntensity;
                
                float shadowness = dot(i.worldNormal, -GetMainLight().direction);
                shadowness = saturate(shadowness);

                // Apply Lighting (Diffuse + Rim)
                float3 lighting = GetMainLight().color * saturate(dot(normal, GetMainLight().direction));
                // Define an ambient light color
                lighting = saturate( lighting + _AmbientColor.rgb);
                // texture
                float4 litBase = float4(baseTex.rgb * lighting, baseTex.a);

                // Combine Rim Lighting with Base Lighting
                return litBase + rimLight * shadowness;
            }

            ENDHLSL
        }
    }
}
