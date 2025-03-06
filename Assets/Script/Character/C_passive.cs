using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_passive : MonoBehaviour
{

    public List<GameObject> Obj_list = new List<GameObject>();
    private List<GameObject> Passive_list = new List<GameObject>();

    private C_attribute _attr;
    private C_upgrade_attr _upgrade_attr;

    // Start is called before the first frame update
    void Start()
    {
        _attr = transform.GetComponent<C_attribute>();
        _upgrade_attr = transform.GetComponent<C_upgrade_attr>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPassiveAction(int id)
    {
        switch(id)
        {
            case 220003: // 水环-太阴玄水
                CreateOrReplacePassiveObject(id, typeof(shuihuan_sc));
                break;
            case 220001: // 环绕飞剑-桃木剑环
                CreateOrReplacePassiveObject(id, typeof(sword_sc));
                break;
            case 220002: // 小炮仗-离火弹花
                CreateOrReplacePassiveObject(id, typeof(xiaopaozhang_sc));
                break;

            default:
                Debug.LogWarning($"未找到 ID {id} 对应的函数！");
                break;
        }
    }


    // 通用方法
    private GameObject CreateOrReplacePassiveObject(int id, System.Type scriptType)
    {
        GameObject temp_obj = null;

        // 在 Passive_list 中查找是否已有相同名称的对象
        GameObject existingObj = Passive_list.Find(obj => obj.name == id.ToString());

        // 如果找到了，先销毁旧对象并从列表中移除
        if (existingObj != null)
        {
            Passive_list.Remove(existingObj);
            Destroy(existingObj);
        }

        // 在 Obj_list 中查找名称等于 id 的 GameObject
        GameObject FB = Obj_list.Find(obj => obj.name == id.ToString());

        // 创建新的 FB
        if (FB != null)
        {
            temp_obj = Instantiate(FB);
            temp_obj.name = FB.name; // 确保新创建的对象名字一致

            // 初始化脚本
            Component scriptInstance = temp_obj.GetComponent(scriptType);
            if (scriptInstance != null)
            {
                // 通过反射调用 Init 方法
                scriptType.GetMethod("Init")?.Invoke(scriptInstance, new object[] { _attr, _upgrade_attr.Get_info_by_ID(id), transform });
            }
            else
            {
                Debug.LogWarning($"GameObject {FB.name} 没有 {scriptType.Name} 组件！");
            }

            // 添加到 Passive_list
            Passive_list.Add(temp_obj);
        }
        else
        {
            Debug.LogWarning($"未找到名为 {id} 的 GameObject，无法创建实例！");
        }

        return temp_obj;
    }



   
}
