using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExistTime : MonoBehaviour
{
    public C_base player;          // 玩家引用
    public Text timeText;          // 显示存活时间的UI文本

    private float survivalTime;    // 存活时间

    void Start()
    {
        // 获取玩家组件
        player = GameObject.Find("Player")?.GetComponent<C_base>();
        survivalTime = 0f; // 初始化存活时间
    }

    void Update()
    {
        if (player == null || timeText == null) return;

        // 如果玩家未死亡，增加存活时间
        if (!player.isDie)
        {
            survivalTime += Time.deltaTime;
        }
        else
        {
            // 玩家死亡时停止更新
            return;
        }

        // 更新UI显示，格式化为 mm:ss
        timeText.text =FormatTime(survivalTime);
    }

    // 格式化时间为 mm:ss
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60); // 转换分钟
        int seconds = Mathf.FloorToInt(time % 60); // 转换秒
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
