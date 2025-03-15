using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeUpgrade : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] sprites;
    public C_base player;
    public Image dengji;
    void Start()
    {
        player = GameObject.Find("Player")?.GetComponent<C_base>();
    }

    // Update is called once per frame
    void Update()
    {
        Changed();
    }

    public void Changed()
    {
        if (player != null) {
            dengji.sprite = sprites[player.skill_class[0].Lv-1];
        }
    }
}
