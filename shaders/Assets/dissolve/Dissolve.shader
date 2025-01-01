Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _AmbientColor ("Ambient Color", Color) = (0.1, 0.1, 0.1, 1)
        _NoiseTex ("Noise", 2D) = "gray" {}
        _DissolveThreshold ("Dissolve Threshold", Range(0,1)) = 0.3
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
        float _DissolveThreshold;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_NoiseTex);
        SAMPLER(sampler_NoiseTex);

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

            float3 DiffuseLighting(VertexOutput i)
            {
                // Normalize directions
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 normal = normalize(i.worldNormal);

                // Apply Lighting (Diffuse)
                float3 lighting = GetMainLight().color * saturate(dot(normal, GetMainLight().direction));
                // Define an ambient light color
                lighting = saturate( lighting + _AmbientColor.rgb);
                // texture
                return lighting;
            }

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
                float4 noiseTex = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv) * _Tint;

                float narrowmask = step(_DissolveThreshold, noiseTex.r);
                float widemask = step(_DissolveThreshold - 0.2, noiseTex.r);
                float mask = widemask - narrowmask;
                if ( mask == 0 ) return float4(1,0,0,1);
                return mask;
                // return maskWider.xxxx;
                //return float4(mask, mask2, 0, 1);

                // float4 maskWcol = maskWider * float4(1,0,0,1);
                // float4 maskCol = mask * float4(0,1,0,1);
                // if(maskCol.a == 0) discard;
                // return maskWcol;


                // float3 lighting = DiffuseLighting(i);
                // float4 litBase = float4(baseTex.rgb * lighting, baseTex.a);
                
                // float4 colored = lerp(float4(1,0,0,0), litBase, mask2);
                // float4 maskedWiderColor = lerp(masked, litBase, maskWider);
                // float4 masked = lerp(float4(1, 0, 0, 1), litBase, mask);
                // return maskedWidered;
                
                // float4 final =  masked * mask;
                // if(final.a == 0) discard;
                // return final;
            }

            ENDHLSL
        }
    }
}
