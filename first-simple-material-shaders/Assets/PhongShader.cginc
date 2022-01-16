#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

struct MeshData {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 uv0 : TEXCOORD0;
};

struct Interpolator {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 wPos : TEXCOORD1;
    float3 normal : TEXCOORD2;
    float3 tangent : TEXCOORD3;
    float3 bitangent : TEXCOORD4;
    LIGHTING_COORDS(5, 6) // use texcoords 3 and 4 for lighting
};

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _Color;
sampler2D _NormalMap;
fixed _NormalStr;
sampler2D _HeightMap;
fixed _HeightStr;
sampler2D _AmbOccMap;
fixed _AmbOccStr;
sampler2D _RoughMap;
fixed _RoughStr;
fixed _AmbCoef;
fixed _DifCoef;
fixed _SpecCoef;
fixed _SpecGlos;

fixed3 phongLight(Interpolator i) {
    // ambient
    float ambientOcclusion = lerp(1, tex2D(_AmbOccMap, i.uv).x, _AmbOccStr);
    fixed3 ambient = _AmbCoef * ambientOcclusion * unity_AmbientSky;

    // diffuse
    float3 L = normalize(UnityWorldSpaceLightDir(i.wPos));
    float attenuation = LIGHT_ATTENUATION(i);
    float3 tangentSpaceNormal = normalize(lerp(float3(0, 0, 1), UnpackNormal(tex2D(_NormalMap, i.uv)), _NormalStr));
    float3x3 tangentToWorld = {
        i.tangent.x, i.bitangent.x, i.normal.x,
        i.tangent.y, i.bitangent.y, i.normal.y,
        i.tangent.z, i.bitangent.z, i.normal.z
    };
    float3 N = mul(tangentToWorld, tangentSpaceNormal);
    //float3 N = UnityObjectToWorldNormal(i.normal.xyz);
    float LdotN = max(0, dot(L, N));
    fixed3 diffuse = _DifCoef * attenuation * LdotN * _LightColor0.xyz;

    // specular
    float3 V = normalize(_WorldSpaceCameraPos - i.wPos);
    float roughness = tex2D(_RoughMap, i.uv).x;
    _SpecGlos = lerp(_SpecGlos, max(0, min(10, 1 / roughness)), _RoughStr);
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

    i.uv = TRANSFORM_TEX(v.uv0, _MainTex);

    float height = tex2Dlod(_HeightMap, float4(i.uv, 0, 0)).x * 2 - 1;
    v.vertex.xyz += v.normal * height * _HeightStr * 0.1;

    i.pos = UnityObjectToClipPos(v.vertex);
    i.wPos = mul(unity_ObjectToWorld, v.vertex);

    i.normal = UnityObjectToWorldNormal(v.normal);
    i.tangent = UnityObjectToWorldDir(v.tangent.xyz);
    i.bitangent = cross(i.normal, i.tangent) * v.tangent.w * unity_WorldTransformParams.w;

    TRANSFER_VERTEX_TO_FRAGMENT(i); // transfer lighting
    return i;
}

fixed4 frag(Interpolator i) : SV_TARGET {
    fixed4 texel = tex2D(_MainTex, i.uv);
    return texel * _Color * fixed4(phongLight(i), 0);
}