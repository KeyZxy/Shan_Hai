using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    public C_upgrade_attr player; // �������  
    // ���߿�����  
    public RectTransform daoju1;
    public RectTransform daoju2;
    public RectTransform daoju3;
    public RectTransform daoju4;
    public RectTransform daoju5;
    public RectTransform daoju6;

    // ���Ե���  
    public GameObject liulizhu;    
    public GameObject shenmuqin;    
    public GameObject qingzhiying;    
    public GameObject ruyi;          
    public GameObject jiake;         
    public GameObject yazhui;       


    private Dictionary<int, GameObject> idToImage; // ID ��ͼ���ӳ��  
    private RectTransform[] Frames; // ���Ե��߿�����  

    // ���߿��Ƿ�����  
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
        isFilled = false; // ��ʼ�����߿���  
        UpdateItemIcons();
    }

    void Update()
    {
        // ���ڵ��߿�δ��ʱ����ͼ��  
        if (!isFilled)
        {
            UpdateItemIcons();
        }
    }

    private void UpdateItemIcons()
    {
        // ������߿�������ֱ�ӷ���  
        if (isFilled) return;

        // ������еĵ���ͼ��  
        ClearItemIcons();

        // �����ʾ infos �б��е���Ч������Ϣ  
        int displayedCount = 0; // �Ѿ���ʾ�ĵ�������  

        foreach (var itemInfo in player.infos)
        {
            // ������ ID �Ƿ���ӳ����  
            if (idToImage.ContainsKey(itemInfo.attr_ID) && displayedCount < Frames.Length)
            {
                GameObject itemImage = idToImage[itemInfo.attr_ID];

                // ��ʾ����ͼ�굽��Ӧ�ĵ��߿�  
                itemImage.SetActive(true);
                itemImage.transform.position = Frames[displayedCount].position;

                // ����Ƿ�����ľ�٣��������ƫ�� 5 ����λ  
                if (itemInfo.attr_ID == 200003) // ���� 200003 ����ľ�ٵ� ID  
                {
                    RectTransform rectTransform = itemImage.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition += new Vector2(5, 5); // ���Ͻ�ƫ�� 5 ����λ  
                }

                displayedCount++;
            }

            // �������ʾ�������ߣ�����ѭ��  
            if (displayedCount >= Frames.Length)
            {
                break;
            }
        }

        // ����Ƿ����������п�  
        isFilled = displayedCount >= Frames.Length;
    }

    private void ClearItemIcons()
    {
        // ������еĵ���ͼ��  
        foreach (var itemImage in idToImage.Values)
        {
            itemImage.SetActive(false);
        }
    }
}