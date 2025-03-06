using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class xiaopaozhang_atk_sc : MonoBehaviour
{

    private Transform source;
    private Player_skill_class _skill_class;



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
        StartCoroutine(delay_destory(0.2f , gameObject));
    }



    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的是否为敌人，这里假设敌人有 "Enemy" 标签
        if (other.CompareTag("Enemy"))
        {
            E_base _base = other.GetComponent<E_base>();
            _base.Get_damage(source, _skill_class);

        }
    }

    IEnumerator delay_destory(float time, GameObject FB)
    {
        yield return new WaitForSeconds(time);
        Destroy(FB);
    }

}
