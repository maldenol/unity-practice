Shader "Diniska/UnlitShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _SpecColor ("Specular color", Color) = (1., 1., 1., 1.)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
            };

            struct Interpolator {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _SpecColor;

            Interpolator vert(MeshData v) {
                Interpolator i;
                i.pos = UnityObjectToClipPos(v.vertex);
                i.uv = TRANSFORM_TEX(v.uv0, _MainTex);
                return i;
            }

            fixed4 frag(Interpolator i) : SV_Target {
                fixed4 texel = tex2D(_MainTex, -i.uv);
                if (i.uv.x > 0.5)
                    texel *= _SpecColor;
                if (i.uv.y > 0.5)
                    texel /= _SpecColor;
                return texel;
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}