using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Ex : MonoBehaviour
{
    public C_attribute player;
    private int maxex;
    private float exBefore;

    public float speed = 5f; // 过渡速度  
    private Image image;
    public Text text;
    private bool isResetting = false; // 是否正在清空经验条  

    void Start()
    {
        image = GetComponent<Image>();
        player = GameObject.Find("Player")?.GetComponent<C_attribute>();

        if (player != null)
        {
            exBefore = player.Get_current_ex();
            maxex = player.Get_max_ex();
        }
    }

    void Update()
    {
        if (player == null) return;

        int ex = player.Get_current_ex();
        maxex = Mathf.Max(player.Get_max_ex(), 1); // 防止 maxex == 0  

        if (ex >= maxex)
        {
            isResetting = true; // 经验值满，开始清空  
        }

        float deltaTime = (Time.timeScale > 0) ? Time.deltaTime : Time.unscaledDeltaTime;

        if (isResetting)
        {
            // 如果正在清空经验条，进行平滑过渡  
            exBefore = Mathf.Lerp(exBefore, 0, deltaTime * speed);
            if (exBefore <= 0.01f) // 避免浮点数误差  
            {
                exBefore = 0;
                isResetting = false; // 清空完成，恢复正常经验更新  
            }
        }
        else
        {
            // 正常更新经验值  
            ex = Mathf.Clamp(ex, 0, maxex);
            exBefore = Mathf.Lerp(exBefore, ex, deltaTime * speed);
        }

        image.fillAmount = exBefore / maxex;
        // 更新血量文本  
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if (text != null)
        {
            text.text = Mathf.RoundToInt(exBefore).ToString() + " / " + maxex.ToString();
        }
    }

}
