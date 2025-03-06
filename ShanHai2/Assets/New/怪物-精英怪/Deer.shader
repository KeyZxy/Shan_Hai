Shader "Unlit/Deer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Emission("Emission",2D) = "white" {}
        _Normal("Normal",2D) = "white" {}
        _AmbientIntenisty("ambient intensity",Range(0,1)) = 1
    }
    SubShader
    {
        
        Cull Off
        Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            //#pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 worldPos : TEXCOORD5;
                SHADOW_COORDS(4)
            };

            sampler2D _MainTex,_Emission,_Normal;
            float4 _MainTex_ST,_Emission_ST,_Normal_ST;
            float _AmbientIntenisty;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.tangent = v.tangent;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 base = tex2D(_MainTex, i.uv);
                //计算tbn矩阵
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                // 计算世界空间中的切线、副切线和法线

                float3 worldTangent = UnityObjectToWorldDir(i.tangent.xyz);
                float3 worldBitangent = cross(worldNormal, worldTangent);

                // 构建 TBN 矩阵
                float3 tspace0 = float3(worldTangent.x, worldBitangent.x, worldNormal.x);
                float3 tspace1 = float3(worldTangent.y, worldBitangent.y, worldNormal.y);
                float3 tspace2 = float3(worldTangent.z, worldBitangent.z, worldNormal.z);
                
                float4 normalColor = tex2D(_Normal,i.uv);
                float3 normalDir = UnpackNormal(normalColor);

                float3 mappedNormal;
                mappedNormal.x = dot(tspace0, normalDir);
                mappedNormal.y = dot(tspace1, normalDir);
                mappedNormal.z = dot(tspace2, normalDir);
                mappedNormal = normalize(mappedNormal);

                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                
                float diffuse = dot(mappedNormal,_WorldSpaceLightPos0) * 0.5 + 0.5;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed shadow =SHADOW_ATTENUATION(i); 

                float4 emission = tex2D(_Emission,i.uv);
                // apply fog
                
                
                float4 col = base * diffuse;
                col += emission;
                col *= atten;
                col.xyz += ambient * _AmbientIntenisty * base;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
                return float4(ambient,1);
            }
            ENDCG
        }

        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_fwdbase
            //#pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 worldPos : TEXCOORD5;
                SHADOW_COORDS(4)
            };

            sampler2D _MainTex,_Emission,_Normal;
            float4 _MainTex_ST,_Emission_ST,_Normal_ST;
            float _AmbientIntenisty;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.tangent = v.tangent;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //计算tbn矩阵
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                // 计算世界空间中的切线、副切线和法线

                float3 worldTangent = UnityObjectToWorldDir(i.tangent.xyz);
                float3 worldBitangent = cross(worldNormal, worldTangent);

                // 构建 TBN 矩阵
                float3 tspace0 = float3(worldTangent.x, worldBitangent.x, worldNormal.x);
                float3 tspace1 = float3(worldTangent.y, worldBitangent.y, worldNormal.y);
                float3 tspace2 = float3(worldTangent.z, worldBitangent.z, worldNormal.z);
                
                float4 normalColor = tex2D(_Normal,i.uv);
                float3 normalDir = UnpackNormal(normalColor);

                float3 mappedNormal;
                mappedNormal.x = dot(tspace0, normalDir);
                mappedNormal.y = dot(tspace1, normalDir);
                mappedNormal.z = dot(tspace2, normalDir);
                mappedNormal = normalize(mappedNormal);

                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                //fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                
                float diffuse = dot(mappedNormal,_WorldSpaceLightPos0) * 0.5 + 0.5;

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed shadow =SHADOW_ATTENUATION(i); 

                float4 emission = tex2D(_Emission,i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col *= diffuse;
                col += emission;
                col *= atten;
                col.xyz += ambient * _AmbientIntenisty;
                return col;
                return float4(ambient,1);
            }
            ENDCG
        }
    }
}

