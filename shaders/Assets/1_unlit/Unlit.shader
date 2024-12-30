Shader "Custom/Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        float4 _Tint;
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

                return baseTex * _Tint;
            }

            ENDHLSL
        }
    }
} 

/* commeted version
Shader "Custom/Unlit" // Shader name, used to reference this shader in Unity
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // A 2D texture property, defaulting to white
        _Tint ("Tint", Color) = (1,1,1,1)     // A color property for tinting the texture
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        // Tags specify rendering settings. This shader is for the Universal Render Pipeline (URP)
        // and will render in the "Geometry" queue.

        HLSLINCLUDE // Start of the HLSL (shader code) section

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        // Include common functions and macros for the Universal Render Pipeline.

        // Define constant buffer for material properties (updated by Unity automatically).
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST; // Used for texture tiling and offset (_ST = Scale, Translation)
        float4 _Tint;       // Color property to tint the texture
        CBUFFER_END

        // Declare the texture and its sampler
        TEXTURE2D(_MainTex);        // Declares the 2D texture _MainTex
        SAMPLER(sampler_MainTex);   // Declares the sampler for _MainTex (controls sampling behavior)

        // Input structure for the vertex shader
        struct VertexInput
        {
            float4 position : POSITION; // Vertex position in object space
            float2 uv : TEXCOORD0;      // UV coordinates for the texture
        };

        // Output structure from the vertex shader to the fragment shader
        struct VertexOutput
        {
            float4 position : SV_POSITION; // Transformed position for screen space rendering
            float2 uv : TEXCOORD0;         // UV coordinates passed to the fragment shader
        };

        ENDHLSL // End of shared HLSL code section

        Pass
        {
            Name "Forward" // Name of this rendering pass
            Tags { "LightMode"="SRPDefaultUnlit" } 
            // Tags define this as an unlit shader for URP's forward rendering path.

            HLSLPROGRAM // Start of the pass-specific HLSL program

            #pragma vertex vert     // Specifies the vertex shader function
            #pragma fragment frag   // Specifies the fragment (pixel) shader function

            // Vertex shader: Transforms vertices and passes UVs to the fragment shader.
            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;
                o.position = TransformObjectToHClip(i.position.xyz);
                // Transforms the vertex position from object space to clip space for rendering.

                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                // Applies tiling and offset to the UV coordinates using _MainTex_ST.
                return o;
            }

            // Fragment shader: Calculates the final color of each pixel.
            float4 frag(VertexOutput i) : SV_Target
            {
                // Samples the texture at the given UV coordinates.
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // Multiplies the texture color by the tint to apply the color adjustment.
                return baseTex * _Tint;
            }

            ENDHLSL // End of the HLSL program for this pass
        }
    }
}

*/