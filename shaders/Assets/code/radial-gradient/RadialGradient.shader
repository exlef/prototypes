Shader "Custom/RadialGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlendAmount ("Blend Amount", Range(0,1)) = 0.5
        _Tint ("Tint", Color) = (1,1,1,1)
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", Range(0,1)) = 0.5 
        _GradientFactor ("Gradient Factor", Range(1,4)) = 1
        _InnerColor ("Inner Color", Color) = (1,1,1,1)
        _OuterColor ("Outer Color", Color) = (1,1,1,1)
        [NoScaleOffset] _NoiseTex ("Noise Tex", 2D) = "gray" {}
        [NoScaleOffset] _MaskTex ("Mask Tex", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0,1)) = 0 
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float _BlendAmount;
        float4 _Tint;
        float2 _Center;
        float _Radius;
        float _GradientFactor;
        float4 _InnerColor;    
        float4 _OuterColor;
        float _DistortionStrength;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);
        TEXTURE2D(_MaskTex);
        SAMPLER(sampler_MaskTex);

        struct VertexInput
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        ENDHLSL

        
        Pass
        {
            Name "Forward"
		    Tags { "LightMode"="SRPDefaultUnlit" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                float2 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv).rg * _DistortionStrength; 
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 maskTex = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                float distance = saturate(length(i.uv + noise - _Center.xy));
                distance += 1 - _Radius;
                distance = saturate(distance);

                float gradientMask = saturate(pow(distance, _GradientFactor));
                float4 gradientCol = lerp(_InnerColor, _OuterColor, gradientMask);
                float4 textureGradientCol = lerp(baseTex, gradientCol, _BlendAmount);
                return lerp(baseTex, textureGradientCol, maskTex.r);
            }

            ENDHLSL
        }
    }
} 
