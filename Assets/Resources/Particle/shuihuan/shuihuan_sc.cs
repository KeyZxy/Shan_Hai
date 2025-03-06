using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class shuihuan_sc : MonoBehaviour
{

    public GameObject obj_1;
    public GameObject obj_2;
    public GameObject obj_3;

    public GameObject Atk_1;
    public GameObject Atk_2;
    public GameObject Atk_3;

    public GameObject Hit;
    public GameObject start_obj;

    private C_attribute _attr;
    private upgrade_info _skill_info;
    private Transform _target;
    private bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isStart)
        {
            transform.position = _target.position;
        }

    }

    public void Init(C_attribute attr , upgrade_info skill_info , Transform tr)
    {
        _target = tr;
        _attr = attr;
        _skill_info = skill_info;
        GameObject FB = Instantiate(start_obj, transform.position, start_obj.transform.rotation, transform);
        isStart = true; 
        Show_area(_skill_info.type_skill.Lv);
        StartCoroutine(Loop_Atk(_skill_info.type_skill.CD - _attr.Get_cool_down()));
  

    }

    private IEnumerator Loop_Atk(float time)
    {
        while (true)
        {
            float interval_time = _skill_info.type_skill.CD - _attr.Get_cool_down();
            yield return new WaitForSeconds(interval_time);
            int lv = _skill_info.type_skill.Lv;
            GameObject obj = Get_obj(lv);
            StartCoroutine(Create_atk(obj));

            
        }
    }

    private IEnumerator Create_atk(GameObject obj)
    {
        int count = _skill_info.type_skill.count + _attr.Get_atk_count();
        for (int i = 0; i < count; i++)
        {
            GameObject FB = Instantiate(obj, transform.position, start_obj.transform.rotation, transform);
            FB.transform.GetComponent<shuihuan_atk_sc>().Init(_target,_skill_info.type_skill, Hit);
            StartCoroutine(delay_destory(0.4f, FB));
            yield return new WaitForSeconds(0.15f); // 每次循环间隔 0.15 秒
        }
    }

    void Show_area(int lv)
    {
        if (lv == 1 ||  lv == 2 || lv == 3 )
        {
            GameObject FB = Instantiate(obj_1 , transform.position, start_obj.transform.rotation, transform);
        }
        if (lv == 4 || lv == 5 || lv == 6)
        {
            GameObject FB = Instantiate(obj_2, transform.position, start_obj.transform.rotation, transform);
        }
        if (lv == 7 || lv == 8 || lv == 9)
        {
            GameObject FB = Instantiate(obj_3, transform.position, start_obj.transform.rotation, transform);
        }
    }



    GameObject Get_obj(int lv)
    {
        GameObject FB = null;
        if (lv == 1 || lv == 2 || lv == 3)
        {
            FB = Atk_1;
        }
        if (lv == 4 || lv == 5 || lv == 6)
        {
            FB = Atk_2;
        }
        if (lv == 7 || lv == 8 || lv == 9)
        {
            FB = Atk_3;
        }
        return FB;
    }

    IEnumerator delay_destory(float time , GameObject FB)
    {
        yield return new WaitForSeconds(time);
        Destroy(FB);
    }

}
