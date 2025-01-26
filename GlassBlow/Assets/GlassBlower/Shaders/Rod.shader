Shader "Unlit/Rod"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RotationTex ("Rotation Texture", 2D) = "white" {}

        _RotationSpeed ("Rotation Speed", Float) = 1
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
            };

            sampler2D _MainTex;
            sampler2D _RotationTex;

            float4 _MainTex_ST;
            float _RotationSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 main_tex = tex2D(_MainTex, i.uv);

                float2 rotation_uv = float2(i.uv.x, i.uv.y + _Time.y * _RotationSpeed);
                fixed4 rotation_tex = tex2D(_RotationTex, rotation_uv);

                fixed4 color = main_tex;
                color *= i.color;
                color.rgb *= rotation_tex;
                color.rgb *= color.a;
                return color;
            }
            ENDCG
        }
    }
}