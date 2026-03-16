Shader "Custom/SpriteBlur"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _BlurSize ("Blur Amount", Range(0, 0.005)) = 0.001
        _Color ("Tint", Color) = (1,1,1,1)
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

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float _BlurSize;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            // Thuật toán lấy mẫu 9 điểm xung quanh pixel gốc để tạo hiệu ứng nhòe
            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                fixed4 sum = fixed4(0.0, 0.0, 0.0, 0.0);
                float s = _BlurSize;

                sum += tex2D(_MainTex, float2(uv.x - s, uv.y - s)) * 0.077847;
                sum += tex2D(_MainTex, float2(uv.x, uv.y - s)) * 0.123317;
                sum += tex2D(_MainTex, float2(uv.x + s, uv.y - s)) * 0.077847;

                sum += tex2D(_MainTex, float2(uv.x - s, uv.y)) * 0.123317;
                sum += tex2D(_MainTex, float2(uv.x, uv.y)) * 0.195346;
                sum += tex2D(_MainTex, float2(uv.x + s, uv.y)) * 0.123317;

                sum += tex2D(_MainTex, float2(uv.x - s, uv.y + s)) * 0.077847;
                sum += tex2D(_MainTex, float2(uv.x, uv.y + s)) * 0.123317;
                sum += tex2D(_MainTex, float2(uv.x + s, uv.y + s)) * 0.077847;

                return sum * IN.color;
            }
            ENDCG
        }
    }
}