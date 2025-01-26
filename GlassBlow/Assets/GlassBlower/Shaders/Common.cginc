#ifndef AMBIENT_INCLUDED
#define AMBIENT_INCLUDED

#include <HLSLSupport.cginc>
#include "UnityCG.cginc"

float4 _UTime;

// Gradient noise from Jorge Jimenez's presentation:
// http://www.iryoku.com/next-generation-post-processing-in-call-of-duty-advanced-warfare
float GradientNoise(float2 uv)
{
    uv = floor(uv * _ScreenParams.xy + _Time.x);
    float f = dot(float2(0.06711056, 0.00583715), uv);
    return frac(52.9829189 * frac(f));
}

#define ADD_GRADIENT_NOISE(c, uv) c += (1.0 / 255.0) * GradientNoise(uv) - (0.5 / 255.0);

#if defined(AMBIENT_ENABLED)

fixed4 _Ambient;

fixed4 AddAmbient(fixed4 color)
{
    return color * fixed4(_Ambient.rgb, 1);
}

#define ADD_AMBIENT(color) color = AddAmbient(color);

#else

#define ADD_AMBIENT(color)

#endif

#endif
