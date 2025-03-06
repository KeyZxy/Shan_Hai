Shader "Unlit/玉石02"
{
    Properties
    {
        _JadeMask("jade mask",2D) = "white" {}
        _BaseColor ("BaseColor", color) = (1.0,1.0,1.0,1.0)
        _Distort("Distort",Range(0,1)) = 0
        _Power("Power",Float) = 1.0
        _Scale("Scale",Float) = 1.0
        _BaseTex("BaseTex", 2D) = "white" {}
        _CubeMap("CubeMap", Cube) = "white" {}
        _Rotate_CubeMap("Rotate)CubeMap",range(0,360)) = 0
        _AddColor("AdddColor",color) = (1,1,1,1)
        _SkyLightOpacity("SkyLghtUpacity",range(0,1)) = 0
        _BackLightColor("BackLightColor",color) = (1,1,1,1)
        _LightIntensity("light intensity",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags{"LightMode" = "ForwardBase"}          //tags
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwbase               //计算光照必加


            #include "UnityCG.cginc"
            #include "AutoLight.cginc"                    //计算光照必加

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3  normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal_world : TEXCOORD1;
                float3 pos_world : TEXCOORD2;
            };

            float4 _BaseColor;
            sampler2D _BaseTex;
            samplerCUBE _CubeMap;
            float4 _MainTex_ST;
            float4 _LightColor0;
            float _Distort;
            float _Power;
            float _Scale;
            float4 _CubeMap_HDR;
            float _Rotate_CubeMap;
            float4 _AddColor;
            float _SkyLightOpacity;
            float4 _BackLightColor;
            float _LightIntensity;
            sampler2D _JadeMask;

            float remap(float value, float inMin, float inMax, float outMin, float outMax)
            {
                 // 公式：(value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin
                 return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
            }
                
             half3 RotateFunction(half RotateValue, half3 Target)      //旋转cubemap的函数
            {
                 half rotation_cube = RotateValue * UNITY_PI / 180;
                half cos2 = cos(rotation_cube);
                half sin2 = sin(rotation_cube);
                half2x2 m_rotate_cube = half2x2(cos2, -sin2, sin2, cos2);
                half2 redlect_dir_rotate = mul(Target.xz, m_rotate_cube);    //旋转xz平面
                Target = half3(redlect_dir_rotate.x, Target.y, redlect_dir_rotate.y);   //y值不变
                return Target;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal_world = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.pos_world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float mask_j = tex2D(_JadeMask,i.uv).a;
                mask_j =  smoothstep(0.4,0.5,mask_j);
                //mask_j = 1- mask_j;
                float3 normal_dir = normalize(i.normal_world);
                float3 view_dir = normalize(_WorldSpaceCameraPos.xyz - i.pos_world);
                float3 light_dir = normalize(_WorldSpaceLightPos0.xyz);
                
                half3 ambient = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
                
                //漫反射
                float3 base_color = tex2D(_BaseTex,i.uv);
                float base_term = max(0.0, dot(normal_dir, light_dir));
                float final_base_term = base_term *0.5 + 0.5;
                
                float3 base_light = final_base_term * base_color * _LightColor0.xyz;

                float3 sky_light = (dot(normal_dir,float3(0,1,0)) + 1.0) * 0.5;  //天光模拟

                float3 final_base = base_light + _AddColor.xyz * 0.1 ;

                //透射光
                float3 back_dir = -normalize(light_dir + normal_dir * _Distort);  //偏移透射光方向
                float VdotL = max(0.0,dot(view_dir, back_dir));
                float backlight_term = max(0.0, pow(VdotL, _Power)) * _Scale;  //透射光强度
                //float thickness = 1.0 - tex2D(_ThicknessMap,i.uv).r;           //采样厚度图
                float3 back_color = backlight_term * _LightColor0.xyz  * _BackLightColor;

                //光泽反射
                float3 reflect_dir = reflect(-view_dir, normal_dir);  //反射角度
                reflect_dir = RotateFunction(_Rotate_CubeMap, reflect_dir);   //旋转CubeMap
                float4 hdr_color = texCUBE(_CubeMap, reflect_dir);            //采样CubeMap方式

                float Ndotv = 1.0 - max(0.0, dot(normal_dir, view_dir));      //菲涅尔
                float3 env_color = DecodeHDR(hdr_color,_CubeMap_HDR);         //解码hdr _CubeMap_HDR需要在参数那定义
                float3 final_env = env_color * Ndotv;                         //加上菲涅尔
                //back_color * base_term * mask_j
                float3 final_color =  back_color * remap(base_term,0,1,0.1,1) * mask_j+ final_base * _LightColor0.xyz * _LightIntensity+ ambient * base_color + _BackLightColor * Ndotv * mask_j;
                
                //final_color =   final_base;
                return float4(final_color, 1.0);
            }         
            ENDCG
        }

        //        Pass
        //{
        //    Tags{"LightMode" = "ForwardAdd"}           //ForwarAdd必加
        //    Blend One One                              //ForwarAdd必加
        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag
        //    #pragma multi_compile_fwbase 
//
//
        //    #include "UnityCG.cginc"
        //    #include "AutoLight.cginc"
//
        //    struct appdata
        //    {
        //        float4 vertex : POSITION;
        //        float2 uv : TEXCOORD0;
        //        float3  normal : NORMAL;
        //    };
//
        //    struct v2f
        //    {
        //        float2 uv : TEXCOORD0;
        //        float4 pos : SV_POSITION;
        //        float3 normal_world : TEXCOORD1;
        //        float3 pos_world : TEXCOORD2;
        //        LIGHTING_COORDS(3,4)            //接收阴影 有衰减
        //    };
//
        //    float4 _BaseColor;
        //    sampler2D _ThicknessMap;
        //    samplerCUBE _CubeMap;
        //    float4 _MainTex_ST;
        //    float4 _LightColor0;
        //    float _Distort;
        //    float _Power;
        //    float _Scale;
        //    float4 _CubeMap_HDR;
        //    float _Rotate_CubeMap;
        //    float4 _AddColor;
        //    float _SkyLightOpacity;
        //    float4 _BackLightColor;
//
        //     half3 RotateFunction(half RotateValue, half3 Target)      //旋转cubemap的函数
        //    {
        //         half rotation_cube = RotateValue * UNITY_PI / 180;
        //        half cos2 = cos(rotation_cube);
        //        half sin2 = sin(rotation_cube);
        //        half2x2 m_rotate_cube = half2x2(cos2, -sin2, sin2, cos2);
        //        half2 redlect_dir_rotate = mul(Target.xz, m_rotate_cube);    //旋转xz平面
        //        Target = half3(redlect_dir_rotate.x, Target.y, redlect_dir_rotate.y);   //y值不变
        //        return Target;
        //    }
//
        //    v2f vert (appdata v)
        //    {
        //        v2f o;
        //        o.pos = UnityObjectToClipPos(v.vertex);
        //        o.uv = v.uv;
        //        o.normal_world = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
        //        o.pos_world = mul(unity_ObjectToWorld, v.vertex).xyz;
        //        TRANSFER_VERTEX_TO_FRAGMENT(o);         //接收阴影 有衰减
        //        return o;
        //    }
//
        //    half4 frag (v2f i) : SV_Target
        //    {
        //        float atten = LIGHT_ATTENUATION(i);           //接收阴影 有衰减
        //        float3 normal_dir = normalize(i.normal_world);
        //        float3 view_dir = normalize(_WorldSpaceCameraPos.xyz - i.pos_world);
        //        float3 light_dir = normalize(_WorldSpaceLightPos0.xyz);
        //        float3 light_dirOther = normalize(_WorldSpaceLightPos0.xyz - i.pos_world);
        //        light_dir = lerp(light_dir, light_dirOther, _WorldSpaceLightPos0.w);
//
        //        //float NdotL =  max(0.0, dot(normal_dir, light_dir));
//
        //        //透射光
        //        float3 back_dir = -normalize(light_dir + normal_dir * _Distort);
        //        float VdotL = max(0.0,dot(view_dir, back_dir));
        //        float backlight_term = max(0.0, pow(VdotL, _Power)) * _Scale;
        //        float thickness = 1.0 - tex2D(_ThicknessMap,i.uv).r;
        //        float3 back_color = backlight_term * _LightColor0.xyz * thickness
        //                             * _BackLightColor.xyz * atten;
//
        //        float3 final_color = back_color;
        //        return float4(final_color, 1.0);
        //    }         
        //    ENDCG
        //}
    }
}