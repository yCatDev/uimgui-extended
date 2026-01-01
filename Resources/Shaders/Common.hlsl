#ifndef DEARIMGUI_COMMON_INCLUDED
#define DEARIMGUI_COMMON_INCLUDED

struct ImVert   // same layout as ImDrawVert
{
    float2 vertex   : POSITION;
    float2 uv       : TEXCOORD0;
    uint   color    : TEXCOORD1; // gets reordered when using COLOR semantics

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 vertex   : SV_POSITION;
    float2 uv       : TEXCOORD0;
    half4  color    : COLOR;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

#endif
