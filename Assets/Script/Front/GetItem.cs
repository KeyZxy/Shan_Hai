using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItem : MonoBehaviour
{
    public C_upgrade_attr player; // �������  
    // ���߿�����  
    public Image daoju1;
    public Image daoju2;
    public Image daoju3;
    public Image daoju4;
    public Image daoju5;
    public Image daoju6;

    // ���Ե���  
    public Sprite[] sprites;


    private Dictionary<int, Sprite> idToImage; // ID ��ͼ���ӳ��  
    private Image[] Frames; // ���Ե��߿�����  

    // ���߿��Ƿ�����  
    private bool isFilled;

    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_upgrade_attr>();

        idToImage = new Dictionary<int, Sprite>
        {
            { 200001, sprites[0] },//qingzhiying
            { 200002,  sprites[1]},//ruyi
            { 200003, sprites[2] },//shenmuqin
            { 200004, sprites[3] },//jiake
            { 200008, sprites[4] },//yazhui
            { 200012, sprites[5] }, //liulizhu
            {200009, sprites[6] },//ling
            {200010,sprites[7] },//nang
            {200014,sprites[8] },//yan
            {200017,sprites[9] }//tai
        };

        Frames = new Image[] { daoju1, daoju2, daoju3, daoju4, daoju5, daoju6 };
        
    }

    void Update()
    {
        // ���ڵ��߿�δ��ʱ����ͼ��  
        if (player != null && player.infos != null)
        {
            UpdateItemIcons();
        }
    }

    private void UpdateItemIcons()
    {
        // ������Ϣ������ͼ��  
        for (int i = 0; i < player.infos.Count && i < Frames.Length; i++)
        {
            var Info = player.infos[i];
            if (idToImage.TryGetValue(Info.attr_ID, out Sprite newSprite))
            {
                Frames[i].sprite = newSprite; // �����µ�ͼ��  
            }
        }
    }

   
}