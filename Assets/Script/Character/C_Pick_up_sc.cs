using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class C_Pick_up_sc : MonoBehaviour
{

    private C_attribute _attr;

    // Start is called before the first frame update
    void Start()
    {
        _attr = GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_attribute>();
        Re_size(_attr.Get_PickUp_distance());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Re_size(int s)
    {
        transform.localScale = new Vector3(s, s, s);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的是否为敌人，这里假设敌人有 "Enemy" 标签
        if (other.CompareTag(SaveKey.Exp))
        {
            other.GetComponent<Exp_sc>().pick_up(transform);

        }
    }

}
