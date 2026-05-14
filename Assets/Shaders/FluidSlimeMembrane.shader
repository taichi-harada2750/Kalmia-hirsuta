Shader "Custom/FluidSlimeMembrane"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Slime Color", Color) = (0.2, 0.8, 0.9, 1)
        _CoreA ("Core A Position (World)", Vector) = (0, 0, 0, 0)
        _CoreB ("Core B Position (World)", Vector) = (0, 0, 0, 0)
        _RadiusA ("Radius A", Float) = 1.0
        _RadiusB ("Radius B", Float) = 1.0
        _SminFactor ("Smoothness", Float) = 0.5
        _OutlineColor ("Outline Color", Color) = (0.1, 0.5, 0.6, 1)
        _OutlineWidth ("Outline Width", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _CoreA;
            float4 _CoreB;
            float _RadiusA;
            float _RadiusB;
            float _SminFactor;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            // Polynomial smooth minimum (from Inigo Quilez)
            float smin(float a, float b, float k)
            {
                float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
                return lerp(b, a, h) - k * h * (1.0 - h);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // We assume 2D plane (XY), ignoring Z
                float2 pos2D = i.worldPos.xy;
                float2 coreA2D = _CoreA.xy;
                float2 coreB2D = _CoreB.xy;

                // Distance to each core
                float distA = length(pos2D - coreA2D) - _RadiusA;
                float distB = length(pos2D - coreB2D) - _RadiusB;

                // Smooth minimum of the two distances
                float d = smin(distA, distB, _SminFactor);

                // Anti-aliasing / smoothing the edge
                float aa = fwidth(d); // Smoothness based on screen-space derivative

                // If d < 0, we are inside the slime
                // Using smoothstep for an anti-aliased edge
                float alpha = smoothstep(aa, -aa, d);
                
                // Outline
                float outline = smoothstep(-_OutlineWidth + aa, -_OutlineWidth - aa, d);

                fixed4 finalColor = lerp(_OutlineColor, _Color, outline);
                finalColor.a *= alpha;

                return finalColor;
            }
            ENDCG
        }
    }
}
