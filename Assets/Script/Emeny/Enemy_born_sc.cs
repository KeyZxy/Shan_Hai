using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_born_sc : MonoBehaviour
{

    public GameObject Enemy_obj;
    public float Live_time;
    public float Interval;
    public float Count;
    public float Speed_up_value;
    public float Speed_up_interval;
    public float Speed_up_count;
    public List<GameObject> Posi_s = new List<GameObject>();

    private float Original_live_time;
    private bool isStart = false;
    private float Threshold;
    private float spawnDelay = 0.2f;
    private bool speed_up_mode = false;
    private Coroutine spawnCoroutine;
    private bool isStop = false;

    // Start is called before the first frame update
    void Start()
    {
        Original_live_time = Live_time;
        Threshold = Live_time * (Speed_up_value / 100f);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStop)
            return;
        if(isStart)
        {
            Live_time -= Time.deltaTime;
            if (Live_time <= Threshold && !speed_up_mode)
            {
                //开启狂暴模式
                speed_up_mode = true;
                
            }
            if(Live_time <= 0)
            {
                //结束刷怪
                isStop = true;
                StopCoroutine(spawnCoroutine);
            }
                
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawn_count;
        if (speed_up_mode)
            spawn_count = Speed_up_count;
        else
            spawn_count = Count;

        for (int i = 0; i < spawn_count; i++)
        {
            // 在 Posi 列表中随机选择一个位置
            int randomIndex = Random.Range(0, Posi_s.Count);
            Vector3 spawnPosition = Posi_s[randomIndex].transform.position;

            // 生成敌人
            Instantiate(Enemy_obj, spawnPosition, Quaternion.identity, GameObject.Find("Enemy_group").transform);

            // 等待 spawnDelay 秒后再生成下一个敌人
            yield return new WaitForSeconds(spawnDelay);
        }
    }


    private IEnumerator Loop_Spawn()
    {
        while (true)
        {
            StartCoroutine(SpawnEnemy());
            if(!speed_up_mode)
                yield return new WaitForSeconds(Speed_up_interval);
            else
                yield return new WaitForSeconds(Interval);

        }
    }

    public void start_born()
    {
        if (!isStart)
        {
            spawnCoroutine = StartCoroutine(Loop_Spawn());
            isStart = true;
        }

    }


}
