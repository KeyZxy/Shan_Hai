using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    public C_upgrade_attr player; // 玩家属性  
    // 道具框坐标  
    public RectTransform daoju1;
    public RectTransform daoju2;
    public RectTransform daoju3;
    public RectTransform daoju4;
    public RectTransform daoju5;
    public RectTransform daoju6;

    // 属性道具  
    public GameObject liulizhu;    
    public GameObject shenmuqin;    
    public GameObject qingzhiying;    
    public GameObject ruyi;          
    public GameObject jiake;         
    public GameObject yazhui;       


    private Dictionary<int, GameObject> idToImage; // ID 到图标的映射  
    private RectTransform[] Frames; // 属性道具框数组  

    // 道具框是否已满  
    private bool isFilled;

    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_upgrade_attr>();

        idToImage = new Dictionary<int, GameObject>
        {
            { 200001, qingzhiying },
            { 200002, ruyi },
            { 200003, shenmuqin },
            { 200004, jiake },
            { 200008, yazhui },
            { 200012, liulizhu }
        };

        Frames = new RectTransform[] { daoju1, daoju2, daoju3, daoju4, daoju5, daoju6 };
        isFilled = false; // 初始化道具框不满  
        UpdateItemIcons();
    }

    void Update()
    {
        // 仅在道具框未满时更新图标  
        if (!isFilled)
        {
            UpdateItemIcons();
        }
    }

    private void UpdateItemIcons()
    {
        // 如果道具框已满，直接返回  
        if (isFilled) return;

        // 清除已有的道具图标  
        ClearItemIcons();

        // 逐个显示 infos 列表中的有效道具信息  
        int displayedCount = 0; // 已经显示的道具数量  

        foreach (var itemInfo in player.infos)
        {
            // 检查道具 ID 是否在映射中  
            if (idToImage.ContainsKey(itemInfo.attr_ID) && displayedCount < Frames.Length)
            {
                GameObject itemImage = idToImage[itemInfo.attr_ID];

                // 显示道具图标到对应的道具框  
                itemImage.SetActive(true);
                itemImage.transform.position = Frames[displayedCount].position;

                // 检查是否是神木琴，如果是则偏移 5 个单位  
                if (itemInfo.attr_ID == 200003) // 假设 200003 是神木琴的 ID  
                {
                    RectTransform rectTransform = itemImage.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition += new Vector2(5, 5); // 右上角偏移 5 个单位  
                }

                displayedCount++;
            }

            // 如果已显示六个道具，跳出循环  
            if (displayedCount >= Frames.Length)
            {
                break;
            }
        }

        // 检查是否已填满所有框  
        isFilled = displayedCount >= Frames.Length;
    }

    private void ClearItemIcons()
    {
        // 清除已有的道具图标  
        foreach (var itemImage in idToImage.Values)
        {
            itemImage.SetActive(false);
        }
    }
}