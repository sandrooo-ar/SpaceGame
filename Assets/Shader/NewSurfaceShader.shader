Shader "Custom/UnlitEmissiveTransparent"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
        _Emission ("Emission Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off        // don’t write to depth (prevents sorting issues with transparency)
        Blend SrcAlpha OneMinusSrcAlpha // standard alpha blending
        Cull Back
        Lighting Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float4 _Emission;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // Alpha comes from texture
                fixed alpha = texColor.a;

                // Add emissive effect
                fixed4 finalColor = texColor + _Emission;
                finalColor.a = alpha;

                return finalColor;
            }
            ENDCG
        }
    }
}
