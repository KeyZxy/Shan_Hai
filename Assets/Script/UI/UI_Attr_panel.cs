using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Attr_panel : MonoBehaviour
{


    public Text Exp;
    public Text Hp;
    public Text Recover;
    public Text Atk;
    public Text Def;
    public Text Avoid;
    public Text Crit;
    public Text Crit_atk;
    public Text Atk_speed;
    public Text CD;
    public Text count;
    public Text Duration;
    public Text Move_speed;
    public Text Atk_distance;
    public Text pickup_distance;
    public Text gold_extra;
    public Text luck;
    public Text ex_extra;

    private int frameInterval = 10; // 每 N 帧执行一次
    private C_attribute _attr;

    // Start is called before the first frame update
    void Start()
    {
        _attr = GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_attribute>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf)
        {
            if (Time.frameCount % frameInterval == 0)
            {
                Update_value();
            }
        }
    }


    void Update_value()
    {
        string temp = _attr.Get_max_ex().ToString() + "/" + _attr.Get_current_ex().ToString();
        Exp.text = temp;

        temp = _attr.Get_max_hp().ToString() + "/" + _attr.Get_hp().ToString();
        Hp.text = temp;

        Recover.text = _attr.Get_mp_recovery().ToString();

        Atk.text = _attr.Get_atk().ToString();

        Def.text = _attr.Get_def().ToString();

        Avoid.text = _attr.Get_avoid().ToString() + "%";

        Crit.text = _attr.Get_crit().ToString() + "%";

        Crit_atk.text = _attr.Get_crit_atk().ToString() + "%";

        Atk_speed.text = _attr.Get_attack_speed().ToString();

        CD.text = _attr.Get_cool_down().ToString();

        count.text = _attr.Get_atk_count().ToString();

        Duration.text = _attr.Get_atk_duration().ToString();

        Move_speed.text = _attr.Get_move_speed().ToString();

        Atk_distance.text = _attr.Get_attack_distance().ToString();

        pickup_distance.text = _attr.Get_PickUp_distance().ToString();

        gold_extra.text = _attr.Get_Gold_extra().ToString();

        luck.text = _attr.Get_luck().ToString();

        ex_extra.text = _attr.Get_ex_extra().ToString();
    }
}
