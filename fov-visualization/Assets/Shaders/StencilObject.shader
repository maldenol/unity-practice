Shader "Custom/StencilObject" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Stencil {
            Ref 1
            Comp equal
        }

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input input, inout SurfaceOutputStandard sos) {
            fixed4 col = tex2D (_MainTex, input.uv_MainTex) * _Color;
            sos.Albedo = col.rgb;
            sos.Metallic = _Metallic;
            sos.Smoothness = _Glossiness;
            sos.Alpha = col.a;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
