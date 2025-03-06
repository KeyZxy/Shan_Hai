Shader "Unlit/bamboo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaMask("Alpha mask",2D) = "white" {}
        _GrassColor("grass color",COLOR) = (1,1,1,1)
        _NormalMap("normal map",2D) = "white" {}
        _AmbinetIntensity("ambinet intensity",range(0,1)) = 1

        _FresnelPow("fresnel power",range(0,100)) = 1
        _FrensnelColor("fresnel color",COLOR) = (1,1,1,1)
        _RimDense("rim dense",range(0,1)) = 0.5
    }
    SubShader
    {
       // Tags { "RenderType"="Opaque" }
        LOD 100
        //AlphaToMask On
        Cull Off
        ZWrite On
        //Blend SrcAlpha OneMinusSrcAlpha 
        //AlphaToMask On
        
        
        //Blend SrcAlpha OneMinusSrcAlpha 
        Pass
        {
            Tags {"LightMode" = "ForwardBase" "RenderType"="Opaque"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            // make fog work
            #pragma multi_compile_fwdbase 
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 posWS : TEXCOORD4;
                SHADOW_COORDS(3)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex,_NormalMap,_AlphaMask;
            float4 _MainTex_ST,_NormalMap_ST,_GrassColor,_FrensnelColor,_AlphaMask_ST;
            float _AmbinetIntensity,_FresnelPow,_RimDense;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //?????????
                UNITY_TRANSFER_INSTANCE_ID(v,o); //??????
                o.pos = UnityObjectToClipPos(v.pos);
                o.normal = v.normal;
                o.posWS = mul(unity_ObjectToWorld, v.pos) ;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i); 
                float3 givenNormal = tex2D(_NormalMap,i.uv);

                
                float3 worldNormal = UnityObjectToWorldNormal(givenNormal);
                float lambertTerm = dot(_WorldSpaceLightPos0,worldNormal) * 0.5 + 0.5 ;
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);

                
                

                // sample the texture
                fixed4 oricol = tex2D(_MainTex, i.uv);
                ambient *= oricol.xyz;
                ambient *= _AmbinetIntensity;
                float plantAlpha = tex2D(_AlphaMask,i.uv).r;
                clip(plantAlpha - 0.1);
                // apply fog
                
                float4 col = oricol;
                col.xyz *= lambertTerm;
                fixed shadow =SHADOW_ATTENUATION(i);

                col.xyz *= shadow;
                //???alphatest
                
                col.xyz += ambient;
                col *= _GrassColor;

                
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWS);
               
                //??????Ч?
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
                float fresnelValue = pow(1 - saturate(dot(worldNormal,viewDir)),_FresnelPow);
                float fresnelMask = smoothstep(_RimDense,1,fresnelValue);
                float3 fresnelTerm =fresnelMask * _FrensnelColor.xyz * oricol.xyz;
                fresnelTerm *= shadow;

               float4 finalColor = float4(col.xyz + fresnelTerm,1);
               UNITY_APPLY_FOG(i.fogCoord, finalColor);
               //return float4(fresnelValue,fresnelValue,fresnelValue,1);
               return finalColor;
            }
            ENDCG
        }

        Pass
        {
            Tags {"LightMode" = "ShadowCaster" "RenderType"="Opaque"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            // make fog work
            #pragma multi_compile_fwdbase 
            #pragma multi_compile_fog
            #pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 posWS : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex,_AlphaMask;
            float4 _MainTex_ST,_GrassColor,_AlphaMask_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.normal = v.normal;
                o.posWS = mul(unity_ObjectToWorld, v.pos) ;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                


                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                float lambertTerm = dot(_WorldSpaceLightPos0,worldNormal) * 0.5 + 0.5 ;
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                float plantAlpha = tex2D(_AlphaMask,i.uv).r;
                clip(plantAlpha - 0.1);
                //clip(col.a - 0.1);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col.xyz *= lambertTerm;
                fixed shadow =SHADOW_ATTENUATION(i);

                col.xyz *= shadow;
                //???alphatest
                col.xyz += ambient * col;
                col *= _GrassColor;

                
               UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWS);
               return float4(1,1,1,1);
               //return float4(attenuation,attenuation,attenuation,1);
               return float4(col.xyz,1);
            }
            ENDCG
        }

       
    }
    
    
}
