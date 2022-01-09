Shader "Diniska/PhongShader"
{
    Properties
    {
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
    SubShader
    {
        Tags { "RenderType"="Transparent" "LightMode"="ForwardBase" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
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

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                fixed4 ambient = _AmbCoef * _AmbInt * _AmbCol;
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
                float lightAndNormalDotProduct = max(0, dot(lightDir, worldNormal));
                fixed4 diffuse = _DifCoef * _LightColor0 * lightAndNormalDotProduct;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos.xyz));
                float3 idealSpecularDir = 2 * lightAndNormalDotProduct * worldNormal - lightDir;
                float specular = _SpecCoef * pow(_LightColor0 * max(0, dot(viewDir, idealSpecularDir)), _SpecShin);
                o.light = ambient + diffuse + specular;
                return o;
            }

            fixed4 frag (v2f i) : SV_TARGET
            {
                fixed4 col = tex2D(_MainTex, -i.uv);
                return col * i.light;
            }

            ENDCG
        }
    }
    Fallback "Unlit"
}