Shader "Unlit/ShaderEffect"
{
    Properties
    {
        _Color ("Main", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _ErrorTex ("Texture", 2D) = "white" {}
        _Show ("Show", Int) = 100
        _Error ("Error", Int) = 50
    }
    SubShader
    {
        Pass 
        {
            Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "LightMode"="ForwardBase" }

            ZWrite Off
            // 开启混合模式，并设置混合因子为SrcAlpha和OneMinusSrcAlpha
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _ErrorTex;
            float4 _ErrorTex_ST;
            
            float4 _Color;
            float _Show;
            float _Error;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                // sample the texture
                if (i.vertex.y >= _Show + _Error) {
                    //clip(-1);
                    col = fixed4(0, 0, 0, 0);
                } else if (i.vertex.y > _Show) {
                    float ErrorValue = (_Error + _Show - i.vertex.y) / _Error;
                    float Layer = 4 - floor(ErrorValue * 3);
                    col = tex2D(_ErrorTex, (i.uv * Layer) % 1) * float4(_Color.xyz, ErrorValue);
                } else {
                    col = tex2D(_MainTex, i.uv) * _Color;
                }
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            
            ENDCG
        }
    }
}
