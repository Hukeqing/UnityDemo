Shader "Unlit/ShaderEffect"
{
    Properties
    {
        _Color ("Main", Color) = (1, 1, 1, 1)
        _Slider ("Slider", Range(0, 1000)) = 200
    }
    SubShader
    {
        Pass 
        {
            Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "LightMode"="ForwardBase" }
            
            ZWrite Off
            // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
            Blend SrcAlpha OneMinusSrcAlpha

            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;
            fixed _Slider;
            struct appdata
            {
                float4 vertex : POSITION;   
                float2 uv : TEXCOORD;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert (appdata i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                return o;
            }
            
            fixed4 checker(float2 uv)
            {
                float v = (floor(uv * 10).x + floor(uv * 10).y) % 2;
                float4 checker = float4(v, v, v, 1);
                return checker;
            }
            
            fixed4 frag (v2f i) : SV_TARGET
            {
                if (i.pos.y > _Slider)
                    return checker(i.uv);
                return fixed4(0, 0, 0, 0);
            }
            
            ENDCG
        }
    }
}
