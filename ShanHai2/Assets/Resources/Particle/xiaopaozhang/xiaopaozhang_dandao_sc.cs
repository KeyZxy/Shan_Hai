using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xiaopaozhang_dandao_sc : MonoBehaviour
{

    private float flightDuration = 0.3f; // 飞行总时间
    private float height = 3f;        // 抛物线最高点

    private Transform _source;
    private Player_skill_class _skill_class;
    private Transform _target = null;
    private GameObject _hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Transform s , Player_skill_class skill , Transform tr , GameObject hit)
    {
        _source = s;
        _target = tr;
        _hit = hit;
        _skill_class = skill;
        if (tr == null)
            return;
        StartCoroutine(ParabolicMotion(transform, flightDuration, height));
    }

    private IEnumerator ParabolicMotion(Transform projectile, float duration, float maxHeight)
    {
        Vector3 startPos = projectile.position;
        float time = 0f;

        while (time < duration)
        {
            if (_target == null)
            {
                Destroy(gameObject);
                yield return null;
            }
            time += Time.deltaTime;
            float t = time / duration; // 归一化时间（0 到 1）

            // 计算 XZ 平面位置
            Vector3 horizontalPos = Vector3.Lerp(startPos, _target.position, t);

            // 计算抛物线的 Y 轴高度
            float parabolaHeight = maxHeight * (1 - (2 * t - 1) * (2 * t - 1)); // 抛物线公式

            // 更新子弹位置
            projectile.position = new Vector3(horizontalPos.x, startPos.y + parabolaHeight, horizontalPos.z);

            yield return null;
        }
        // 确保最后位置精确落到目标点
        projectile.position = _target.position;
        Destroy(projectile.gameObject);
        GameObject Hit_FB = Instantiate(_hit, _target.position, _hit.transform.rotation);
        Hit_FB.transform.GetComponent<xiaopaozhang_atk_sc>().Init(_source, _skill_class);

    }

}
