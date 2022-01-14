Shader "Diniska/PhongShader" {
    Properties {
        [Header(Material)]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)

        [Header(Ambient)]
        _AmbCoef ("Coefficient", Range(0, 5)) = 0.15

        [Header(Diffuse)]
        _DifCoef ("Coefficient", Range(0, 5)) = 0.6

        [Header(Specular)]
        _SpecCoef ("Coefficient", Range(0, 5)) = 0.35
        _SpecGlos ("Glossiness", Range(0, 10)) = 2
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        Pass {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #include "PhongShader.cginc"
            ENDCG
        }
        Pass {
            Tags { "LightMode"="ForwardAdd" }
            Blend One One
            CGPROGRAM
            #pragma multi_compile_fwdadd
            #include "PhongShader.cginc"
            ENDCG
        }
    }
    Fallback "Diffuse"
}