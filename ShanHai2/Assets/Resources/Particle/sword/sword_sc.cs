using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class sword_sc : MonoBehaviour
{
    public List<GameObject> posiList = new List<GameObject>();
    public GameObject Hit;
    public GameObject jian;


    private C_attribute _attr;
    private upgrade_info _skill_info;
    private Transform _target;
    private bool isStart = false;
    private float heightOffset = 1f;
    private float rotate_speed = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            float speed_ex = _skill_info.type_skill.move_speed;
            transform.position = new Vector3(_target.position.x, _target.position.y + heightOffset, _target.position.z);
            Vector3 rotationAmount = new Vector3(0, rotate_speed + speed_ex, 0) * Time.deltaTime;
            transform.Rotate(rotationAmount, Space.Self);
        }
    }

    public void Init(C_attribute attr, upgrade_info skill_info, Transform tr)
    {
        _target = tr;
        _attr = attr;
        _skill_info = skill_info;
        isStart = true;
        StartCoroutine(Loop_Atk(_skill_info.type_skill.CD - _attr.Get_cool_down()));

    }

    private IEnumerator Loop_Atk(float time)
    {
        while (true)
        {
            Create_atk(jian);
            float interval_time = _skill_info.type_skill.CD - _attr.Get_cool_down();
            yield return new WaitForSeconds(interval_time);
            


        }
    }

    private void Create_atk(GameObject obj)
    {
        int count = _skill_info.type_skill.count + _attr.Get_atk_count();
        for (int i = 0; i < count; i++)
        {
            GameObject FB = Instantiate(obj, posiList[i].transform.position, posiList[i].transform.rotation);
            FB.transform.parent = posiList[i].transform;
            FB.transform.GetComponent<sword_atk_sc>().Init(_target, _skill_info.type_skill, Hit);
            float timer = _attr.Get_atk_duration() + _skill_info.type_skill.duration;
            StartCoroutine(delay_destory(timer , FB));
        }
    }

    IEnumerator delay_destory(float time, GameObject FB)
    {
        yield return new WaitForSeconds(time);
        Destroy(FB);
    }

}
