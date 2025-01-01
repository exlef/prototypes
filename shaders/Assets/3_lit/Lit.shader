Shader "Custom/Lit"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _AmbientColor ("Ambient Color", Color) = (0.1, 0.1, 0.1, 1)
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

            HLSLPROGRAM

            #define _SPECULAR_COLOR

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

                SurfaceData surfaceInput = (SurfaceData)0;
                surfaceInput.albedo = mainTexCol.rgb * _Tint.rgb;
                surfaceInput.alpha = mainTexCol.a * _Tint.a;
                surfaceInput.specular = 1;

                return UniversalFragmentBlinnPhong(lightingInput, surfaceInput);
            }

            ENDHLSL
        }
    }
}
