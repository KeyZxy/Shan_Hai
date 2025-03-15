using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExistTime : MonoBehaviour
{
    public C_base player;          // �������
    public Text timeText;          // ��ʾ���ʱ���UI�ı�

    private float survivalTime;    // ���ʱ��

    void Start()
    {
        // ��ȡ������
        player = GameObject.Find("Player")?.GetComponent<C_base>();
        survivalTime = 0f; // ��ʼ�����ʱ��
    }

    void Update()
    {
        if (player == null || timeText == null) return;

        // ������δ���������Ӵ��ʱ��
        if (!player.isDie)
        {
            survivalTime += Time.deltaTime;
        }
        else
        {
            // �������ʱֹͣ����
            return;
        }

        // ����UI��ʾ����ʽ��Ϊ mm:ss
        timeText.text =FormatTime(survivalTime);
    }

    // ��ʽ��ʱ��Ϊ mm:ss
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60); // ת������
        int seconds = Mathf.FloorToInt(time % 60); // ת����
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
