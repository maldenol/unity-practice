Shader "Diniska/PhongShader" {
    Properties {
        [Header(Material)]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _HeightMap ("Height map", 2D) = "black" {}
        _HeightStr ("Height map strength", Range(0, 2)) = 1
        [NoScaleOffset] _NormalMap ("Normal map", 2D) = "bump" {}
        _NormalStr ("Normal map strength", Range(0, 1)) = 1

        [Header(Ambient)]
        _AmbCoef ("Coefficient", Range(0, 5)) = 0.15
        [NoScaleOffset] _AmbOccMap ("Ambient occlusion map", 2D) = "white" {}
        _AmbOccStr ("Ambient occlusion map strength", Range(0, 1)) = 1

        [Header(Diffuse)]
        _DifCoef ("Coefficient", Range(0, 5)) = 0.6

        [Header(Specular)]
        _SpecCoef ("Coefficient", Range(0, 5)) = 0.35
        _SpecGlos ("Glossiness", Range(0, 10)) = 2
        [NoScaleOffset] _RoughMap ("Roughness map", 2D) = "black" {}
        _RoughStr ("Roughness map strength", Range(0, 1)) = 1
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