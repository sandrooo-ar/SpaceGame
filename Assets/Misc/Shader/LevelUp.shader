Shader "UI/LevelUpShockwaveSimple"
{
    Properties
    {
        _Tint("Tint Color", Color) = (1,0.8,0.2,1)
        _Intensity("Intensity", Range(0,1)) = 0
        _WaveRadius("Wave Radius", Range(0,2)) = 0
        _WaveWidth("Wave Width", Range(0.01,1)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Tint;
            float _Intensity;
            float _WaveRadius;
            float _WaveWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5,0.5);
                float dist = distance(i.uv, center);

                // soft radial ring
                float ring = smoothstep(_WaveRadius - _WaveWidth, _WaveRadius, dist) -
                             smoothstep(_WaveRadius, _WaveRadius + _WaveWidth, dist);

                fixed4 col = _Tint * ring * _Intensity;
                col.a = col.a;
                return col;
            }
            ENDCG //
        }
    }
}
