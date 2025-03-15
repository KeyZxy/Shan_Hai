using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItem : MonoBehaviour
{
    public Sprite[] sprites1;
    public Sprite[] sprites2;

    public C_upgrade_attr player;
    public Image Item1;
    public Image Item2;
    public Image Item3;
    public Image Item4;
    public Image Item5;
    public Image Item6;

    private Image[] itemImages; // 存储所有的图标  

    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_upgrade_attr>();
        itemImages = new Image[] { Item1, Item2, Item3, Item4, Item5, Item6 };

        // 初始时将所有 Item 隐藏  
        foreach (var item in itemImages)
        {
            item.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame  
    void Update()
    {
        Changed();
    }

    public void Changed()
    {
        if (player != null)
        {
            // 切换逻辑  
            for (int i = 0; i < player.infos.Count; i++)
            {
                var itemInfo = player.infos[i];

                // 如果当前 itemInfo 不在范围内, 跳过该循环  
                if (i >= itemImages.Length) continue;

                // 根据 attr_ID 切换 sprites  
                if (itemInfo.attr_ID == 200010) // 假设是发射数量   
                {
                    itemImages[i].sprite = sprites2[itemInfo.type_attr.level - 1];
                }
                else // 使用 sprites1  
                {
                    itemImages[i].sprite = sprites1[itemInfo.type_attr.level - 1];
                }

                // 显示当前的 Item  
                itemImages[i].gameObject.SetActive(true);
            }

            // 如果 player.infos 的数量小于 itemImages.length 需要隐藏多余的 Items  
            for (int i = player.infos.Count; i < itemImages.Length; i++)
            {
                itemImages[i].gameObject.SetActive(false);
            }
        }
    }
}