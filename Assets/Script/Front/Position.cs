using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    RectTransform rectTransform; void Start()
    {
        rectTransform = GetComponent<RectTransform>();// 获取屏幕的宽度和高度
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;// 设置血条的位置：距离屏幕左下角的5%（相对于屏幕尺寸）
        float offsetX = screenWidth * 0.05f;  // 左边缘5%
        float offsetY = screenHeight * 0.05f; // 下边缘5%// 更新血条的位置
        rectTransform.anchorMin = new Vector2(0, 0);  // 锚点设置为左下角
        rectTransform.anchorMax = new Vector2(0, 0);  // 锚点设置为左下角
        rectTransform.pivot = new Vector2(0, 0);     // pivot设置为左下角
        rectTransform.anchoredPosition = new Vector2(offsetX, offsetY); // 设置实际位置
    }
}
