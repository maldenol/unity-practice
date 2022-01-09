Shader "Diniska/UnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SpecColor ("Specular color", Color) = (1., 1., 1., 1.)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _SpecColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, -i.uv);
                if (i.uv[0] > 0.5)
                    col *= _SpecColor;
                if (i.uv[1] > 0.5)
                    col /= _SpecColor;
                return col;
            }

            ENDCG
        }
    }
    Fallback "Unlit"
}