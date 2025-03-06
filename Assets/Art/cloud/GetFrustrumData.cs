using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFrustrumData : MonoBehaviour
{
    private Camera cam;
    public Material processMat;

    private Matrix4x4 viewMat;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        //开启深度图
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }

    // Update is called once per frame
    void Update()
    {
        //计算摄像机各种参数
        
    }
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //计算摄像机各种参数
        Transform CamTrans = cam.transform;
        float near = cam.nearClipPlane;
        float far = cam.farClipPlane;
        float halfHeight = cam.nearClipPlane * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Vector3 toRight = CamTrans.right * halfHeight * cam.aspect;
        Vector3 upVector = CamTrans.up * halfHeight;
        Vector3 topVector = CamTrans.forward * near + upVector;
        Vector3 bottomVector = CamTrans.forward * near - upVector;

        Vector3 bottomLeft = bottomVector - toRight; //左下
        Vector3 bottomRight = bottomVector + toRight; //右下
        Vector3 topLeft = topVector - toRight; //左上
        Vector3 topRight = topVector + toRight; //右上
        
        viewMat.SetRow(0, new Vector4(bottomLeft.x, bottomLeft.y, bottomLeft.z, 0.0f));
        viewMat.SetRow(1, new Vector4(bottomRight.x, bottomRight.y, bottomRight.z, 0.0f));
        viewMat.SetRow(2, new Vector4(topLeft.x, topLeft.y, topLeft.z, 0.0f));
        
        viewMat.SetRow(3, new Vector4(topRight.x, topRight.y, topRight.z, 0.0f));
        
        processMat.SetMatrix("_ViewMatrix", viewMat);
        //processMat.SetVector("_CamTransform",new Vector4(CamTrans.position.x, CamTrans.position.y, CamTrans.position.z, 0.0f));
        
        Graphics.Blit(src, dest, processMat);
        

    }
    
}
