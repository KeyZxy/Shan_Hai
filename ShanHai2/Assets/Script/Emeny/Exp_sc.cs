using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp_sc : MonoBehaviour
{

    public int exp_value = 5;

    private Transform _target;
    private float acceleration = 5f; // 加速度
    private float initialSpeed = 5f; // 初始速度
    private float currentSpeed = 0f; // 当前速度

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = initialSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(_target)
        {
            // 计算方向
            Vector3 direction = (_target.position - transform.position).normalized;

            // 更新速度
            currentSpeed += acceleration * Time.deltaTime;

            // 移动物体
            transform.position += direction * currentSpeed * Time.deltaTime;

            // 可选：检查是否到达目标
            if (Vector3.Distance(transform.position, _target.position) < 1f)
            {
                transform.position = _target.position; // 确保最终位置
                GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_attribute>().Exp_up(exp_value);
                Destroy(gameObject);
            }
        }
    }

    public void Set_exp(int e)
    {
        exp_value = e;
    }

    public void pick_up(Transform t)
    {
        _target = t;
        Destroy(transform.GetComponent<Rigidbody>());
        Destroy(transform.GetComponent<BoxCollider>());
        
    }

}
