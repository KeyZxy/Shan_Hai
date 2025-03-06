using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class jiguangbo_sc : MonoBehaviour
{

    private Transform source;
    private Player_skill_class _skill_class;
    private List<Transform> enemys = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Transform s, Player_skill_class sk)
    {
        source = s;
        _skill_class = sk;
    }

    private void OnTriggerStay(Collider other)
    {
        // 检查进入的是否为敌人，这里假设敌人有 "Enemy" 标签
        if (other.CompareTag("Enemy"))
        {
            // 判断是否已经再列表里面
            if (!enemys.Contains(other.transform))
            {
                enemys.Add(other.transform);
                float se = 1f / _skill_class.count;
                StartCoroutine(delay_remove_enemy( se , other.transform));

                other.GetComponent<E_base>().Get_damage(source, _skill_class);
                GameObject shouji_M = ResourceManager.Instance.GetResource<GameObject>("Particle/jiguangbo/shouji_M");
                GameObject.Instantiate(shouji_M, other.transform.position, shouji_M.transform.rotation);
            }



        }
    }

    IEnumerator delay_remove_enemy(float time , Transform tr)
    {
        yield return new WaitForSeconds(time);
        enemys.Remove(tr);
    }
}
