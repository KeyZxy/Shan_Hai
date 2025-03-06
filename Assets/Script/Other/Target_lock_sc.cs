using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target_lock_sc : MonoBehaviour
{

    private Image _image;
    private Transform _target;

    private Vector3 offset = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        _image = transform.GetComponent<Image>();
        _image.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_target != null)
        {
            // 启用 UI
            _image.enabled = true;

            Vector3 posi = _target.Find("Hit_posi").transform.position;
            // 将目标的世界坐标转换为屏幕坐标
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(posi + offset);

            // 更新 UI 的位置
            transform.position = screenPosition;

            transform.Rotate(0, 0, Time.deltaTime * 20); // 每秒旋转20度
        }
    }

    public void Set_target(Transform tr)
    {
        if(tr == null)
            return;
        _target = tr;
        _image.enabled = true;
    }

    public void Remove_target()
    {
        _target = null;
        _image.enabled = false;
    }

}
