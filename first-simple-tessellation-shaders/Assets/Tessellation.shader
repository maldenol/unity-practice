Shader "Custom/Tessellation" {
    Properties {
        [Header(TessellationFactors)]
        _TessFactorEdge0  ("Tessellation factor of edge vertex 0", float) = 1.0
        _TessFactorEdge1  ("Tessellation factor of edge vertex 1", float) = 1.0
        _TessFactorEdge2  ("Tessellation factor of edge vertex 2", float) = 1.0
        _TessFactorInside ("Tessellation factor of inside vertex", float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            #pragma vertex   tessVert
            #pragma hull     hull
            #pragma domain   domain
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _TessFactorEdge0;
            float _TessFactorEdge1;
            float _TessFactorEdge2;
            float _TessFactorInside;

            // Vertex
            struct Vertex {
                float4 pos : POSITION;
            };

            // Control point of patch and just another
            // structure of vertex
            struct TessControlPoint {
                float4 pos : INTERNALTESSPOS;
            };

            // Factors which specify on how many pieces
            // lines should be divided
            struct TessFactors {
                float edge[3] : SV_TessFactor;
                float inside  : SV_InsideTessFactor;
            };

            // Fragment
            struct Fragment {
                float4 pos : SV_POSITION;
            };

            // Special vertex function for tessellation
            // that just passes vertices to hull function
            TessControlPoint tessVert(Vertex v) {
                TessControlPoint tcp;

                tcp.pos = v.pos;

                return tcp;
            }

            // Function that calculates tessellation factors
            // for specified tessellation patch
            TessFactors patchConstant(InputPatch<TessControlPoint, 3> p) {
                TessFactors tf;

                tf.edge[0] = _TessFactorEdge0;
                tf.edge[1] = _TessFactorEdge1;
                tf.edge[2] = _TessFactorEdge2;
                tf.inside  = _TessFactorInside;

                return tf;
            }

            // Hull function
            // that is applied to each control point (vertex)
            // of tessellation patch (primitive)
            // which will be passed to tessellator
            [UNITY_domain("tri")]
            [UNITY_outputcontrolpoints(3)]
            [UNITY_outputtopology("triangle_cw")]
            [UNITY_partitioning("integer")]
            //[UNITY_partitioning("fractional_odd")]
            //[UNITY_partitioning("fractional_even")]
            [UNITY_patchconstantfunc("patchConstant")]
            TessControlPoint hull(
                InputPatch<TessControlPoint, 3> p,
                uint id : SV_OutputControlPointID
            ) {
                return p[id];
            }

            // Tessellator tessellates given patch
            // based on tessellation factors
            // returning barycentric coordinates weights
            // of old (control) and new vertices

            // Normal vertex function
            // that will be called in domain function
            Fragment vert(Vertex v) {
                Fragment f;

                f.pos = UnityObjectToClipPos(v.pos);

                return f;
            }

            // Domain function
            // that takes tessellation factors, barycentric coordinates weights
            // and patch of passed by tessellator vertex
            // and then passes it to vertex function and returns result fragment
            [UNITY_domain("tri")]
            Fragment domain(
                TessFactors tf,
                OutputPatch<TessControlPoint, 3> p,
                float3 barycentricCoordinatesWeights : SV_DomainLocation
            ) {
                Vertex v;

                v.pos =
                    p[0].pos * barycentricCoordinatesWeights.x +
                    p[1].pos * barycentricCoordinatesWeights.y +
                    p[2].pos * barycentricCoordinatesWeights.z;

                return vert(v);
            }

            // Normal fragment function
            fixed4 frag(Fragment f) : SV_Target {
                return fixed4(0.5, 0.5, 0.5, 1.0);
            }

            ENDCG
        }
    }
}
