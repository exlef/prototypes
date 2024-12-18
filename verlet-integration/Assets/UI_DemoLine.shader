
            Shader "UI_Demo/Line"
            {
                            SubShader
                {
                                Pass
                    {
                                    CGPROGRAM
                        #pragma vertex vert
                        #pragma fragment frag

                        # include "UnityCG.cginc"

                        StructuredBuffer<float4> _LinesBuffer;

                        struct v2f
                    {
                        float4 pos : SV_POSITION;
                        };

                    float4x4 CreateMatrixFromPosition(float4 position)
                    {
                        float4x4 translationMatrix = float4x4(
                            1, 0, 0, position.x,
                            0, 1, 0, position.y,
                            0, 0, 1, position.z,
                            0, 0, 0, 1
                        );

                        return translationMatrix;
                    }

                    float4x4 CreateMatrixFrom2DPosition(float2 position, float z)
                    {
                        float4x4 translationMatrix = float4x4(
                            1, 0, 0, position.x,
                            0, 1, 0, position.y,
                            0, 0, 1, z,
                            0, 0, 0, 1
                        );

                        return translationMatrix;
                    }


                    v2f vert(appdata_base v, uint instanceID : SV_InstanceID, uint vertexID : SV_VertexID)
                    {
                        v2f o;
                        
                        // Fetch line start and end points from buffer
                        float4 linePoints = _LinesBuffer[instanceID];
            
                        // Choose between start and end point based on vertex position
                        float2 instancePosition = vertexID == 0 ? linePoints.xy : linePoints.zw;

                        // Create matrix procedurally in shader
                        float4x4 instanceMatrix = CreateMatrixFrom2DPosition(instancePosition, 0);

                        // Transform vertex
                        o.pos = mul(UNITY_MATRIX_VP, mul(instanceMatrix, v.vertex));

                        return o;
                    }

                    float4 frag(v2f i) : SV_Target
                        {
                            return float4(1,1,1,1);
                }
                ENDCG
            }
                }
            }