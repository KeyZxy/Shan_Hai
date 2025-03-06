Shader "Unlit/heightFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FogStart("fog start",FLOAT) = 1
        _FogEnd("fog end",FLOAT) = 1
        _FogSpeed("fog speed",Range(0,10)) =1
        
        _FogTex("fog tex",2D) =  "white" {}
        _FogColor("fog color",COLOR) = (1,1,1,1)
    	
    	_NoiseIntensity("noise intensity",Range(0,1)) =1
        
        [Header(Volume Lighting)]
        _VLightColor("VLight Color",COLOR) = (1,1,1,1)
        _VLightIntensity("vlight intensity",Range(0,10)) = 0.1
        _StepSize("step size",Range(0,1)) = 0.1
        _MaxStep ("max step",float) = 200      //设置最大步数
        _MaxDistance ("max distance",float) = 1000   //最大步进距离
    	_ShadowAttenuation("ShadowAttenuation",Float) = 0.08
    	_VLightPower("vlight power",Range(0,10)) = 1
    	_DirLightDistance("DirLightDistance",FLOAT) = 100
    	_LightExtinction("light extinction",Range(0,1)) = 0.1
    	_Vlight_G("vlight_g",Range(-1,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile DIRECTIONAL 

            #include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"
            #include "UnityDeferredLibrary.cginc"
            #include "VolumeLighting.cginc"

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
                float3 viewRay : TEXCOORD2;
            	float3 worldPos : TEXCOORD3;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex,_FogTex;
            float4 _MainTex_ST,_FogColor,_FogTex_ST,_VLightColor;

            float _FogStart,_FogEnd,_FogSpeed,_StepSize,_MaxStep,_MaxDistance,_VLightIntensity,_VLightPower,_NoiseIntensity;
            float _DirLightDistance,_LightExtinction,_Vlight_G;

            //sampler2D _CameraDepthTexture;
            uniform float4x4 _ViewMatrix;
            //sampler2D _ShadowMapTexture;
            //Texture2D _ShadowMapTexture;
            //SamplerComparisonState sampler_ShadowMapTexture;
            float _ShadowAttenuation;
            //uniform float4 _CamTransform;
            UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);

            float Remap(float x,float from1,float to1,float from2,float to2) {
				return (x - from1) / (to1 - from1) * (to2 - from2) + from2;
			}

            

            float GetShadow(float3 worldPos) {
				//比较灯光空间深度
				//UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
				float4 lightPos = mul(unity_WorldToShadow[0], float4(worldPos, 1));
            	//float4 lightPos = ComputeScreenPos(float4(worldPos, 1));
            	//unitySampleShadow(lightPos);
 				float shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, lightPos.xyz);
				float depth = lightPos.z ;
				float shadowValue = step(shadow, depth);
				//阴影的衰减
				float dis = abs(depth - shadow);								
				shadowValue += clamp(Remap(dis, _ShadowAttenuation,0.1,0,1),0,1)*(1-shadowValue);
            	//return shadow;
				return shadowValue;
			}
			//观察方向ray，从near到far对散射光线通过Ray-Marching采样积分
			
            //返回介质中x处接收到的光线（RGB），以及x处到光源的方向
			float3 lightAt(float3 pos, out float3 lightDir)
			{
			    //_LightPosition.w = 1时，为SpotLight
			    //根据点光源和平行光分别处理
			    lightDir = normalize(_WorldSpaceLightPos0.xyz - pos *  _WorldSpaceLightPos0.w);
			    float lightDistance = lerp(_DirLightDistance, distance(_WorldSpaceLightPos0.xyz, pos), _WorldSpaceLightPos0.w);
			    //从介质中x处到视点的消光系数，采用累乘避免多次积分
			    // = lerp(1, exp(-lightDistance * _LightExtinction), _IncomingLoss);
				float transmittance = _LightExtinction;
			    float3 lightColor = _VLightColor.rgb;
			    //考虑光源方向与片元到光源方向之间夹角的能量损失
			    //lightColor *= step(_LightCosHalfAngle, dot(lightDir, _LightDirection.xyz));
			    //考虑阴影
			    lightColor *=GetShadow(pos);
			    //透射率造成的衰减
			    lightColor *= transmittance;
			    //散射系数=消光系数-吸收系数，但这里参数简化为比例，即散射系数=消光系数*(1 - _Absorption)
			    //lightColor *= extinctionAt(pos, _BrightIntensity) * (1 - _Absorption);
			
			    return lightColor;
			}

            float3 scattering(float3 ray, float stepSize, float Depth, float Steps ,out float3 transmittance)
			{
			    transmittance = 1;
			    float3 totalLight = 0;
			    //float stepSize = (far - near) / Steps;
            	//float stepSize = Depth/Steps;
			    // [UNITY_LOOP]
			    for(int i= 1; i <= Steps; i++)
			    {
			        float3 pos = _WorldSpaceCameraPos + ray * (stepSize * i);
			    	if (stepSize * i >= Depth)
			    	{
			    		break;
			    	}
			        //从视点到介质中x处的透射率，采用累乘避免多次积分
			        transmittance *= exp(-stepSize * _LightExtinction);
			
			        float3 lightDir;
			        //散射光线=从介质中x处到视点的透射（光）率*从光源到介质中x处的散射光线*步进权重*从介质中x处到视点的Phase function（粒子直径对散射方向的影响）
			        totalLight += transmittance * lightAt(pos, lightDir) * stepSize * HGPhase(lightDir,-ray,_Vlight_G);
			    }
			    return totalLight;
			}
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
            	o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //对每一个顶点进行赋值
                if (o.uv.y < 0.5 && o.uv.x < 0.5) {
					o.viewRay = _ViewMatrix[2];  //左下
                    #if UNITY_UV_STARTS_AT_TOP
                    o.viewRay = _ViewMatrix[0];
                    #endif
				}
				else if (o.uv.y < 0.5 && o.uv.x > 0.5) { //右下
					o.viewRay = _ViewMatrix[3];
				    #if UNITY_UV_STARTS_AT_TOP
                    o.viewRay = _ViewMatrix[1];
                    #endif
				}
				else if (o.uv.y > 0.5 && o.uv.x < 0.5) { //左上
					o.viewRay = _ViewMatrix[0];
				    #if UNITY_UV_STARTS_AT_TOP
                    o.viewRay = _ViewMatrix[2];
                    #endif
				}
				else if (o.uv.y > 0.5 && o.uv.y > 0.5) { //右上
					o.viewRay = _ViewMatrix[1];
				    #if UNITY_UV_STARTS_AT_TOP
                    o.viewRay = _ViewMatrix[3];
                    #endif
				}
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	
                float fogNoise = tex2D(_FogTex,i.worldPos.xy + _FogSpeed * _Time.x);
                float4 finalFogColor = fogNoise * _FogColor;
            	finalFogColor = lerp(finalFogColor,_FogColor,_NoiseIntensity);
                //采样深度图
                SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                //转化深度
                float realDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
				
            	float3 viewRay = normalize(i.viewRay);
            	float3 worldRay = normalize(mul((float3x3)unity_ObjectToWorld, viewRay));
                //还原世界坐标
                float3 worldPos = _WorldSpaceCameraPos.xyz + viewRay * realDepth;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                //基于世界坐标高度插值颜色
                float fogInterpolator = (worldPos.y - _FogStart)/(_FogEnd - _FogStart);
                fogInterpolator = saturate(fogInterpolator);

                //加入体积光
                float totalInt = 0;
                float3 rayDir = normalize(i.viewRay);
                float3 currentPos = _WorldSpaceCameraPos.xyz;
                //进行光线步进
                //[unroll(200)]
                //for (int j = 0 ;j <_MaxStep;j++)
                //{
                //    currentPos -= rayDir * _StepSize;
                //	//当超过最大步进距离或超过场景距离时停止
                //    if (distance(currentPos,_WorldSpaceCameraPos.xyz) > _MaxDistance ||distance(currentPos,_WorldSpaceCameraPos.xyz) >  distance(worldPos,_WorldSpaceCameraPos.xyz))
                //        break;
                //    //UNITY_LIGHT_ATTENUATION(attenuation, i, currentPos);
                //    float atten = GetShadow(currentPos);
                //    totalInt += atten * _VLightIntensity;
                //}
            	//totalInt = pow(totalInt,_VLightPower);
            	//return float4(totalInt,totalInt,totalInt,1);
                //float4 finalVlight = totalInt *_VLightColor ;
            	float3 transmitt;
            	float3 finalVlight = scattering(rayDir,_StepSize,realDepth,_MaxStep,transmitt);
                col = lerp(finalFogColor,col,fogInterpolator);
               // col.rgb = col.rgb + finalVlight * _VLightIntensity;
                // apply fog
                //return float4(finalVlight,1);
                UNITY_APPLY_FOG(i.fogCoord, col);
            	//return float4(worldPos,1);
                return col;
            }
            ENDCG
        }
    }
}
