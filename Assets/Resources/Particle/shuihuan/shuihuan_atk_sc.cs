using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class shuihuan_atk_sc : MonoBehaviour
{

    private float pushForce = 10f;
    private Transform source;
    private Player_skill_class _skill_class;
    private bool isStart = false;
    private GameObject Hit_obj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Transform s , Player_skill_class sk , GameObject obj)
    {
        source = s;
        _skill_class = sk;
        Hit_obj = obj;
        isStart = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isStart)
            return;
        // 检查进入的是否为敌人，这里假设敌人有 "Enemy" 标签
        if (other.CompareTag("Enemy"))
        {
            E_base _base = other.GetComponent<E_base>();
            _base.Get_damage(source, _skill_class);
            Vector3 enemyPosition = other.transform.position;
            Vector3 playerPosition = transform.position;
            // 将 y 坐标设为相同，忽略高度差
            enemyPosition.y = playerPosition.y;
            // 计算并标准化方向向量，仅考虑水平平面
            Vector3 pushDirection = (enemyPosition - playerPosition).normalized;
            Vector3 force = pushDirection * pushForce;
            _base.ApplyPush(force);
            Instantiate(Hit_obj, other.transform.position, other.transform.rotation);
        }
    }

}
