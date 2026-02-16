Shader "Custom/FogOfWar"
{
    Properties
    {
        _FogTex ("Fog Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1)
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _FogTex;
            float4 _FogColor;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float fog = tex2D(_FogTex, i.uv).r;

                // fog = 0   - fully opaque
                // fog = 0.5 - semi-transparent
                // fog = 1   - invisible
                float alpha = 1.0 - fog;

                return fixed4(_FogColor.rgb, alpha);
            }
            ENDCG
        }
    }
}
