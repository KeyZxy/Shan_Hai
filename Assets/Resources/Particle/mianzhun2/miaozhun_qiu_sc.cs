using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miaozhun_qiu_sc : MonoBehaviour
{

    private Transform _source;
    private Player_skill_class _skill;
    private float forwardOffset;
    private float minOffset = 5f;  // 最小距离
    private float maxOffset = 20f; // 最大距离
    private float scrollSpeed = 3f; // 滚轮调整速度

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_source == null) 
            return;

        // 鼠标滚轮调整 forwardOffset
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            forwardOffset += scrollInput * scrollSpeed;
            forwardOffset = Mathf.Clamp(forwardOffset, minOffset, maxOffset);
        }

        // 计算目标位置（_source 的前方）
        Vector3 targetPosition = _source.position + _source.forward * forwardOffset;

        // 设置当前物体的位置
        transform.position = targetPosition;

        // 使当前物体始终面向 _source 的朝向
        transform.rotation = Quaternion.LookRotation(_source.forward);
    }

    public void Init(Transform s , Player_skill_class sk)
    {
        _source = s;
        _skill = sk;
        if (_source != null)
        {
            Vector3 offset = transform.position - _source.position;
            forwardOffset = Vector3.Dot(offset, _source.forward);
        }
    }
}
