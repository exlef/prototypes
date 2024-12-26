Shader "Custom/Decal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DecalTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _DecalTex_ST;
        float4 _Tint;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_DecalTex);
        SAMPLER(sampler_DecalTex);

        struct VertexInput
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            float2 uvDecal : TEXCOORD1;
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
                o.uvDecal = TRANSFORM_TEX(i.uv, _DecalTex);
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 decalTex = SAMPLE_TEXTURE2D(_DecalTex, sampler_DecalTex, i.uvDecal);
                float4 tintedBaseTex = baseTex * _Tint;

                float4 finalColor = lerp(tintedBaseTex, decalTex, decalTex.a);

                return finalColor;
            }

            ENDHLSL
        }
    }
} 



















































// Shader "Custom/Decal"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//         _DecalTex ("Texture", 2D) = "white" {}
//     }
//     SubShader
//     {
//         Tags { "RenderType"="Transparent" "Queue"="Transparent" }

//         Pass
//         {
//             Name "UnlitPass"
//             Tags { "LightMode"="UniversalForward" }

//             Blend SrcAlpha OneMinusSrcAlpha
//             ZWrite Off

//             HLSLPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

//             struct Attributes
//             {
//                 float4 positionOS : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct Interpolators
//             {
//                 float4 positionHCS : SV_POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             TEXTURE2D(_MainTex);
//             SAMPLER(sampler_MainTex);
//             TEXTURE2D(_DecalTex);
//             SAMPLER(sampler_DecalTex);

//             Interpolators vert(Attributes input)
//             {
//                 Interpolators output;
//                 output.positionHCS = TransformObjectToHClip(input.positionOS);
//                 output.uv = input.uv;
//                 return output;
//             }

//             half4 frag(Interpolators input) : SV_Target
//             {
//                 half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
//                 half4 colorDecal = SAMPLE_TEXTURE2D(_DecalTex, sampler_DecalTex, input.uv);
//                 half4 resultColor = 1;
//                 if(colorDecal.a == 0)
//                     resultColor = mainColor;
//                 else
//                     resultColor = colorDecal;
//                 return resultColor;
//             }
//             ENDHLSL
//         }
//     }
//     FallBack "Unlit/Transparent"
// }
