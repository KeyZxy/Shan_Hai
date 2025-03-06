using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // 单例实例
    public static ResourceManager Instance { get; private set; }

    // 存储加载的资源的字典
    private Dictionary<string, Object> resources = new Dictionary<string, Object>();

    private void Awake()
    {
        // 确保单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 保证切换场景后仍然存在
            PreloadResources(); // 调用预加载方法
        }
        else
        {
            Destroy(gameObject);
        }
    }



    // 通用的加载资源方法
    private void LoadResource(string path)
    {
        if (!resources.ContainsKey(path))
        {
            Object resource = Resources.Load(path);
            if (resource != null)
            {
                resources.Add(path, resource);
            }
            else
            {
                Debug.LogWarning($"资源未找到：{path}");
            }
        }
    }

    // 获取资源的公共方法
    public T GetResource<T>(string path) where T : Object
    {
        if (resources.TryGetValue(path, out Object resource))
        {
            return resource as T;
        }
        Debug.LogWarning($"资源未预加载：{path}");
        return null;
    }


    // 预加载资源
    private void PreloadResources()
    {
        LoadResource("Particle/Fu/start");
        LoadResource("Particle/Fu/Fu");
        LoadResource("Prefab/UI/HUDObj");
        LoadResource("Particle/EXP/EXP_obj");
        LoadResource("Particle/jiguangbo/jiguangb");
        LoadResource("Particle/jiguangbo/jieshu_M");
        LoadResource("Particle/jiguangbo/shouji_M");
        LoadResource("Particle/mianzhun2/miaozhunqiu_M");
        LoadResource("Particle/huongjue/huongjue");
    }



}
