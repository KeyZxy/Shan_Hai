using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GodRayPostEffect : MonoBehaviour
{
    //高亮部分提取阈值
    public Color colorThreshold = Color.gray;
    public Material processMaterial = null;
    //体积光颜色
    public Color lightColor = Color.white;
    //光强度
    [Range(0.0f, 200.0f)]
    public float lightFactor = 0.5f;
    //径向模糊uv采样偏移值
    [Range(0.0f, 100.0f)]
    public float samplerScale = 1;
    //Blur迭代次数
    [Range(1,10)]
    public int blurIteration = 2;
    //降低分辨率倍率
    [Range(0, 3)]
    public int downSample = 1;
    
    [Range(0,1)]
    public float depthThreshold = 0;
    //光源位置
    public Transform lightTransform;
    //产生体积光的范围
    [Range(0.0f, 50.0f)]
    public float lightRadius = 2.0f;
    //提取高亮结果Pow倍率，适当降低颜色过亮的情况
    [Range(1.0f, 40.0f)]
    public float lightPowFactor = 3.0f; 

    private Camera targetCamera = null;
    
    private int _colorThresholdID;
    private int _viewPortLightPosID;
    private int _lightRadiusID;
    private int _powFactorID;
    private int _offsetsID;
    
    private int _blurTexID;
    private int _lightColorID;
    private int _lightFactorID;
    private int _depthThresholdID;
    void Awake()
    {
        targetCamera = GetComponent<Camera>();
        targetCamera.depthTextureMode |= DepthTextureMode.Depth;
        // 初始化时获取属性 ID
        _colorThresholdID = Shader.PropertyToID("_ColorThreshold");
        _viewPortLightPosID = Shader.PropertyToID("_ViewPortLightPos");
        _lightRadiusID = Shader.PropertyToID("_LightRadius");
        _powFactorID = Shader.PropertyToID("_PowFactor");
        _offsetsID = Shader.PropertyToID("_offsets");
        
        _blurTexID = Shader.PropertyToID("_BlurTex");
        _lightColorID = Shader.PropertyToID("_LightColor");
        _lightFactorID = Shader.PropertyToID("_LightFactor");
        
        _depthThresholdID = Shader.PropertyToID("_DepthThreshold");
        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (processMaterial && targetCamera)
        {
            int rtWidth = source.width >> downSample;
            int rtHeight = source.height >> downSample;
            //RT分辨率按照downSameple降低
            RenderTexture temp1 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);

            //计算光源位置从世界空间转化到视口空间
            Vector3 viewPortLightPos = lightTransform == null ? new Vector3(.5f, .5f, 0) : targetCamera.WorldToViewportPoint(lightTransform.position);
          
            //将shader变量改为PropertyId
            
            processMaterial.SetVector(_colorThresholdID, colorThreshold);
            processMaterial.SetVector(_viewPortLightPosID, new Vector4(viewPortLightPos.x, viewPortLightPos.y, viewPortLightPos.z, 0));
            processMaterial.SetFloat(_lightRadiusID, lightRadius);
            processMaterial.SetFloat(_powFactorID, lightPowFactor);
            processMaterial.SetFloat(_depthThresholdID, depthThreshold);
            
            //根据阈值提取高亮部分,使用pass0进行高亮提取，比Bloom多一步计算光源距离剔除光源范围外的部分
            Graphics.Blit(source, temp1, processMaterial, 0);

            processMaterial.SetVector(_viewPortLightPosID, new Vector4(viewPortLightPos.x, viewPortLightPos.y, viewPortLightPos.z, 0));
            processMaterial.SetFloat(_lightRadiusID, lightRadius);
            //径向模糊的采样uv偏移值
            float samplerOffset = samplerScale / source.width;
            //径向模糊，两次一组，迭代进行
            for (int i = 0; i < blurIteration; i++)
            {
                RenderTexture temp2 = RenderTexture.GetTemporary(rtWidth, rtHeight, 0, source.format);
                float offset = samplerOffset * (i * 2 + 1);
                processMaterial.SetVector(_offsetsID, new Vector4(offset, offset, 0, 0));
                Graphics.Blit(temp1, temp2,processMaterial, 1);

                offset = samplerOffset * (i * 2 + 2);
                processMaterial.SetVector(_offsetsID, new Vector4(offset, offset, 0, 0));
                Graphics.Blit(temp2, temp1, processMaterial, 1);
                RenderTexture.ReleaseTemporary(temp2);
            }
           
            // 使用属性 ID 设置属性值
            processMaterial.SetTexture(_blurTexID, temp1);
            processMaterial.SetVector(_lightColorID, lightColor);
            processMaterial.SetFloat(_lightFactorID, lightFactor);
            //最终混合，将体积光径向模糊图与原始图片混合，pass2
            Graphics.Blit(source, destination, processMaterial, 2);

            //释放申请的RT
            RenderTexture.ReleaseTemporary(temp1);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
