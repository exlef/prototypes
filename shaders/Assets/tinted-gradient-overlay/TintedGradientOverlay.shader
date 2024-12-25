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
        _Direction ("Gradient Direction", Range(0,1)) = 0.0 // 0 = Horizontal, 1 = Vertical
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.2)) = 0.05
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineBlend ("Outline Blend", Range(0, 1)) = 0.5
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
        float _Direction;
        float _OutlineThickness;
        float4 _OutlineColor;
        float _OutlineBlend;
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
            float2 uvNoTilingOffset : TEXCOORD1;
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
                o.uvNoTilingOffset = i.uv;
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                float edgeX = min(i.uvNoTilingOffset.x, 1.0 - i.uvNoTilingOffset.x);
                float edgeY = min(i.uvNoTilingOffset.y, 1.0 - i.uvNoTilingOffset.y);
                float edgeFactor = min(edgeX, edgeY);
                float outline = 1 - step(_OutlineThickness, edgeFactor);
                // float outline = 1 - smoothstep(0.0, _OutlineThickness, edgeFactor);

                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float factor = lerp(i.uv.x, i.uv.y, saturate(_Direction));
                factor = saturate(pow(abs(factor), _GradientTransition));
                
                float4 gradientColor = lerp(_ColorBottom, _ColorTop, factor);
                float4 texturedColor = lerp(baseTex * _Tint, gradientColor, _Blend);
                float4 finalColor = lerp(texturedColor, _OutlineColor, outline * _OutlineBlend);
                return finalColor;
            }

            ENDHLSL
        }
    }
} 
