Shader "Custom/Test" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurRadius ("Blur Radius", Range(0, 10)) = 1
    }

    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _BlurRadius;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = fixed4(0, 0, 0, 0);

                // Blur in x-axis
                for (float j = -_BlurRadius; j <= _BlurRadius; j += 0.1) {
                    col += tex2D(_MainTex, i.uv + float2(j / _ScreenParams.x, 0));
                }

                // Blur in y-axis
                for (float j = -_BlurRadius; j <= _BlurRadius; j += 0.1) {
                    col += tex2D(_MainTex, i.uv + float2(0, j / _ScreenParams.y));
                }

                col /= (_BlurRadius * 2.0 + 1.0) * (_BlurRadius * 2.0 + 1.0);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
