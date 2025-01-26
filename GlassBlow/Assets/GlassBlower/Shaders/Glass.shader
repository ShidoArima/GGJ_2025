Shader "Unlit/Glass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RotationTex ("Rotation Texture", 2D) = "white" {}
        _Mask ("Mask", 2D) = "white" {}

        _RotationSpeed ("Rotation Speed", Float) = 1
        _ColdColor ("Heat Color", Color) = (1, 1, 1, 1)
        _HeatColor ("Heat Color", Color) = (1, 1, 1, 1)
        _ExtremeColor ("Extreme Color", Color) = (1, 1, 1, 1)
        _ExpandColor ("Expand Color", Color) = (1, 1, 1, 1)
        _HeatPhase ("Heat Phase", Range(0, 1)) = 0
        _HeatParams ("Heat Params", Vector) = (1, 1, 1, 1)
        _HeatParams2 ("Heat Params", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
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

            #include "UnityCG.cginc"
            #include "Simplex.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _RotationTex;
            sampler2D _Mask;

            float4 _MainTex_ST;
            float _RotationSpeed;
            float4 _ColdColor;
            float4 _HeatColor;
            float4 _ExpandColor;
            float4 _ExtremeColor;

            //Position and radius
            float3 _ExpandPosition;
            float _ExpandRadius;

            float4 _HeatParams;
            float4 _HeatParams2;
            float _HeatPhase;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 main_tex = tex2D(_MainTex, i.uv);
                fixed mask = tex2D(_Mask, i.uv).a;

                float2 rotation_uv = float2(i.uv.x, i.uv.y + _Time.y * _RotationSpeed);
                fixed4 rotation_tex = tex2D(_RotationTex, rotation_uv);

                float noise = snoise(float3(i.uv.xy, 0) * _HeatParams.y + _Time.x * _HeatParams.z) * _HeatParams.w;
                float noise2 = snoise(float3(i.uv.xy, 0) * _HeatParams2.y + _Time.x * _HeatParams2.z) * _HeatParams2.w;

                float heatV = 1 - abs((i.uv.y - 0.5f) * 2);
                float multiplier = saturate(heatV * _HeatParams.x + noise);
                float4 extremeColor = lerp(_HeatColor, _ExtremeColor, heatV * _HeatParams2.x + noise2);
                float4 heatColor = lerp(_ColdColor, extremeColor, multiplier * _HeatPhase);

                float2 diff = i.worldPos - _ExpandPosition;
                float expandPhase = 1 - smoothstep(0, _ExpandRadius, length(diff));
                float4 expandColor = lerp(float4(0, 0, 0, 1), _ExpandColor, expandPhase);

                fixed4 color = main_tex;
                color.rgb *= heatColor.rgb + expandColor.rgb;
                color = saturate(color);
                color *= i.color + rotation_tex;
                color.a = lerp(color.a, 1, _HeatPhase) * mask * expandColor.a;
                color.rgb *= color.a;
                return color;
            }
            ENDCG
        }
    }
}