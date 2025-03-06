Shader "Unlit/NewUnlitShader"
{
    Properties
    {

        _NoiseTex("noise tex",3D) = "defaulttexture" {}
        _NoiseTiling("noise tiling",FLOAT) = 1
        _TimeSpeed("time speed",range(0,10)) =1
        _OffsetIntensity("offset intensity",range(0,0.01)) = 0.005
		_Tessllation("tessllation",range(0,64)) = 1
        _TessRange("tessllation range",range(0,1000)) = 500

        
        _DiffusePower("diffuse power",range(0,10)) = 1

        _BackSSSStrength("back sss strength",range(0,10)) = 1
        _BackSSSColor("back sss color",COLOR) = (1,1,1,1)
        _BackSSSPower("back sss power",range(0,10)) =1 

        
    }
    SubShader
    {
        //Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        
		AlphaToMask On
        Cull off
        
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            Tags { "LightMode" = "ForwardBase" "RenderType"="Opaque"}

            CGPROGRAM
            #pragma vertex new_vert
            #pragma fragment frag
			#pragma hull hs
   			#pragma domain ds
			#pragma target 5.0
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile DIRECTIONAL SHADOWS_SCREEN
            // make fog work
           
			#include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //������Ӱ
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS                    //������Ӱ
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE            //������Ӱ
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            //#pragma multi_compile _ _SHADOWS_SOFT  
            



            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(4)
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
				

            };


            float _Tessllation,_TessRange,_OffsetIntensity,_NoiseTiling,_TimeSpeed,_BackSSSStrength,_BackSSSPower;
            float4 _BackSSSColor;
            float _DiffusePower;
            sampler3D _NoiseTex;


            
            


			

			struct InternalTessInterp_appdata {
   				  float4 vertex : INTERNALTESSPOS;
   				  float4 tangent : TANGENT;
   				  float3 normal : NORMAL;
                 };

             struct PatchTess
            {
                float edge[3]:SV_TessFactor;
                float inside:SV_InsideTessFactor;
            };



			InternalTessInterp_appdata new_vert (appdata v) {
     			InternalTessInterp_appdata o;

                

     			o.vertex = v.vertex;
     			o.normal = v.normal;
				//UNITY_TRANSFER_FOG(o,o.vertex);
                //TRANSFER_SHADOW(o);
     			return o;
   			}


            v2f vert (appdata v)
            {	

                v2f o;
                //���¼��㷨��
                float3 temp_dir = v.normal.y > 0.999 ? float3(1,0,0):float3(0,1,0);
                float3 binormal = cross(v.normal,temp_dir);
                float3 tangent = cross(binormal,v.normal);

                float3 neigbour1 = v.vertex.xyz + binormal * 0.01;
                float3 neigbour2 = v.vertex.xyz + tangent * 0.01;

                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNeigbour1 = mul(unity_ObjectToWorld, neigbour1);
                float3 worldNeigbour2 = mul(unity_ObjectToWorld, neigbour2);
                float vertexOffsetNoise = tex3Dlod(_NoiseTex,float4(worldPos * _NoiseTiling + _Time.x * _TimeSpeed,0)).r;
                float vertexNeigbour1OffsetNoise = tex3Dlod(_NoiseTex,float4(worldNeigbour1 * _NoiseTiling + _Time.x * _TimeSpeed,0)).r;
                float vertexNeigbour2OffsetNoise = tex3Dlod(_NoiseTex,float4(worldNeigbour2 * _NoiseTiling + _Time.x * _TimeSpeed,0)).r;
     

                v.vertex.xyz += v.normal * vertexOffsetNoise *_OffsetIntensity;

                neigbour1 += v.normal * vertexNeigbour1OffsetNoise *_OffsetIntensity;
                neigbour2 += v.normal * vertexNeigbour2OffsetNoise *_OffsetIntensity;

                float3 new_dir1 = neigbour1 - v.vertex.xyz;
                float3 new_dir2 = neigbour2 - v.vertex.xyz;

                v.normal = normalize(-cross(new_dir1,new_dir2));

                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                TRANSFER_SHADOW(o)
                UNITY_TRANSFER_FOG(o,o.pos);
				//��������
				

                return o;
            }

			 PatchTess hsconst (InputPatch<InternalTessInterp_appdata,3> v) {
   				PatchTess o;
                float3 worldPos = mul(unity_ObjectToWorld, v[1].vertex);
                float3 cameraWS = _WorldSpaceCameraPos.xyz;
                float Distance = distance(worldPos , cameraWS);
                float finalTess = lerp(1,_Tessllation ,1 - saturate(Distance/_TessRange));
                


   			  	o.edge[0] = finalTess; 
   			  	o.edge[1] = finalTess; 
   			  	o.edge[2] = finalTess; 
   			  	o.inside = finalTess;
   			  	return o;
   			}
			
   			[domain("tri")]
   			[partitioning("fractional_odd")]
   			[outputtopology("triangle_cw")]
   			[patchconstantfunc("hsconst")]
   			[outputcontrolpoints(3)]
   			InternalTessInterp_appdata hs (InputPatch<InternalTessInterp_appdata,3> v, uint id : SV_OutputControlPointID) {
   			  return v[id];
   			}
			
   			[domain("tri")]
   			v2f ds ( PatchTess tessFactors, const OutputPatch<InternalTessInterp_appdata,3> vi, float3 bary : SV_DomainLocation) {
   			  appdata v;
			
   			  v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
   			  v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
			
   			  v2f o = vert (v);
   			  return o;
   			}
		



            half4 frag (v2f i) : SV_Target
            {   
                
                float3 worldNormal =UnityObjectToWorldNormal(i.normal);
                //Light main_light = GetMainLight();
                //Light main_light = GetMainLight(TransformWorldToShadowCoord(i.worldPos.xyz));
                float lambert_item = max(dot(worldNormal,normalize(_WorldSpaceLightPos0.xyz)),0);
                lambert_item = pow(lambert_item,_DiffusePower);
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
            	


                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);


                float3 backLitDir = worldNormal * _BackSSSStrength + normalize(_WorldSpaceLightPos0.xyz);
                float backSSS = max(0,dot(viewDir,backLitDir));
                backSSS = pow(backSSS,_BackSSSPower);

                float NdotV = max(0,dot(worldNormal,viewDir)); 
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                fixed shadow =SHADOW_ATTENUATION(i); 

                float3 finalColor = float3(1,1,1) *lambert_item * shadow *_LightColor0.rgb + ambient + backSSS * _BackSSSColor + NdotV * _BackSSSColor * 0.5;
            	float4 col = float4(finalColor,1);
				UNITY_APPLY_FOG(i.fogCoord, col);
               //return float4(shadow,shadow,shadow,1);
				return col;
				
            }
            ENDCG
        }

		Pass
        {
            Tags { "LightMode" = "ShadowCaster" }

            CGPROGRAM
            #pragma vertex new_vert
            #pragma fragment frag
			#pragma hull hs
   			#pragma domain ds
			#pragma target 5.0
            // make fog work
           
			#include "Lighting.cginc"
			#include "UnityCG.cginc"
            #include"AutoLight.cginc"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //������Ӱ
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS                    //������Ӱ
            //#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE            //������Ӱ
            //#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            //#pragma multi_compile _ _SHADOWS_SOFT  
            #pragma multi_compile_fwdbase 



            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(4)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                SHADOW_COORDS(3)
				

            };


            float _Tessllation,_TessRange,_OffsetIntensity,_NoiseTiling,_TimeSpeed,_BackSSSStrength,_BackSSSPower;
            float4 _BackSSSColor;
            float _DiffusePower;
            sampler3D _NoiseTex;


            
            


			

			struct InternalTessInterp_appdata {
   				  float4 vertex : INTERNALTESSPOS;
   				  float4 tangent : TANGENT;
   				  float3 normal : NORMAL;
                 };

             struct PatchTess
            {
                float edge[3]:SV_TessFactor;
                float inside:SV_InsideTessFactor;
            };



			InternalTessInterp_appdata new_vert (appdata v) {
     			InternalTessInterp_appdata o;

                

     			o.vertex = v.vertex;
     			o.normal = v.normal;
                 TRANSFER_SHADOW(o);
				UNITY_TRANSFER_FOG(o,o.vertex);
     			return o;
   			}


            v2f vert (appdata v)
            {	

                v2f o;
                //���¼��㷨��
                float3 temp_dir = v.normal.y > 0.999 ? float3(1,0,0):float3(0,1,0);
                float3 binormal = cross(v.normal,temp_dir);
                float3 tangent = cross(binormal,v.normal);

                float3 neigbour1 = v.vertex.xyz + binormal * 0.01;
                float3 neigbour2 = v.vertex.xyz + tangent * 0.01;

                
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNeigbour1 = mul(unity_ObjectToWorld, neigbour1);
                float3 worldNeigbour2 = mul(unity_ObjectToWorld, neigbour2);
                float vertexOffsetNoise = tex3Dlod(_NoiseTex,float4(worldPos * _NoiseTiling + _Time.x * _TimeSpeed,0)).r;
                float vertexNeigbour1OffsetNoise = tex3Dlod(_NoiseTex,float4(worldNeigbour1 * _NoiseTiling + _Time.x * _TimeSpeed,0)).r;
                float vertexNeigbour2OffsetNoise = tex3Dlod(_NoiseTex,float4(worldNeigbour2 * _NoiseTiling + _Time.x * _TimeSpeed,0)).r;
     

                v.vertex.xyz += v.normal * vertexOffsetNoise *_OffsetIntensity;

                neigbour1 += v.normal * vertexNeigbour1OffsetNoise *_OffsetIntensity;
                neigbour2 += v.normal * vertexNeigbour2OffsetNoise *_OffsetIntensity;

                float3 new_dir1 = neigbour1 - v.vertex.xyz;
                float3 new_dir2 = neigbour2 - v.vertex.xyz;

                v.normal = normalize(-cross(new_dir1,new_dir2));

                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = v.normal;
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o,o.pos);
				//��������
				

                return o;
            }

			 PatchTess hsconst (InputPatch<InternalTessInterp_appdata,3> v) {
   				PatchTess o;
                float3 worldPos = mul(unity_ObjectToWorld, v[1].vertex);
                float3 cameraWS = _WorldSpaceCameraPos.xyz;
                float Distance = distance(worldPos , cameraWS);
                float finalTess = lerp(1,_Tessllation ,1 - saturate(Distance/_TessRange));
                


   			  	o.edge[0] = finalTess; 
   			  	o.edge[1] = finalTess; 
   			  	o.edge[2] = finalTess; 
   			  	o.inside = finalTess;
   			  	return o;
   			}
			
   			[domain("tri")]
   			[partitioning("fractional_odd")]
   			[outputtopology("triangle_cw")]
   			[patchconstantfunc("hsconst")]
   			[outputcontrolpoints(3)]
   			InternalTessInterp_appdata hs (InputPatch<InternalTessInterp_appdata,3> v, uint id : SV_OutputControlPointID) {
   			  return v[id];
   			}
			
   			[domain("tri")]
   			v2f ds ( PatchTess tessFactors, const OutputPatch<InternalTessInterp_appdata,3> vi, float3 bary : SV_DomainLocation) {
   			  appdata v;
			
   			  v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
   			  v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
			
   			  v2f o = vert (v);
   			  return o;
   			}
		



            half4 frag (v2f i) : SV_Target
            {   
                
                float3 worldNormal =UnityObjectToWorldNormal(i.normal);
                //Light main_light = GetMainLight();
                //Light main_light = GetMainLight(TransformWorldToShadowCoord(i.worldPos.xyz));
                float lambert_item = max(dot(worldNormal,normalize(_WorldSpaceLightPos0.xyz)),0);
                lambert_item = pow(lambert_item,_DiffusePower);
                float3 ambient = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);


                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos.xyz);


                float3 backLitDir = worldNormal * _BackSSSStrength + normalize(_WorldSpaceLightPos0.xyz);
                float backSSS = max(0,dot(viewDir,backLitDir));
                backSSS = pow(backSSS,_BackSSSPower);

                float NdotV = max(0,dot(worldNormal,viewDir)); 
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                fixed shadow =SHADOW_ATTENUATION(i); 

                float3 finalColor = float3(1,1,1) *lambert_item * shadow *_LightColor0.rgb + ambient + backSSS * _BackSSSColor + NdotV * _BackSSSColor * 0.5;
				UNITY_APPLY_FOG(i.fogCoord, float4(finalColor,1));
               //return float4(ambient,1);
				return float4(finalColor,1);
				
            }
            ENDCG
        }
    }
}
