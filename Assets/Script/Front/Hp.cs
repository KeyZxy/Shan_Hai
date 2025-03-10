using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hp : MonoBehaviour
{
    public C_attribute player;
    private int maxhp;
    private float hpBefore; // 记录上一帧的血量
    public float speed = 5f; // 过渡速度
    private Image image;
    public Text text;

    void Start()
    {
        image = GetComponent<Image>();
        player = GameObject.Find("Player")?.GetComponent<C_attribute>();

        if (player != null)
        {
            hpBefore = player.Get_hp();
            maxhp = Mathf.Max(player.Get_max_hp(), 1); // 确保 maxhp 至少为 1
        }
    }

    void Update()
    {
        if (player == null) return;

        int hp = player.Get_hp();
        maxhp = Mathf.Max(player.Get_max_hp(), 1); // 防止除以 0

        hp = Mathf.Clamp(hp, 0, maxhp); // 确保血量在有效范围

        hpBefore = Mathf.Lerp(hpBefore, hp, Time.deltaTime * speed); // 平滑过渡

        image.fillAmount = hpBefore / maxhp;

        // 更新血量文本
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (text != null)
        {
            text.text = Mathf.RoundToInt(hpBefore).ToString() + " / " + maxhp.ToString();
        }
    }
}
