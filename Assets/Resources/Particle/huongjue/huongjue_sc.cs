using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class huongjue_sc : MonoBehaviour
{

    private Transform source;
    private Player_skill_class _skill_class;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delay_destory(1f , gameObject));   
        SphereCollider collider_obj = transform.GetComponent<SphereCollider>();
        StartCoroutine(delay_destory_collider(0.3f));
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


    private void OnTriggerEnter(Collider other)
    {
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

    IEnumerator delay_destory_collider(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(transform.GetComponent<SphereCollider>());
    }


}
