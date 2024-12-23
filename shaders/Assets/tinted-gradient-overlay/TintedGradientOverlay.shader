Shader "Custom/TintedGradientOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _ColorTop ("Top Color", Color) = (0, 0, 1, 1)
        _ColorBottom ("Bottom Color", Color) = (1, 0, 0, 1)
        _GradientTransition ("Gradient Transition", Range(0.1, 10.0)) = 1.0
        _Blend ("Blend Amount", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _Tint;
        float4 _ColorTop;
        float4 _ColorBottom;
        float _GradientTransition;
        float _Blend;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

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
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float factor = i.uv.y;
                factor = pow(factor, _GradientTransition);              
                float4 gradientColor = lerp(_ColorBottom, _ColorTop, factor) * _Tint;
                return lerp(baseTex, gradientColor, _Blend);
            }

            ENDHLSL
        }
    }
} 
