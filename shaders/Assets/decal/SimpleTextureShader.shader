Shader "Custom/UnlitSha derWithTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DecalTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Name "UnlitPass"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_DecalTex);
            SAMPLER(sampler_DecalTex);

            Interpolators vert(Attributes input)
            {
                Interpolators output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Interpolators input) : SV_Target
            {
                half4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 colorDecal = SAMPLE_TEXTURE2D(_DecalTex, sampler_DecalTex, input.uv);
                half4 resultColor = 1;
                if(colorDecal.a == 0)
                    resultColor = mainColor;
                else
                    resultColor = colorDecal;
                return resultColor;
            }
            ENDHLSL
        }
    }
    FallBack "Unlit/Transparent"
}
