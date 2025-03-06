using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class eye_lock_sc : MonoBehaviour
{

    private Camera _mainCamera; // 摄像机，负责将3D物体转换到屏幕空间
    private Transform _target; // 目标3D物体的Transform
    private RectTransform _uiElement; // UI元素的RectTransform
    private Image _image;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _uiElement = GetComponent<RectTransform>();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_target != null)
        {
            // 将目标物体的世界坐标转换为屏幕坐标
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(_target.position);

            // 如果目标在摄像机前面（z轴正值）
            if (screenPos.z > 0)
            {
                // 将UI元素的屏幕坐标设置为目标的屏幕坐标
                _uiElement.position = screenPos;
            }
        }
    }

    public void Hide_eye()
    {
        _target = null;
        _image.enabled = false;
    }

    public void Show_eye(Transform tr)
    {
        _target = tr;
        _image.enabled = true;
    }

}
