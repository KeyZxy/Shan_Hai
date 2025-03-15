using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetPassive : MonoBehaviour
{
    public C_upgrade_attr player;  // 玩家被动技能的引用  

    // 被动技能框坐标  
    public Image beidong1;
    public Image beidong2;
    public Image beidong3;

    // 被动技能图标  
    public Sprite[] sprites; 

    private Dictionary<int, Sprite> idToImage; // ID 到图标的映射  
    private Image[] passiveImages; // 用于存储被动技能框的 Image 组件  

    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_upgrade_attr>();

        idToImage = new Dictionary<int, Sprite>
        {
            { 220003, sprites[0] }, // 水环  
            { 220001, sprites[1] }, // 剑环  
            { 220002, sprites[2] }  // 小炮仗  
        };

        passiveImages = new Image[] { beidong1, beidong2, beidong3 };
    }

    void Update()
    {
        if (player != null && player.passive_infos != null)
        {
            UpdatePassiveSkills();
        }
    }

    private void UpdatePassiveSkills()
    {
       

        // 遍历被动技能信息并更新图标  
        for (int i = 0; i < player.passive_infos.Count && i < passiveImages.Length; i++)
        {
            var passiveInfo = player.passive_infos[i];
            if (idToImage.TryGetValue(passiveInfo.attr_ID, out Sprite newSprite))
            {
                passiveImages[i].sprite = newSprite; // 设置新的图标  
            }
        }
    }
}