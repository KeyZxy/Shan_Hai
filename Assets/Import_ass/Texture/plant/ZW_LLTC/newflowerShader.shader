Shader "SH/plane/grass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Normal("normal",2D) = "white" {}
        _Color("color",COLOR) = (1,1,1,1)
        _TerrianTex("terrian tex",2D) = "white" {}
        _Xscale("xscale",FLOAT) = 1
        _Zscale("zscale",FLOAT) = 1
        _GroundFactor("ground factor",range(0,10)) = 5
        _WindDir("wind direction",Vector) = (1,0,0,0)
        _WindUnifromIntensity("wind uniform intensity",range(0,60)) = 30

        _ShadowIntensity("shadow intensity",range(0,1))  = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off
        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            //???????????
            //#pragma skip_variants DIRLIGHTMAP_COMBINED DYNAMICLIGHTMAP_ON LIGHTMAP_ON LIGHTMAP_SHADOW_MIXING LIGHTPROBE_SH SHADOWS_SHADOWMASK VERTEXLIGHT_ON
            //??????��?????????????
            #pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                SHADOW_COORDS(3)
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float4 origrin_vert1 :TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            float4 _Color;
            uniform float3 _camerapos;
            uniform float2 _camerasize;
            sampler2D _TerrianTex;
            float4 _TerrianTex_ST;
            float _Xscale,_Zscale,_GroundFactor,_WindUnifromIntensity,_ShadowIntensity;
            float3 _WindDir;
            sampler2D _MainTex,_Normal;
            float4 _MainTex_ST,_Normal_ST;

            v2f vert (appdata v)
            {

                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //?????????
                UNITY_TRANSFER_INSTANCE_ID(v,o); //??????
                //????�T????
                //float windIntensity = (sin(_Time.x *20) * 0.5 + 0.5);
                //float vertIntensity = _WindUnifromIntensity *pow( v.uv.y,2) * windIntensity;
                //vertIntensity = radians(vertIntensity);
                ////v.vertex.y *= cos(vertIntensity);
                ////v.vertex.xz += _WindDir.xz * sin(vertIntensity);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.origrin_vert1 = v.vertex;
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = mul(unity_ObjectToWorld, i.origrin_vert1);
                float4 oriNormal = tex2D(_Normal,i.uv);
                float3 packedNormal = UnpackNormal(oriNormal);
                float3 worldNormal = UnityObjectToWorldNormal(packedNormal);
                
                UNITY_SETUP_INSTANCE_ID(i); //??????
                float atten = SHADOW_ATTENUATION(i);
                float4 mainColor = tex2D(_MainTex,i.uv);
                float grassDiffuise = dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)) * 0.5 + 0.5;
                //float uv_x = (worldPos.x - _camerapos.x)/_Xscale + 0.5;
                //float uv_z = (worldPos.z - _camerapos.z)/_Zscale + 0.5;
                //float uv_x = (worldPos.x - _camerapos.x)/_camerasize.x + 0.5;
                //float uv_z = (worldPos.z - _camerapos.z)/_camerasize.y + 0.5;
                //float4 final_color = tex2D(_TerrianTex,float2(uv_x, uv_z));
                float4 final_color = mainColor * grassDiffuise;
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                clip(final_color.a - 0.1);
                //final_color = lerp(final_color,_Color * grassDiffuise,pow(i.uv.y,_GroundFactor));
                final_color = lerp(final_color*_ShadowIntensity,final_color,atten);
                final_color.xyz += ambient * mainColor.xyz;
                //final_color += a
                UNITY_APPLY_FOG(i.fogCoord, final_color);
                //return float4(atten,atten,atten,1);
                //return float4(0,0,0,1);
                return final_color;
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
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdadd
            //???????????
            //#pragma skip_variants DIRLIGHTMAP_COMBINED DYNAMICLIGHTMAP_ON LIGHTMAP_ON LIGHTMAP_SHADOW_MIXING LIGHTPROBE_SH SHADOWS_SHADOWMASK VERTEXLIGHT_ON
            //??????��?????????????
            //#pragma multi_compile DIRECTIONAL SHADOWS_SCREEN

            #include "Lighting.cginc"
            #include"AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                //SHADOW_COORDS(3)
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float4 origrin_vert1 :TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                
            };

            float4 _Color;
            uniform float3 _camerapos;
            uniform float2 _camerasize;
            sampler2D _TerrianTex;
            float4 _TerrianTex_ST;
            float _Xscale,_Zscale,_GroundFactor,_WindUnifromIntensity,_ShadowIntensity;
            float3 _WindDir;
            sampler2D _MainTex,_Normal;
            float4 _MainTex_ST,_Normal_ST;

            v2f vert (appdata v)
            {

                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //?????????
                UNITY_TRANSFER_INSTANCE_ID(v,o); //??????
                //????�T????
                //float windIntensity = (sin(_Time.x *20) * 0.5 + 0.5);
                //float vertIntensity = _WindUnifromIntensity *pow( v.uv.y,2) * windIntensity;
                //vertIntensity = radians(vertIntensity);
                ////v.vertex.y *= cos(vertIntensity);
                ////v.vertex.xz += _WindDir.xz * sin(vertIntensity);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.origrin_vert1 = v.vertex;
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = mul(unity_ObjectToWorld, i.origrin_vert1);
                float4 oriNormal = tex2D(_Normal,i.uv);
                float3 packedNormal = UnpackNormal(oriNormal);
                float3 worldNormal = UnityObjectToWorldNormal(packedNormal);

                #ifdef USING_DIRECTIONAL_LIGHT  //ƽ�й��¿���ֱ�ӻ�ȡ����ռ��µĹ��շ���
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				#else//������Դ��_WorldSpaceLightPos0������Դ���������꣬�붥��������������������ɵõ�����ռ��µĹ��շ���
					fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz - worldPos.xyz);
				#endif
                UNITY_SETUP_INSTANCE_ID(i); //??????
                //float atten = SHADOW_ATTENUATION(i);
                float4 mainColor = tex2D(_MainTex,i.uv);
                float grassDiffuise = dot(worldNormal, normalize(worldLightDir.xyz)) * 0.5 + 0.5;
                //float uv_x = (worldPos.x - _camerapos.x)/_Xscale + 0.5;
                //float uv_z = (worldPos.z - _camerapos.z)/_Zscale + 0.5;
                //float uv_x = (worldPos.x - _camerapos.x)/_camerasize.x + 0.5;
                //float uv_z = (worldPos.z - _camerapos.z)/_camerasize.y + 0.5;
                //float4 final_color = tex2D(_TerrianTex,float2(uv_x, uv_z));
                #ifdef USING_DIRECTIONAL_LIGHT  //ƽ�й��²����ڹ���˥������ֵΪ1
					fixed atten = 1.0;
				#else
					#if defined (POINT)    //���Դ�Ĺ���˥������
						//unity_WorldToLight���þ�������ռ䵽��Դ�ռ�任�����붥�������������˿ɵõ���Դ�ռ��µĶ�������
						float3 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1)).xyz;
						//����Unity���ú���tex2D��Unity��������_LightTexture0�����������������Դ˥������ȡ��˥��������
						//��ͨ��UNITY_ATTEN_CHANNEL�õ�˥��������˥��ֵ���ڵķ������Եõ����յ�˥��ֵ
						fixed atten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#elif defined (SPOT)   //�۹�ƵĹ���˥������
						float4 lightCoord = mul(unity_WorldToLight, float4(worldPos, 1));
						//(lightCoord.z > 0)���۹�Ƶ����ֵС�ڵ���0ʱ�������˥��Ϊ0
						//_LightTextureB0������ù�Դʹ����cookie����˥������������Ϊ_LightTextureB0
					fixed atten = (lightCoord.z > 0) * tex2D(_LightTexture0, lightCoord.xy / lightCoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightCoord, lightCoord).rr).UNITY_ATTEN_CHANNEL;
					#else
						fixed atten = 1.0;
					#endif
				#endif
                grassDiffuise *= atten;
                float4 final_color = mainColor * grassDiffuise;
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                clip(final_color.a - 0.1);
                //final_color = lerp(final_color,_Color * grassDiffuise,pow(i.uv.y,_GroundFactor));
                //final_color = lerp(final_color*_ShadowIntensity,final_color,atten);
                //final_color.xyz += ambient * mainColor.xyz;
                //final_color += a
                UNITY_APPLY_FOG(i.fogCoord, final_color);
                //return float4(atten,atten,atten,1);
                //return float4(0,0,0,1);
                return final_color;
            }
            ENDCG
        }

        Pass
        {
            Tags{ "LightMode"="ShadowCaster"}
            
            
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 origrin_vert1 :TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float4 _Color;
            uniform float3 _camerapos;
            uniform float2 _camerasize;
            sampler2D _TerrianTex;
            float4 _TerrianTex_ST;
            float _Xscale,_Zscale,_GroundFactor,_WindUnifromIntensity;
            float3 _WindDir;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {

                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //?????????
                UNITY_TRANSFER_INSTANCE_ID(v,o); //??????
                //????�T????
               //float windIntensity = (sin(_Time.x *20) * 0.5 + 0.5);
               //float vertIntensity = _WindUnifromIntensity *pow( v.uv.y,2) * windIntensity;
               //vertIntensity = radians(vertIntensity);
               //v.vertex.y *= cos(vertIntensity);
               //v.vertex.xz += _WindDir.xz * sin(vertIntensity);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.origrin_vert1 = v.vertex;
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = mul(unity_ObjectToWorld, i.origrin_vert1);
                UNITY_SETUP_INSTANCE_ID(i); //??????
                float4 mainColor = tex2D(_MainTex,i.uv);
                float grassDiffuise = dot(float3(0,1,0), normalize(_WorldSpaceLightPos0.xyz)) * 0.5 + 0.5;
                //float uv_x = (worldPos.x - _camerapos.x)/_Xscale + 0.5;
                //float uv_z = (worldPos.z - _camerapos.z)/_Zscale + 0.5;
                float uv_x = (worldPos.x - _camerapos.x)/_camerasize.x + 0.5;
                float uv_z = (worldPos.z - _camerapos.z)/_camerasize.y + 0.5;
                float4 final_color = mainColor * grassDiffuise;
                clip(final_color.a - 0.1);
                //final_color = lerp(final_color,_Color * grassDiffuise,pow(i.uv.y,_GroundFactor));
                return final_color;
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
