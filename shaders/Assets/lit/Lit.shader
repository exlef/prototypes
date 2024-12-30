Shader "Custom/Lit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MyBaseColor("Base Color", color) = (1,1,1,1)
        _MySmoothness("Smoothness", Range(0,1)) = 0
        _MyMetallic("Metallic", Range(0,1)) = 0
    }

    SubShader
    {
        Tags {
			"RenderPipeline"="UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry"
		}

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _MyBaseColor;
        float _MySmoothness;
        float _MyMetallic;
        CBUFFER_END

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        struct VertexInput
        {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
            float4 normal : NORMAL;
            float4 texCoord1 : TEXCOORD1; // for light map and GI
        };

        struct VertexOutput
        {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
            DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
            float3 positionWS : TEXCOORD2;
            float3 normalWS : TEXCOORD3;
            float3 viewDirWS : TEXCOORD4;
        };

        ENDHLSL

        
        Pass
        {
            Name "Forward"
		    Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            // ---------------------------------------------------------------------------
			// Keywords
			// ---------------------------------------------------------------------------

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                o.positionWS = TransformObjectToWorld(i.position.xyz);
                o.normalWS = TransformObjectToWorldDir(i.normal.xyz);
                o.viewDirWS = _WorldSpaceCameraPos - o.positionWS;
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                OUTPUT_LIGHTMAP_UV(i.texCoord1, unity_LightmapST, o.lightmapUV);
			    OUTPUT_SH(o.normalWS.xyz, o.vertexSH);
                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                
                // lighting
				InputData inputData = (InputData)0;
                inputData.positionWS = i.positionWS;
                inputData.normalWS = normalize(i.normalWS);
                inputData.viewDirectionWS = normalize(i.viewDirWS);
                inputData.bakedGI = SAMPLE_GI(i.lightmapUV, i.vertexSH, inputData.normalWS);
                inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                inputData.fogCoord = 0;
                inputData.vertexLighting = real3(0, 0, 0);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.position);
                inputData.shadowMask = real4(1, 1, 1, 1);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = _MyBaseColor.rgb;
                surfaceData.specular = 0;
                surfaceData.metallic = _MyMetallic; 
                surfaceData.smoothness = _MySmoothness;
                surfaceData.normalTS = 0;
                surfaceData.emission = 0;
                surfaceData.occlusion = 1;
                surfaceData.alpha = 1;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;
                
                float4 color = UniversalFragmentPBR(inputData, surfaceData);

                return color;
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
                    
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }
    }
} 