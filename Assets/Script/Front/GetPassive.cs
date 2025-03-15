using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GetPassive : MonoBehaviour
{
    public C_upgrade_attr player;  // 玩家被动技能的引用  

    // 被动技能框坐标  
    public RectTransform beidong1;
    public RectTransform beidong2;
    public RectTransform beidong3;

    // 被动技能图标  
    public GameObject shui; // 水环图标  
    public GameObject jian; // 剑环图标  
    public GameObject deng; // 小炮仗图标  

    private Dictionary<int, GameObject> idToImage; // ID 到图标的映射  
    private RectTransform[] passiveFrames; // 被动技能框数组  

    // 被动框是否已满  
    private bool isPassiveFilled;

    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_upgrade_attr>();

        idToImage = new Dictionary<int, GameObject>
        {
            { 220003, shui }, // 水环  
            { 220001, jian }, // 剑环  
            { 220002, deng }  // 小炮仗  
        };

        passiveFrames = new RectTransform[] { beidong1, beidong2, beidong3 };

        // 初始化图标为不激活  
        ClearPassiveIcons();
        isPassiveFilled = false; // 初始时被动框不满  
    }

    void Update()
    {
        if (player != null && player.passive_infos != null)
        {
            UpdatePassiveIcons();
        }
    }

    private void UpdatePassiveIcons()
    {
        // 如果被动框已满，直接返回  
        if (isPassiveFilled)
        {
            return;
        }

        ClearPassiveIcons();

        // 更新被动框图标  
        // 逐个显示 infos 列表中的有效道具信息  
        int displayedCount = 0; // 已经显示的道具数量  

        foreach (var passiveInfo in player.passive_infos)
        {
            
            if (idToImage.ContainsKey(passiveInfo.attr_ID) && displayedCount < passiveFrames.Length)
            {
                GameObject skillImage = idToImage[passiveInfo.attr_ID];

                // 设置技能图标位置到对应的被动技能框  
                skillImage.SetActive(true);
                skillImage.transform.position = passiveFrames[displayedCount].position;
                displayedCount++;
            }

            if (displayedCount >= passiveFrames.Length)
            {
                break;
            }
        }

        // 如果被动框已满，设置标志  
        isPassiveFilled = displayedCount >= passiveFrames.Length;
    }

    private void ClearPassiveIcons()
    {
        // 清除已有的被动技能图标  
        foreach (var skillImage in idToImage.Values)
        {
            skillImage.SetActive(false);
        }
    }
}