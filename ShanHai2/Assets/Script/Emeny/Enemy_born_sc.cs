using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_born_sc : MonoBehaviour
{

    public Text born_text;
    public GameObject Enemy;
    public float interval = 2f;

    private bool isStop = true;
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Key_Check();
        if(!isStop)
        {
            timer += Time.deltaTime;

            // 当计时器超过生成间隔时
            if (timer >= interval)
            {
                SpawnEnemy(); // 生成敌人
                timer = 0f; // 重置计时器
            }
        }
    }

    void SpawnEnemy()
    {
        Instantiate(Enemy, transform.position, Quaternion.identity , GameObject.Find("Enemy_group").transform);
    }


    void Key_Check()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if(isStop)
            {
                isStop = false;
                born_text.text = "F2刷怪-开启";
            }else
            {
                isStop = true;
                born_text.text = "F2刷怪-关闭";
            }
        }
    }
}
