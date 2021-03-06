Shader "Diniska/SimpleColorShader" {
    Properties {
        //[Header(Header_Name)]
        //_Value ("Value", Type) = Default_Value
    }
    SubShader {
        Tags {
            "RenderType"="Transparent" // for post-processing
            "Queue"="Transparent" // actual render order
        }
        Pass {
            ZWrite Off // do not write at Z-buffer
            //ZTest LEqual // Z-test
            //Cull Off // culling (Back, Off, Front)
            BlendOp Add // blend mode operation
            Blend One One // additive blending
            //Blend DstColor Zero // multiplicative blending
            //Blend Zero SrcColor // multiplicative blending

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
                //fixed3 normal : NORMAL;
                //fixed4 tangent : TANGENT;
                //fixed4 color : COLOR;
                //float2 uv0 : TEXCOORD0;
                //float2 uv1 : TEXCOORD1;
            };

            struct Interpolator {
                fixed4 pos : SV_POSITION;
                fixed4 col : COLOR;
                //float2 uv : TEXCOORD0;
            };

            //Value_Type _Value;

            Interpolator vert(MeshData v) {
                Interpolator i;
                i.pos = UnityObjectToClipPos(v.vertex);
                i.col = _SinTime;
                return i;
            }

            fixed4 frag(Interpolator i) : SV_Target {
                return i.col;
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}
