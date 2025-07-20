Shader "Custom/ScratchMaskold"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _MaskTex ("Mask (A)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade

        sampler2D _MainTex;
        sampler2D _MaskTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 mask = tex2D(_MaskTex, IN.uv_MainTex);
            col.a = 1.0 - mask.r; // maskが白い部分が透明になる
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
