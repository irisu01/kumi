#include "sh_Utils.h"

layout(location = 0) in lowp vec4 v_Colour;
layout(location = 1) in mediump vec2 v_TexCoord;

layout(set = 0, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 0, binding = 1) uniform lowp sampler m_Sampler;

layout(std140, set = 1, binding = 0) uniform m_AnimationData {
    lowp float progress;
};

layout(location = 0) out lowp vec4 o_Colour;

void main() {
    float one = g_BackbufferDraw ? 1 : 0;
    
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord, -0.9) * one;
    o_Colour = colour.r < progress ? vec4(v_Colour.rgb, v_Colour.a * colour.a) : vec4(0);
}