Shader "Sprites/JitterFreeUnlit"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Blend One OneMinusSrcAlpha

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 texturePointSmooth(sampler2D tex, float2 uvs)
            {
                float2 size = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                float2 pixel = abs(1.0 / size);

                // No desplazamos las UVs, solo suavizamos en el espacio local
                float2 uv_pixels = uvs * size;
                float2 delta_pixel = frac(uv_pixels) - 0.5;

                float2 ddxy = fwidth(uv_pixels);
                float2 mip = log2(ddxy) - 0.5;

                float2 smoothUV = uvs + (clamp(delta_pixel / ddxy, -0.5, 0.5) - delta_pixel) * pixel;

                return tex2Dlod(tex, float4(smoothUV, 0, min(mip.x, mip.y)));
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = texturePointSmooth(_MainTex, IN.texcoord) * IN.color;
                return c;
            }

            ENDCG
        }
    }
}