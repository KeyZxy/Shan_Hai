using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fu_sc : MonoBehaviour
{



    private GameObject _target;
    private bool launch = false;
    private Transform source;

    public float arcHeight_nim = 3f;  // 控制飞行轨迹的弧度
    public float arcHeight_max = 5f;  // 控制飞行轨迹的弧度
    public float randomOffsetRange = 3f; // 控制点的随机偏移范围
    public Player_skill_class _skill_class;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float t = 0f; // 用于插值的时间变量
    private Vector3 controlPoint; // 控制点
    private float randomMoveSpeed;


    void Start()
    {
        // 初始化起点和目标点的位置
        startPosition = transform.position;
        if (_target != null)
            targetPosition = _target.transform.position;
        else
            Destroy(gameObject);


        // 随机生成控制点
        GenerateRandomControlPoint();
        randomMoveSpeed = _skill_class.move_speed + Random.Range(-2f, 3f);
    }

    void Update()
    {
        if (launch)
        {
            if (_target != null)
            {
                transform.LookAt(_target.transform);
                // 飞行轨迹带弧度：通过二次贝塞尔曲线进行插值
                t += Time.deltaTime * (randomMoveSpeed / Vector3.Distance(startPosition, targetPosition));


                // 计算贝塞尔曲线上当前位置
                Vector3 currentPosition = CalculateBezierPoint(t, startPosition, controlPoint, targetPosition);


                // 更新物体位置
                transform.position = currentPosition;

                // 让物体的朝向跟随飞行方向
                Vector3 direction = (currentPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);

                // 当物体到达目标位置时销毁或停止
                if (t >= 1f)
                {
                    transform.position = targetPosition; // 确保物体最后位置准确
                                                         //    Destroy(gameObject); // 或者停止飞行
                    StartCoroutine(delay_destory(0.8f, gameObject));
                }
            }
            else
            {
                Destroy(gameObject);
            }

        }

    }

    public void Init(GameObject t, Transform s , Player_skill_class sk)
    {
        _target = t;
        launch = true;
        source = s;
        _skill_class = sk;
    }

    // 随机生成控制点
    void GenerateRandomControlPoint()
    {

        float arcHeight = Random.Range(arcHeight_nim, arcHeight_max);

        Vector3 midpoint = (startPosition + targetPosition) / 2;

        // 随机偏移控制点
        float randomX = Random.Range(-randomOffsetRange, randomOffsetRange);
        float randomY = Random.Range(0, arcHeight); // 可以控制弧度的高度
        float randomZ = Random.Range(-randomOffsetRange, randomOffsetRange);

        controlPoint = midpoint + new Vector3(randomX, randomY, randomZ);
    }


    // 二次贝塞尔曲线公式
    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的是否为敌人，这里假设敌人有 "Enemy" 标签
        if (other.CompareTag("Enemy"))
        {

            other.GetComponent<E_base>().Get_damage(source , _skill_class);
            GameObject FB = (GameObject)Resources.Load("Particle/Fu/end");
            GameObject go = GameObject.Instantiate(FB, other.transform.position, transform.localRotation);

            Destroy(gameObject);
        }
    }

    IEnumerator delay_destory(float time, GameObject FB)
    {
        yield return new WaitForSeconds(time);
        Destroy(FB);
    }

}
