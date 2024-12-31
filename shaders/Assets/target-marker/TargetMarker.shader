Shader "Custom/TargetMarker"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _AmbientColor ("Ambient Color", Color) = (0.1, 0.1, 0.1, 1)
        [NoScaleOffset] _GridTex ("Grid Texture", 2D) = "black" {}
        _Speed ("Speed", Float) = 1
        _TargetX ("TargetX", Range(0, 1)) = 0.5
        _TargetY ("TargetY", Range(0, 1)) = 0.5
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
        float _Speed;
        float _TargetX;
        float _TargetY;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        TEXTURE2D(_GridTex);
        SAMPLER(sampler_GridTex);

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
            float2 uvNoScaleOffset : TEXCOORD3;
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
                o.uvNoScaleOffset = i.uv;
                return o;
            }

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

            float4 frag(VertexOutput i) : SV_Target
            {
                // Sample Base Texture
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _Tint;
                float4 gridTex = SAMPLE_TEXTURE2D(_GridTex, sampler_GridTex, i.uvNoScaleOffset);
                
                // lighting
                float3 lighting = DiffuseLighting(i);
                float4 litBase = float4(baseTex.rgb * lighting, baseTex.a);

                // distance mask
                float2 target = float2(_TargetX * _MainTex_ST.x ,_TargetY * _MainTex_ST.x);
                // float2 target = float2(0.5 * _MainTex_ST.x, 0.5 * _MainTex_ST.x);
                float distance = length(target - i.uv);
                distance += (sin(_Time.y * _Speed) + 1) * 0.5;
                distance = saturate(distance);
                distance = smoothstep(0.4, 0.6, distance);
                float distanceMask = 1 - distance;

                float4 gridTexApplied = litBase + gridTex;
                return lerp(litBase, gridTexApplied, distanceMask);
                // return lerp(litBase, gridTexApplied, gridTex.a);
            }

            ENDHLSL
        }
    }
}
