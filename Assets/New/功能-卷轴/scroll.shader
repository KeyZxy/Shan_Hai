Shader "Unlit/scroll"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackTex("backTex",2D) = "white" {}
    }
    SubShader
    {
        
        //Cull Off
        //Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque" }
        //LOD 100

        Pass
        {
            Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque" }
        LOD 100
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 base = tex2D(_MainTex, i.uv);
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT;
                //half3 ambient = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed shadow =SHADOW_ATTENUATION(i); 
                float diffuse = saturate(dot(-worldNormal,_WorldSpaceLightPos0));
                // apply fog
                
                float4 col = base * diffuse;
                col *= shadow;
                col.xyz += ambient * base;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        

        Pass
        {
            Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque" }
            LOD 100
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            #pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex, _BackTex;
            float4 _MainTex_ST, _BackTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 base = tex2D(_BackTex, i.uv);
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT;
                //half3 ambient = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed shadow =SHADOW_ATTENUATION(i); 
                float diffuse = saturate(dot(worldNormal,_WorldSpaceLightPos0));
                // apply fog
                
                float4 col = base * diffuse;
                col *= shadow;
                col.xyz += ambient * base;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
        Pass
        {
            
            Tags { "LightMode" = "ShadowCaster" }
            Cull  Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                float diffuse = dot(worldNormal,_WorldSpaceLightPos0) * 0.5 + 0.5;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col *= diffuse;
                col *= atten;
                return col;
            }
            ENDCG
        }

    }
}
