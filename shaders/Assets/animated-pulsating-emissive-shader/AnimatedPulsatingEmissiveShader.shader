Shader "Custom/AnimatedPulsatingEmissiveShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 5)) = 1.0
        _PulseSpeed ("Pulse Speed", Range(0.1, 10)) = 2.0
        _ScrollTex ("Scrolling Texture", 2D) = "black" {}
        _ScrollSpeed ("Scroll Speed", Vector) = (1, 1, 0, 0)
        _MaskTex ("Emission Mask", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _Tint;
        float4 _EmissionColor;
        float _PulseSpeed;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_ScrollTex);
        SAMPLER(sampler_ScrollTex);
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

            float GetSineTime(float speed)
            {
                // Use sine function to oscillate color over time
                float time = _Time.y * speed; // _Time.y gives the time in seconds
                float sineValue = sin(time); // Get sine value
                sineValue = (sineValue + 1.0) * 0.5; // Normalize to [0, 1]
                return sineValue;
            }

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float offset = _Time.y;
                i.uv.x += offset;
                float4 scrollerTex = SAMPLE_TEXTURE2D(_ScrollTex, sampler_ScrollTex, i.uv);
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv).a;
                
                float sine = GetSineTime(_PulseSpeed);
                
                float4 textured = baseTex * _Tint + scrollerTex.aaaa;
                float4 texturedEmmision = textured * _EmissionColor;
                float4 texturedEmmisionPulsate = lerp(textured, texturedEmmision, sine);
                return lerp (baseTex, texturedEmmisionPulsate, mask);
            }

            ENDHLSL
        }
    }
} 
