Shader "Sprites/JitterFreeUnlitCombined"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
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
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _RendererColor;
            float4 _MainTex_ST;
            float2 _Flip;
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _AlphaTex;
            float _EnableExternalAlpha;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                float2 flip = _Flip.xy;
                float2 texcoord = IN.texcoord;
                texcoord = lerp(texcoord, 1.0 - texcoord, 0.5 * (1.0 - flip));
                OUT.texcoord = texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif
                return OUT;
            }

            // JitterFree sampling
            float4 texturePointSmooth(sampler2D tex, float2 uvs)
            {
                float2 size = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
                float2 pixel = abs(1.0 / size);

                uvs -= pixel * 0.5;
                float2 uv_pixels = uvs * size;
                float2 delta_pixel = frac(uv_pixels) - 0.5;

                float2 ddxy = fwidth(uv_pixels);
                float2 mip = log2(ddxy) - 0.5;

                float2 smoothUV = uvs + (clamp(delta_pixel / ddxy, -0.5, 0.5) - delta_pixel) * pixel;

                return tex2Dlod(tex, float4(smoothUV, 0, min(mip.x, mip.y)));
            }

            fixed4 SampleSpriteTexture(float2 uv)
            {
                #if defined(ETC1_EXTERNAL_ALPHA)
                    fixed4 c = texturePointSmooth(_MainTex, uv);
                    fixed4 alpha = tex2D(_AlphaTex, uv);
                    c.a = lerp(c.a, alpha.r, _EnableExternalAlpha);
                    return c;
                #else
                    return texturePointSmooth(_MainTex, uv);
                #endif
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}