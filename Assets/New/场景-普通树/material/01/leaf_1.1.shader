Shader "Unlit/leaf"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GrassColor("grass color",COLOR) = (1,1,1,1)

        _AmbinetIntensity("ambinet intensity",range(0,1)) = 1
        _ShadowIntensity("shadow intensity",Range(0,1)) = 1
        
        _LambertPower("lambert power",Range(0,10)) =1

        _FresnelPow("fresnel power",range(0,100)) = 1
        _FrensnelColor("fresnel color",COLOR) = (1,1,1,1)
        _RimDense("rim dense",range(0,1)) = 0.5
        
        _MovingNoise("moving noise",2D) = "white" {}
        _NoiseTiling("noise tiling",FLOAT) = 1
        _NoisePower("noise power",Range(0,10)) =1
        
        _Strength("strength",range(0,100)) = 1
        _Speed("speed",range(0,10)) =1 
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
                //float test : TEXCOORD5;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 posWS : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex,_MovingNoise;
            float4 _MainTex_ST,_MovingNoise_ST,_GrassColor,_FrensnelColor;
            float _AmbinetIntensity,_FresnelPow,_RimDense,_ShadowIntensity,_LambertPower,_Speed,_Strength,_NoiseTiling;
            float _NoisePower;

            v2f vert (appdata v)
            {
                v2f o;

                //float3 worldPos = UnityObjectToWorldDir(v.pos);
                float noise = tex2Dlod(_MovingNoise,float4(v.pos.xz * _NoiseTiling,0,0)).x;
                noise = pow(noise,_NoisePower);
                
                
                float stage1 = dot(v.pos, float3(0, 1, 0)) * _Strength;
                float stage2 = sin(dot(v.pos, float3(1, 0, 0)) * _Strength + _Time.y * _Speed);
                float3 stage3 = stage1 * stage2 * float3(0.001, 0, 0.001);                
                o.pos = UnityObjectToClipPos(v.pos + stage3 * noise);
                //o.test =  noise;
                //o.pos = UnityObjectToClipPos(v.pos);
                o.normal = v.normal;
                o.posWS = mul(unity_ObjectToWorld, v.pos) ;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                float lambertTerm = saturate(dot(_WorldSpaceLightPos0,worldNormal) * 0.5 + 0.5) ;
                lambertTerm = pow(lambertTerm,_LambertPower);
               
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                
                // sample the texture
                fixed4 oricol = tex2D(_MainTex, i.uv);
                ambient *= _GrassColor.xyz;
                ambient *= _AmbinetIntensity;
                
                clip(oricol.a - 0.1);
                // apply fog
                
                float4 col = oricol * _GrassColor;
                col.xyz *= lambertTerm;
                fixed shadow =SHADOW_ATTENUATION(i);
                float shadowFactor = 1 - _ShadowIntensity;
                //lambertTerm *= shadow;
                col.xyz = lerp(col.xyz * shadowFactor,col,shadow);
                //col.xyz *= shadow;
                //???alphatest
                
                col.xyz += ambient;
                //col *= _GrassColor;

                
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWS);
                
                
                //??????��?
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
                float fresnelValue = pow(1 - saturate(dot(worldNormal,viewDir)),_FresnelPow);
                float fresnelMask = smoothstep(_RimDense,1,fresnelValue);
                float3 fresnelTerm =fresnelMask * _FrensnelColor.xyz * oricol.xyz;
                //fresnelTerm *= shadow;
                fresnelTerm = lerp(fresnelTerm * shadowFactor,fresnelTerm,shadow);
                float4 finalColor = float4(col.xyz + fresnelTerm,1);
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                //return float4(i.test,i.test,i.test,1);
                //return float4(lambertTerm,lambertTerm,lambertTerm,1);
                //return float4(fresnelValue,fresnelValue,fresnelValue,1);
                return finalColor;
            }
            ENDCG
        }

Pass
        {
            Tags {"LightMode" = "ForwardAdd"}
            Blend One One
            ZWrite off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            // make fog work
            #pragma multi_compile_fwdadd 
            #pragma multi_compile_fog
            //#pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

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
                //float test : TEXCOORD5;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 posWS : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex,_MovingNoise;
            float4 _MainTex_ST,_MovingNoise_ST,_GrassColor,_FrensnelColor;
            float _AmbinetIntensity,_FresnelPow,_RimDense,_ShadowIntensity,_LambertPower,_Speed,_Strength,_NoiseTiling;
            float _NoisePower;

            v2f vert (appdata v)
            {
                v2f o;

                //float3 worldPos = UnityObjectToWorldDir(v.pos);
                float noise = tex2Dlod(_MovingNoise,float4(v.pos.xz * _NoiseTiling,0,0)).x;
                noise = pow(noise,_NoisePower);
                
                
                float stage1 = dot(v.pos, float3(0, 1, 0)) * _Strength;
                float stage2 = sin(dot(v.pos, float3(1, 0, 0)) * _Strength + _Time.y * _Speed);
                float3 stage3 = stage1 * stage2 * float3(0.001, 0, 0.001);                
                o.pos = UnityObjectToClipPos(v.pos + stage3 * noise);
                //o.test =  noise;
                //o.pos = UnityObjectToClipPos(v.pos);
                o.normal = v.normal;
                o.posWS = mul(unity_ObjectToWorld, v.pos) ;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef USING_DIRECTIONAL_LIGHT  //ƽ�й��¿���ֱ�ӻ�ȡ����ռ��µĹ��շ���
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				#else//������Դ��_WorldSpaceLightPos0������Դ���������꣬�붥��������������������ɵõ�����ռ��µĹ��շ���
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - i.posWS.xyz);
				#endif
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                float lambertTerm = saturate(dot(worldLightDir,worldNormal) * 0.5 + 0.5) ;
                lambertTerm = pow(lambertTerm,_LambertPower);

                #ifdef USING_DIRECTIONAL_LIGHT  //ƽ�й��²����ڹ���˥������ֵΪ1
					fixed atten = 1.0;
				#else
					#if defined (POINT)    //���Դ�Ĺ���˥������
						//unity_WorldToLight���þ�������ռ䵽��Դ�ռ�任�����붥�������������˿ɵõ���Դ�ռ��µĶ�������
						float3 lightCoord = mul(unity_WorldToLight, float4(i.posWS, 1)).xyz;
						//����Unity���ú���tex2D��Unity��������_LightTexture0�����������������Դ˥������ȡ��˥��������
						//��ͨ��UNITY_ATTEN_CHANNEL�õ�˥��������˥��ֵ���ڵķ������Եõ����յ�˥��ֵ
						fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#elif defined (SPOT)   //�۹�ƵĹ���˥������
						float4 lightCoord = mul(unity_WorldToLight, float4(i.posWS, 1));
						//(lightCoord.z > 0)���۹�Ƶ����ֵС�ڵ���0ʱ�������˥��Ϊ0
						//_LightTextureB0������ù�Դʹ����cookie����˥������������Ϊ_LightTextureB0
					fixed atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#else
						fixed atten = 1.0;
					#endif
				#endif
               lambertTerm *= atten;
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                
                // sample the texture
                fixed4 oricol = tex2D(_MainTex, i.uv);
                //ambient *= _GrassColor.xyz;
                //ambient *= _AmbinetIntensity;
                
                clip(oricol.a - 0.1);
                // apply fog
                
                float4 col = oricol * _GrassColor;
                col.xyz *= lambertTerm;
                //fixed shadow =SHADOW_ATTENUATION(i);
                float shadowFactor = 1 - _ShadowIntensity;
                //lambertTerm *= shadow;
                //col.xyz = lerp(col.xyz * shadowFactor,col,shadow);
                //col.xyz *= shadow;
                //???alphatest
                
                //col.xyz += ambient;
                //col *= _GrassColor;

                
                //UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWS);
                
                
                //??????��?
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
                float fresnelValue = pow(1 - saturate(dot(worldNormal,viewDir)),_FresnelPow);
                float fresnelMask = smoothstep(_RimDense,1,fresnelValue);
                float3 fresnelTerm =fresnelMask * _FrensnelColor.xyz * oricol.xyz;
                fresnelTerm *= atten;
                //fresnelTerm *= shadow;
                //fresnelTerm = lerp(fresnelTerm * shadowFactor,fresnelTerm,shadow);
                float4 finalColor = float4(col.xyz + fresnelTerm,1);
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                //return float4(i.test,i.test,i.test,1);
                //return float4(lambertTerm,lambertTerm,lambertTerm,1);
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
                //float test : TEXCOORD5;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD2;
                float3 posWS : TEXCOORD4;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex,_MovingNoise;
            float4 _MainTex_ST,_MovingNoise_ST,_GrassColor,_FrensnelColor;
            float _AmbinetIntensity,_FresnelPow,_RimDense,_ShadowIntensity,_LambertPower,_Speed,_Strength,_NoiseTiling;
            float _NoisePower;

            v2f vert (appdata v)
            {
                v2f o;

                //float3 worldPos = UnityObjectToWorldDir(v.pos);
                float noise = tex2Dlod(_MovingNoise,float4(v.pos.xz * _NoiseTiling,0,0)).x;
                noise = pow(noise,_NoisePower);
                
                
                float stage1 = dot(v.pos, float3(0, 1, 0)) * _Strength;
                float stage2 = sin(dot(v.pos, float3(1, 0, 0)) * _Strength + _Time.y * _Speed);
                float3 stage3 = stage1 * stage2 * float3(0.001, 0, 0.001);                
                o.pos = UnityObjectToClipPos(v.pos + stage3 * noise);
                //o.test =  noise;
                //o.pos = UnityObjectToClipPos(v.pos);
                o.normal = v.normal;
                o.posWS = mul(unity_ObjectToWorld, v.pos) ;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                float3 worldNormal = UnityObjectToWorldNormal(i.normal);
                float lambertTerm = saturate(dot(_WorldSpaceLightPos0,worldNormal) * 0.5 + 0.5) ;
                lambertTerm = pow(lambertTerm,_LambertPower);
               
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                
                // sample the texture
                fixed4 oricol = tex2D(_MainTex, i.uv);
                ambient *= _GrassColor.xyz;
                ambient *= _AmbinetIntensity;
                
                clip(oricol.a - 0.1);
                // apply fog
                
                float4 col = oricol * _GrassColor;
                col.xyz *= lambertTerm;
                fixed shadow =SHADOW_ATTENUATION(i);
                float shadowFactor = 1 - _ShadowIntensity;
                //lambertTerm *= shadow;
                col.xyz = lerp(col.xyz * shadowFactor,col,shadow);
                //col.xyz *= shadow;
                //???alphatest
                
                col.xyz += ambient;
                //col *= _GrassColor;

                
                UNITY_LIGHT_ATTENUATION(attenuation, i, i.posWS);
                
                
                //??????��?
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWS.xyz);
                float fresnelValue = pow(1 - saturate(dot(worldNormal,viewDir)),_FresnelPow);
                float fresnelMask = smoothstep(_RimDense,1,fresnelValue);
                float3 fresnelTerm =fresnelMask * _FrensnelColor.xyz * oricol.xyz;
                //fresnelTerm *= shadow;
                fresnelTerm = lerp(fresnelTerm * shadowFactor,fresnelTerm,shadow);
                float4 finalColor = float4(col.xyz + fresnelTerm,1);
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                //return float4(i.test,i.test,i.test,1);
                //return float4(lambertTerm,lambertTerm,lambertTerm,1);
                //return float4(fresnelValue,fresnelValue,fresnelValue,1);
                return finalColor;
            }
            ENDCG
        }

       
    }
    
    
}
