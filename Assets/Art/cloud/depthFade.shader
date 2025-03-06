Shader "Custom/DepthFadeEffect" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _AlphaScale ("Alpha Scale", Range(0, 1)) = 0.5
        _EdgeSmoothness ("Edge Smoothness", Range(0.001, 5)) = 1.0
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _AlphaScale;
            float _EdgeSmoothness;
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.pos);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // 采样纹理和颜色
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // 获取深度信息
                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                float objectDepth = i.screenPos.w; // 当前物体的深度
                
                // 计算深度差值
                float depthDifference = sceneDepth - objectDepth;
                float depthFade = saturate(depthDifference * _EdgeSmoothness);
                
                // 组合最终透明度
                col.a *= depthFade * _AlphaScale;
                
                // 应用雾效
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}