Shader "Unlit/Ambient/Fog"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _WaveParams1 ("Wave Params 1", Vector) = (1,1,1,1)
        _WaveParams2 ("Wave Params 2", Vector) = (1,1,1,1)
        _NoiseParams ("Noise Params", Vector) = (1,1,1,1)
        _Offset ("Wave Offset", Vector) = (1,1,1,1)
        [Toggle(AMBIENT_ENABLED)] _AmbientEnabled ("Ambient Enabled", Int) = 1
        [Toggle(HORIZONTAL_SAMPLING)] _HorizontalSampling ("Horizontal Sampling", Int) = 0
        [Toggle(INVERSE_UV)] _InverseUv ("Inverse UV", Int) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma shader_feature _ AMBIENT_ENABLED
            #pragma shader_feature _ HORIZONTAL_SAMPLING
            #pragma shader_feature _ INVERSE_UV

            #include "UnityCG.cginc"
            #include "Simplex.cginc"
            #include "Common.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _Color;
            float4 _WaveParams1;
            float4 _WaveParams2;
            float4 _NoiseParams;
            float4 _Offset;

            #if defined(INVERSE_UV)
            #define INVERSE(sample) (1 - sample)
            #else
            #define INVERSE(sample) sample
            #endif

            #if defined(HORIZONTAL_SAMPLING)
            #define GET_UV(uv) INVERSE(float2(uv.y, uv.x))
            #else
            #define GET_UV(uv) INVERSE(uv)
            #endif

            float GetFog(float2 position)
            {
                float2 scaledPosition = _Offset.zw * position;
                float x0 = scaledPosition.x + snoise(float3(scaledPosition.xy, 0) * _NoiseParams.x + _Time.x * _WaveParams1.w) * _NoiseParams.y;
                float x1 = scaledPosition.x + snoise(float3(scaledPosition.xy, 0) * _NoiseParams.z + _Time.x * _WaveParams2.w) * _NoiseParams.w;

                float sin0 = sin(x0 * _WaveParams1.y + _Time.z * _WaveParams1.x + _Offset.x);
                float sin1 = sin(x1 * _WaveParams2.y + _Time.z * _WaveParams2.x + _Offset.x);

                float wind = (sin0 + 1) * 0.5f;
                return wind * _WaveParams1.z + sin1 * _WaveParams2.z + _Offset.y;
            }

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                const float4 world_position = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
                o.vertex = mul(UNITY_MATRIX_VP, world_position);
                o.worldPos = world_position;
                o.texcoord = v.texcoord;

                o.color = _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = GET_UV(i.texcoord);

                float fog = GetFog(uv);
                float fog_multiplier = 1 - smoothstep(0, fog, uv.y);

                // sample the texture
                fixed4 c = i.color;
                c.a *= fog_multiplier;
                c.rgb *= c.a;

                ADD_GRADIENT_NOISE(c, i.texcoord)

                return c;
            }
            ENDCG
        }
    }
}