using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getPos : MonoBehaviour
{
    // Start is called before the first frame update
    private ParticleSystemRenderer particleRenderer;
    private Material particleMaterial;
    void Start()
    {
        // 获取粒子系统的Renderer组件
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        
        // 获取当前使用的材质（返回实例）
        particleMaterial = particleRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        particleMaterial.SetVector("pivot",transform.position);
    }
}
