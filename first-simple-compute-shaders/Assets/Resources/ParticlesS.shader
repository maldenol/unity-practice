Shader "Unlit/ParticlesS" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            //UNITY_SHADER_NO_UPGRADE

            #pragma vertex   vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VertexData {
                float4 pos : POSITION;
            };

            struct FragmentData {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Size;

            FragmentData vert(VertexData v) {
                FragmentData f;

                f.pos = mul(UNITY_MATRIX_MVP, v.pos);
                f.uv  = (float2)0.0;

                return f;
            }

            FragmentData generateQuadBillboard(FragmentData f, float2 offset, float2 uv) {
                f.pos.xy += offset;
                f.uv = uv;

                return f;
            }

            [maxvertexcount(4)]
            void geom(point FragmentData input[1], inout TriangleStream<FragmentData> output) {
                // Getting input point
                FragmentData f = input[0];

                // Generating and adding another points to output stream
                output.Append(
                    generateQuadBillboard(f, float2(-1.0, -1.0) * _Size, float2(0.0, 0.0))
                );
                output.Append(
                    generateQuadBillboard(f, float2(1.0, -1.0) * _Size, float2(1.0, 0.0))
                );
                output.Append(
                    generateQuadBillboard(f, float2(-1.0, 1.0) * _Size, float2(0.0, 1.0))
                );
                output.Append(
                    generateQuadBillboard(f, float2(1.0, 1.0) * _Size, float2(1.0, 1.0))
                );

                // Restarting triangle strip
                output.RestartStrip();
            }

            fixed4 frag(FragmentData f) : SV_Target {
                fixed4 col = tex2D(_MainTex, f.uv);
                return col;
            }

            ENDCG
        }
    }
}
