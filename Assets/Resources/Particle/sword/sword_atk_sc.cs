using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class sword_atk_sc : MonoBehaviour
{

    private Transform source;
    public Player_skill_class _skill_class;
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

    public void Init(Transform s, Player_skill_class sk, GameObject obj)
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
            GameObject fb = Instantiate(Hit_obj, other.transform.position, other.transform.rotation);
        //    StartCoroutine(delay_destory( 0.2f , fb));
        }

    }

    IEnumerator delay_destory(float time, GameObject FB)
    {
        yield return new WaitForSeconds(time);
        Destroy(FB);
    }

}
