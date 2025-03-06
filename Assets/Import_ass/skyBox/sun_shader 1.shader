Shader "Unlit/sun_shader"
{
    Properties
    {
        [Header(sun moon Settings)]
        _sunradius("sun_radius",range(0,0.5)) = 0.2
        _moonradius("moon_radius",range(0,0.5)) = 0.2
        _MaskMoonRadius("mask moon radius",range(1,2)) = 1
        _moon_jitter_x("moon_jitter_x",range(-1,1)) = 0
        _moon_jitter_z("moon_jitter_z",range(-1,1)) = 0
        _SunColor("sun color",Color) = (1,1,1,1)
        _MoonColor("moon color",Color) = (1,1,1,1)
        _SoftMoonEdge("soft moon edge",range(0,1)) = 1
        _MoonEdgePlus("moon edge plus",range(0,10)) = 1
        _Moonout("moon out",range(0,1)) = 1

        
        [Header(day night Settings)]
        _daytopcolor("night_top_color",Color) = (1,1,1,1)
        _nighttopcolor("day_top_color",Color) = (0,0,0,1)
        _SunSetColor("sun set color",Color) = (1,1,1,1)
        _daybottomcolor("night_bottom_color",Color) = (1,1,1,1)
        _nightbottomcolor("day_bottom_color",Color) = (0,0,0,1)

        [Header(star Settings)]
        _startex("star",2D) = "white" {}
        _StarNoiseTex("star noise tex",2D) = "white" {}
        _starcolor("star_color",Color) = (1,1,1,1)
        _starculloff("star_cull_off",range(0,1)) = 0.5

        [Header(galaxy Settings)]
        _GalaxyScopeRadius("galaxy scope radius",range(0,10)) = 1
        _GalaxyScopePow("galaxy scope pow",range(1,10)) = 1
        _GalaxyDir("galaxy dir",Vector) = (1,1,1,1)
        _GalaxyTex("galaxy tex",2D) = "white" {}
        _galaxyNoiseTex("galaxy noise tex",2D) = "white" {}
        _GalaxyColor1("galaxy color1",Color) = (1,1,1,1)
        _GalaxyColor2("galaxy color2",Color) = (1,1,1,1)
        _GalaxyColor3("galaxy color3",Color) = (1,1,1,1)
        
        [Header(atmosphere Settings)]
        _planetradius("planetradius",range(0,30000)) = 10
        _globalDensity("globaldensity",range(0,1)) = 0.5
        _ExtinctionM("extinctionM",range(1,100)) = 10
        _ExtinctionMMoon("extinction moon",range(1,100)) = 10
        _MieG("mie_g",range(0,1)) = 0.5
        _MieGMoon("mie_g moon",range(0,1)) = 0.5
        _Scatterm("Scatter",Color) = (1,1,1,1)
        _Scatterm_moon("Scatter moon",Color) = (1,1,1,1)
        _MieStrengthMoon("mie strength moon",range(0,100)) = 1
        _MieColor("miecolor",Color) = (1,1,1,1)
        _MieStrength("mieStrength",range(0,100)) = 1
        _raystart("raystart",range(0,1)) = 0.5

        [Header(Horizon Settings)]
        _HorizonIntensity("horizon_intensity",range(0,100)) = 10
        _HorizonColorDay("horizon_color_day",Color) = (1,1,1,1)
        _HorizonColorNight("horizon_color_night",Color) = (1,1,1,1)
        _HorizonHeight("horizon_height",range(0,1)) = 0.5


        [Header(skycolor)]
        _SkyColorNear("sky color near",Color) = (1,1,1,1)
        _SkyColorFar("sky color far",Color) = (1,1,1,1)
        _SkyColorMid("sky color mid",Color) = (1,1,1,1)

        _ColorRange("color range",range(0,10)) = 1
        _ColorRange1("color range1",range(0,10)) = 1

        _ColorIntensity("color intenisty",range(0,10)) = 1
        _ColorIntensity1("color intenisty1",range(0,10)) = 1
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

            #include "UnityCG.cginc"
            #include "atmoscatter.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
                float4 new_uv :TEXCOORD2;
                float2 galaxy_noise_uv:TEXCOORD3;
                float2 star_noise_uv : TEXCOORD1;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 new_uv : TEXCOORD3;
                float2 star_noise_uv : TEXCOORD2;
                float2 galaxy_noise_uv:TEXCOORD4;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 world_pos :TEXCOORD5;
            };

            sampler2D _MainTex,_startex,_StarNoiseTex,_GalaxyTex,_galaxyNoiseTex;
            float4 _MainTex_ST,_startex_ST,_StarNoiseTex_ST,_GalaxyTex_ST,_MieColor,_HorizonColorDay,_HorizonColorNight;
            float _sunradius,_ExtinctionM,_globalDensity,_MieStrength,_raystart,_MieGMoon;
            float _moonradius,_MieG,_HorizonIntensity,_HorizonHeight,_ExtinctionMMoon;
            float _moon_jitter_x,_moon_jitter_z,_starculloff,_GalaxyScopeRadius,_GalaxyScopePow;
            float4 _daybottomcolor,_daytopcolor,_nightbottomcolor,_nighttopcolor,_starcolor,_Scatterm,_GalaxyDir,_Scatterm_moon;
            float4 _GalaxyColor1,_GalaxyColor2,_galaxyNoiseTex_ST,_SunColor,_MoonColor,_GalaxyColor3,_SunSetColor;
            float _planetradius,_MieStrengthMoon,_SoftMoonEdge,_MoonEdgePlus,_Moonout,_MaskMoonRadius;


            float4 _SkyColorFar,_SkyColorNear,_SkyColorMid;
            float _ColorRange,_ColorRange1,_ColorIntensity,_ColorIntensity1;

            void Computelocaldensity(float3 position,float3 light_dir,out float localDPA,out float localDCP){
                float3 center = float3(0,-_planetradius,0);
                float height = distance(position,center) - _planetradius;
                localDPA = exp(- height/_globalDensity);
                localDCP = 0;
            }

            float miePhase(float costheta,float MieG){
                float g = MieG;
                float a = 1 - pow(g,2);
                return a/(4*PI*pow(1 + pow(g,2)- 2 * g*costheta,3/2));
            }


            //利用梯形公式进行近似积分
            float4  intergeateScatter(float3 RayStart,float3 RayDir,float SampleCount,float rayLength,float3 lightDir,float4 Scatterm,float ExtinctionM,float mie_g){
                float3 step_vector = RayDir * (rayLength/SampleCount);
                float stepsize = length(step_vector);
                float scattermie = 0;
                //局部量
                float localDPA = 0;
                //总量
                float densitypa = 0;
                float densitycp = 0;

                //前局部量
                float prelocalDPA = 0;
                float prelocalTransmittance = 0;

                Computelocaldensity(RayStart,lightDir,localDPA,densitycp);
                densitypa = localDPA * stepsize;
                prelocalDPA = localDPA;

                float transmittance = 0;
                transmittance += exp(- (densitycp + densitypa) * ExtinctionM) * localDPA;
                prelocalTransmittance = transmittance;
                for(int i = 1;i < SampleCount;i++){
                    //光线步进
                    float3 p = RayStart + step_vector *i;
                    Computelocaldensity(p,lightDir,localDPA,densitycp);
                    densitypa = (prelocalDPA + localDPA) * stepsize/2;
                    transmittance += exp(- (densitycp + densitypa) * ExtinctionM) * localDPA;
                    scattermie += (transmittance + prelocalTransmittance)*stepsize/2;
                    prelocalDPA = localDPA;
                    prelocalTransmittance = transmittance;
                }
                    scattermie = scattermie * miePhase(dot(RayDir,-lightDir.xyz),mie_g);
                    float3 lightinscatter = Scatterm*scattermie;
                    return float4(lightinscatter,1);
            }


            


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.new_uv = v.new_uv;
                o.world_pos = mul(unity_ObjectToWorld,v.vertex);

                o.star_noise_uv = TRANSFORM_TEX(v.star_noise_uv,_StarNoiseTex);
                o.galaxy_noise_uv = TRANSFORM_TEX(v.galaxy_noise_uv,_galaxyNoiseTex);
                o.new_uv.xz = TRANSFORM_TEX(o.new_uv.xz,_GalaxyTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {   //日月循环
                //sun
                float sun = distance(i.uv.xyz,_WorldSpaceLightPos0);
                float sun_disc = 1 - saturate(sun/_sunradius);
                sun_disc = saturate(sun_disc *50);

                //moon
                float moon = distance(i.uv.xyz,-_WorldSpaceLightPos0);
                float moon_disc = 1 - saturate(moon/_moonradius);
                moon_disc =saturate( moon_disc * 50);

                //月亮遮挡(含过渡)
                float new_moon = distance(float3(i.uv.x + _moon_jitter_x,i.uv.y,i.uv.z + _moon_jitter_z),-_WorldSpaceLightPos0);
                float new_moon_disc = 1 - saturate(new_moon/_moonradius * _MaskMoonRadius);
                new_moon_disc = saturate(pow((new_moon_disc*_MoonEdgePlus), _SoftMoonEdge));
                float final_moon = saturate(_Moonout*moon_disc - new_moon_disc);

                float sunDistance = distance(i.uv.xyz,_WorldSpaceLightPos0);
                float sunDistance_interpolator1 = saturate(sunDistance/_ColorRange);
                float sunDistance_interpolator2 = saturate(sunDistance/_ColorRange1);

                sunDistance_interpolator1 = pow(sunDistance_interpolator1,_ColorIntensity);
                sunDistance_interpolator2 = pow(sunDistance_interpolator2,_ColorIntensity1);


                
                float4 skyColor = lerp(_SkyColorFar,_SkyColorMid,sunDistance_interpolator1);
                skyColor = lerp(skyColor,_SkyColorNear,sunDistance_interpolator2);
                
                
                float4 final_change = sun_disc * _SunColor + final_moon * _MoonColor ;
                //float4 final_color = lerp(skyColor,final_change,sun_disc + final_moon);
                
                
                float4 final_color = skyColor + final_change;

                //大气散射
                float3 mie_color = 0;

                float3 RayStart = float3(0,_raystart,0);
                RayStart.y = saturate(RayStart.y);
                float3 RayDir = normalize(i.uv.xyz);

                float3 planet_center = float3(0,-_planetradius,0);
                float2 intersection = RaySphereIntersection(RayStart,RayDir,planet_center,_planetradius);
                float raylength = intersection.y;

                //当x大于0时说明有两个交点
                if(intersection.x > 0)
                        raylength = min(raylength,intersection.x*100);

                float4 inscattering = intergeateScatter(RayStart,RayDir,16,raylength,-_WorldSpaceLightPos0.xyz,_Scatterm,_ExtinctionM,_MieG);
            
                mie_color = _MieColor * _MieStrength * inscattering.xyz;  

                //月光

                inscattering = intergeateScatter(RayStart,RayDir,16,raylength,_WorldSpaceLightPos0.xyz,_Scatterm_moon,_ExtinctionMMoon,_MieGMoon);
            
                mie_color += _MieColor * _MieStrengthMoon * inscattering.xyz;  
                

                return final_color + fixed4(mie_color,1);
                //return day_bottom_color;
            }
            ENDCG
        }
    }
}
