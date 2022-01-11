Shader "Diniska/PhongShader" {
    Properties {
        [Header(Texture)]
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Ambient)]
        _AmbInt ("Intensity", Range(0, 1)) = 1.0
        _AmbCol ("Color", Color) = (1, 1, 1, 1)
        _AmbCoef ("Coefficient", Range(0, 5)) = 0.15

        [Header(Diffuse)]
        _DifCoef ("Coefficient", Range(0, 5)) = 1.0

        [Header(Specular)]
        _SpecCoef ("Coefficient", Range(0, 5)) = 0.75
        _SpecShin ("Shinness", Range(0, 10)) = 5
    }
    SubShader {
        Tags { "RenderType"="Transparent" "LightMode"="ForwardBase" }
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Interpolator {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 light : COLOR0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _AmbInt;
            fixed4 _AmbCol;
            fixed _AmbCoef;
            fixed4 _LightColor0;
            fixed _DifCoef;
            fixed _SpecCoef;
            fixed _SpecShin;

            float phongLight(float4 vertex, fixed3 normal) {
                fixed4 ambient = _AmbCoef * _AmbInt * _AmbCol;
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 N = UnityObjectToWorldNormal(normal.xyz);
                float LdotN = max(0, dot(L, N));
                fixed4 diffuse = _DifCoef * _LightColor0 * LdotN;
                float4 worldPos = mul(unity_ObjectToWorld, vertex);
                float3 V = normalize(UnityWorldSpaceViewDir(worldPos.xyz));
                float3 R = 2 * LdotN * N - L;
                float specular = _SpecCoef * pow(_LightColor0 * max(0, dot(V, R)), _SpecShin);
                //float fresnel = pow(1 - dot(V, N), 2);
                //return ambient + diffuse + specular + fresnel;
                return ambient + diffuse + specular;
            }

            float blinnPhongLight(float4 vertex, fixed3 normal) {
                fixed4 ambient = _AmbCoef * _AmbInt * _AmbCol;
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 N = UnityObjectToWorldNormal(normal.xyz);
                float LdotN = max(0, dot(L, N));
                fixed4 diffuse = _DifCoef * _LightColor0 * LdotN;
                float4 worldPos = mul(unity_ObjectToWorld, vertex);
                float3 V = normalize(UnityWorldSpaceViewDir(worldPos.xyz));
                float3 H = normalize(L + V);
                float specular = _SpecCoef * pow(_LightColor0 * max(0, dot(H, N)), _SpecShin);
                //float fresnel = pow(1 - dot(V, N), 2);
                //return ambient + diffuse + specular + fresnel;
                return ambient + diffuse + specular;
            }

            Interpolator vert(appdata_base v) {
                Interpolator i;
                i.pos = UnityObjectToClipPos(v.vertex);
                i.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                i.light = phongLight(v.vertex, v.normal);
                return i;
            }

            fixed4 frag(Interpolator i) : SV_TARGET {
                fixed4 texel = tex2D(_MainTex, -i.uv);
                return texel * i.light;
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}