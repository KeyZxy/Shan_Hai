using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GetPassive : MonoBehaviour
{
    public C_upgrade_attr player;  // ��ұ������ܵ�����  

    // �������ܿ�����  
    public RectTransform beidong1;
    public RectTransform beidong2;
    public RectTransform beidong3;

    // ��������ͼ��  
    public GameObject shui; // ˮ��ͼ��  
    public GameObject jian; // ����ͼ��  
    public GameObject deng; // С����ͼ��  

    private Dictionary<int, GameObject> idToImage; // ID ��ͼ���ӳ��  
    private RectTransform[] passiveFrames; // �������ܿ�����  

    // �������Ƿ�����  
    private bool isPassiveFilled;

    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_upgrade_attr>();

        idToImage = new Dictionary<int, GameObject>
        {
            { 220003, shui }, // ˮ��  
            { 220001, jian }, // ����  
            { 220002, deng }  // С����  
        };

        passiveFrames = new RectTransform[] { beidong1, beidong2, beidong3 };

        // ��ʼ��ͼ��Ϊ������  
        ClearPassiveIcons();
        isPassiveFilled = false; // ��ʼʱ��������  
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
        // ���������������ֱ�ӷ���  
        if (isPassiveFilled)
        {
            return;
        }

        ClearPassiveIcons();

        // ���±�����ͼ��  
        // �����ʾ infos �б��е���Ч������Ϣ  
        int displayedCount = 0; // �Ѿ���ʾ�ĵ�������  

        foreach (var passiveInfo in player.passive_infos)
        {
            
            if (idToImage.ContainsKey(passiveInfo.attr_ID) && displayedCount < passiveFrames.Length)
            {
                GameObject skillImage = idToImage[passiveInfo.attr_ID];

                // ���ü���ͼ��λ�õ���Ӧ�ı������ܿ�  
                skillImage.SetActive(true);
                skillImage.transform.position = passiveFrames[displayedCount].position;
                displayedCount++;
            }

            if (displayedCount >= passiveFrames.Length)
            {
                break;
            }
        }

        // ������������������ñ�־  
        isPassiveFilled = displayedCount >= passiveFrames.Length;
    }

    private void ClearPassiveIcons()
    {
        // ������еı�������ͼ��  
        foreach (var skillImage in idToImage.Values)
        {
            skillImage.SetActive(false);
        }
    }
}