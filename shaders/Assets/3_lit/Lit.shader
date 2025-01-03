Shader "Custom/Lit"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _Tint;
        float _Smoothness;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct VertexInput
        {
            float3 positionOS : POSITION;
            float2 uv : TEXCOORD0;
            float3 normalOS : NORMAL;
        };

        struct VertexOutput
        {
            float4 positionCS : SV_POSITION;
            float2 uv : TEXCOORD0;
            float3 positionWS : TEXCOORD1;
            float3 normalWS : TEXCOORD2;
        };

        ENDHLSL

        Pass
        {
            Name "ForwardLit" // for debugging
		    Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM

            #define _SPECULAR_COLOR
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            VertexOutput vert(VertexInput IN)
            {
                VertexOutput OUT;
                
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);

                OUT.positionCS = posInputs.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS = normalInputs.normalWS;
                
                return OUT;
            }

            float4 frag(VertexOutput IN) : SV_Target
            {
                float4 mainTexCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = IN.positionWS;
                lightingInput.normalWS = normalize(IN.normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);

                SurfaceData surfaceInput = (SurfaceData)0;
                surfaceInput.albedo = mainTexCol.rgb * _Tint.rgb;
                surfaceInput.alpha = mainTexCol.a * _Tint.a;
                surfaceInput.specular = 1;
                surfaceInput.smoothness = _Smoothness;
                
                return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
            }

            ENDHLSL
        }
        Pass 
        {
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma vertex ShadowPassVertex
			#pragma fragment ShadowPassFragment

			// Material Keywords
			#pragma shader_feature_local_fragment _ALPHATEST_ON
			#pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			// GPU Instancing
			#pragma multi_compile_instancing
			//#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

			
			ENDHLSL
		}
    }
}
