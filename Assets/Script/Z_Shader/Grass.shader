Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NormalMap ("Normal Map", 2D) = "bump" {} // 添加Normal Map属性
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" } // 将RenderType设为Transparent以支持透明
        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha // 设置Alpha混合模式
        Cull Off // 双面渲染
        ZWrite Off // 禁用深度写入，防止不透明对象的渲染出错

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade // 使用fade模式

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap; // 定义Normal Map变量

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap; // 定义Normal Map的UV坐标
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // 使用Normal Map
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
        }
        ENDCG
    }
    FallBack "Transparent"
}
