Shader "Custom/UnlitGlowUI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _GlowColor ("Glow Color", Color) = (0.5, 0.8, 1, 1)
        _GlowStrength ("Glow Strength", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Lighting Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _GlowColor;
            float _GlowStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenUV = o.uv * 2 - 1; // for fake fresnel
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                float alpha = tex.a * _Color.a;

                // Fake glow based on distance from center (radial fade)
                float2 centered = i.screenUV;
                float dist = length(centered);
                float glow = smoothstep(0.7, 0.3, dist) * _GlowStrength;

                fixed4 col = tex * _Color;
                col.rgb += _GlowColor.rgb * glow * alpha;
                col.a = alpha;

                return col;
            }
            ENDCG
        }
    }
}
