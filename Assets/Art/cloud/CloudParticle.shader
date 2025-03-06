Shader "Unlit/CloudParticle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent+10" "PreviewType"="Plane"}
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        
         
        Pass
        {
            Cull Off  ZWrite Off
            ZTest On
			Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_particles 

            #include "UnityCG.cginc"
            #include"AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
                float4 color : COLOR;//粒子颜色
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 oriPos : TEXCOORD3;
                UNITY_FOG_COORDS(1)
                float4 color : COLOR;
                float4 pos : SV_POSITION;
                SHADOW_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 pivot;
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.oriPos = v.vertex;
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = mul(unity_ObjectToWorld,i.oriPos);
                float3 fakeNormal = normalize(worldPos - pivot);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed shadow =SHADOW_ATTENUATION(i);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                float diffuse = saturate(dot(fakeNormal,_WorldSpaceLightPos0));
                float3 finalColor = float3(1,1,1) * diffuse + ambient.xyz;
                
                return float4(finalColor,col.a * i.color.a);
            }
            ENDCG
        }
        Pass
        {
            ZWrite On
            ZTest On
			Tags {"LightMode" = "ShadowCaster"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_particles 

            #include "UnityCG.cginc"
            #include"AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;//粒子颜色
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 oriPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 color : COLOR;
                float4 pos : SV_POSITION;
                SHADOW_COORDS(2)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.oriPos = v.vertex;
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                // apply fog
                fixed shadow =SHADOW_ATTENUATION(i);
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                clip(col.a * i.color.a - 0.1);
                return float4(shadow,shadow,shadow,col.a * i.color.a);
            }
            ENDCG
        }
       
    }
}
