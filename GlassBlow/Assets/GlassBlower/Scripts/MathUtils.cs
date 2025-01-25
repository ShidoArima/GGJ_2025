using UnityEngine;

namespace GlassBlower.Scripts
{
    public static class MathUtils
    {
        public static float SmoothStep(float a, float b, float x)
        {
            float t = Mathf.Clamp01((x - a) / (b - a));
            return t * t * (3f - (2 * t));
        }
    }
}