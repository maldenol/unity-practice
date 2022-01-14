#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct MeshData {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv0 : TEXCOORD0;
};

struct Interpolator {
    float4 pos : SV_POSITION;
    float3 wPos : TEXCOORD1;
    float3 normal : TEXCOORD2;
    float2 uv : TEXCOORD0;
    LIGHTING_COORDS(3, 4) // use texcoords 3 and 4 for lighting
};

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _Color;
fixed _AmbCoef;
fixed _DifCoef;
fixed _SpecCoef;
fixed _SpecGlos;

fixed3 phongLight(Interpolator i) {
    // ambient
    fixed3 ambient = _AmbCoef * unity_AmbientSky;

    // diffuse
    float3 L = normalize(UnityWorldSpaceLightDir(i.wPos));
    float attenuation = LIGHT_ATTENUATION(i);
    float3 N = UnityObjectToWorldNormal(i.normal.xyz);
    float LdotN = max(0, dot(L, N));
    fixed3 diffuse = _DifCoef * attenuation * LdotN * _LightColor0.xyz;

    // specular
    float3 V = normalize(_WorldSpaceCameraPos - i.wPos);
    float specularExponent = exp2(_SpecGlos);
    // Phong
    float3 R = 2 * LdotN * N - L;
    fixed specular = _SpecCoef * attenuation * _SpecGlos * pow(_LightColor0.xyz * max(0, dot(V, R)), specularExponent);
    // Blinn-Phong
    //float3 H = normalize(L + V);
    //fixed3 specular = _SpecCoef * pow(_LightColor0.xyz * max(0, dot(H, N)), specularExponent) * _SpecGlos;

    // fresnel
    //fixed3 fresnel = pow(1 - max(0, dot(V, N)), 2);

    return ambient + diffuse + specular;
}

Interpolator vert(MeshData v) {
    Interpolator i;
    i.pos = UnityObjectToClipPos(v.vertex);
    i.wPos = mul(unity_ObjectToWorld, v.vertex);
    i.normal = UnityObjectToWorldNormal(v.normal);
    i.uv = TRANSFORM_TEX(v.uv0, _MainTex);
    TRANSFER_VERTEX_TO_FRAGMENT(i); // transfer lighting
    return i;
}

fixed4 frag(Interpolator i) : SV_TARGET {
    fixed4 texel = tex2D(_MainTex, -i.uv);
    return texel * _Color * fixed4(phongLight(i), 1);
}