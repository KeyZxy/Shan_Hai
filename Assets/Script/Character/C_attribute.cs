using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class C_attribute : MonoBehaviour
{

    public Up_grade_panel_sc _upgrade;
    public biology_info c_Value = new biology_info();
    public int HUDT_offset = 100;

    private C_Pick_up_sc _pickup;
    private bool stop_moving = false;
    private C_upgrade_attr _up_attr;
    private C_passive _Passive;
    private C_anim _Anim;

    // Start is called before the first frame update
    void Start()
    {
        c_Value.hp = Get_max_hp();
        _pickup = transform.Find("Pick_up_area").GetComponent<C_Pick_up_sc>();
        _up_attr = transform.GetComponent<C_upgrade_attr>();
        _Passive = transform.GetComponent<C_passive>();
        _Anim = transform.GetComponent<C_anim>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Exp_up(int exp)
    {
        c_Value.current_ex = Get_current_ex() + exp;
        if(Get_current_ex() >= Get_max_ex())
        {
            //c_Value.max_ex *= 2;
            c_Value.max_ex = Mathf.RoundToInt(c_Value.max_ex * 1.3f);
            c_Value.current_ex = 0;
            _upgrade.Show_UI();
        }
    }

    public void Stop_move(bool s)
    {
        stop_moving = s;
    }


    public void C_Upgrade_Fun(upgrade_info info)
    {
        switch(info.type)
        {
            case skill_type.None:
                attr_Upgrade(info);
                break;
            case skill_type.Normal_atk:
                attr_Upgrade(info);
                break;
            case skill_type.Passive_atk:
                passive_Upgrade(info);
                break;
            case skill_type.Active_atk:

                break;
            case skill_type.Ultimate_atk:

                break;
        }
    }

    void passive_Upgrade(upgrade_info info)
    {
        _up_attr.Add_attr(info);

        switch (info.attr_ID)
        {
            case 220001:
                _Passive.StartPassiveAction(info.attr_ID);
                break;
            case 220002:
                _Passive.StartPassiveAction(info.attr_ID);
                break;
            case 220003:
                _Passive.StartPassiveAction(info.attr_ID);
                break;

        }

    }

    public bool Reduce_hp(int value, bool crit, bool isAvoid)
    {
        bool isDie = false;
        GameObject Hud_obj = ResourceManager.Instance.GetResource<GameObject>("Prefab/UI/HUDObj");
        GameObject go = null;
        if (Hud_obj != null)
        {
            go = GameObject.Instantiate(Hud_obj, Hud_obj.transform.position, Quaternion.identity);
        }
        if (isAvoid)
        {
            Hud_obj.GetComponent<HUDText>().Init(transform, value, 4, HUDT_offset);
            return false;
        }
        if (value == 0)
        {
            Hud_obj.GetComponent<HUDText>().Init(transform, value, 9, HUDT_offset);
            return false;
        }
        int hp = c_Value.hp - value;
        c_Value.hp = hp;
        if (crit)
        {
            go.GetComponent<HUDText>().Init(transform, value, 7, HUDT_offset);
        }
        else
        {
            go.GetComponent<HUDText>().Init(transform, value, 1, HUDT_offset);
        }
        if (c_Value.hp <= 0)
        {
            isDie = true;
            _Anim.change_anim(Anim_state.Die);
            transform.tag = "Untagged";
        //    C_ctr.enabled = false;

        }
        return isDie;
    }

    void attr_Upgrade(upgrade_info info)
    {
        switch (info.attr_ID)
        {
            case 200001:
                int maxhp = Mathf.RoundToInt(Get_max_hp() * (info.type_attr.max_hp / 100f));
                c_Value._upgrade.max_hp += maxhp;
                break;
            case 200002:
                c_Value._upgrade.mp_Recovery += info.type_attr.mp_Recovery;
                break;
            case 200003:
                int atk = Mathf.RoundToInt(Get_atk() * (info.type_attr.atk / 100f));
                c_Value._upgrade.atk += atk;
                break;
            case 200004:
                int def = Mathf.RoundToInt(Get_def() * (info.type_attr.def / 100f));
                c_Value._upgrade.def += def;
                break;
            case 200005:
                c_Value._upgrade.avoid += info.type_attr.avoid;
                break;
            case 200006:
                c_Value._upgrade.crit += info.type_attr.crit;
                break;
            case 200007:
                c_Value._upgrade.crit_Atk += info.type_attr.crit_Atk;
                break;
            case 200008:
                c_Value._upgrade.attack_speed += info.type_attr.attack_speed;
                break;
            case 200009:
                c_Value._upgrade.cool_down += info.type_attr.cool_down;
                break;
            case 200010:
                c_Value._upgrade.atk_count += info.type_attr.atk_count;
                break;
            case 200011:
                c_Value._upgrade.atk_duration += info.type_attr.atk_duration;
                break;
            case 200012:
                c_Value._upgrade.move_speed += info.type_attr.move_speed;
                break;
            case 200013:
                c_Value._upgrade.attack_distance += info.type_attr.attack_distance;
                break;
            case 200014:
                c_Value._upgrade.pick_distance += info.type_attr.pick_distance;
                _pickup.Re_size(Get_PickUp_distance());
                break;
            case 200015:
                c_Value._upgrade.gold_extra += info.type_attr.gold_extra;
                break;
            case 200016:
                c_Value._upgrade.luck += info.type_attr.luck;
                break;
            case 200017:
                c_Value._upgrade.ex_extra += info.type_attr.ex_extra;
                break;
            case 200018:
                // c_Value._upgrade.ex_extra += info.type_attr.ex_extra;
                return;
            case 200019:
                // c_Value._upgrade.ex_extra += info.type_attr.ex_extra;
                return;
            case 200020:
                // c_Value._upgrade.ex_extra += info.type_attr.ex_extra;
                return;
        }
        // 把已升级得属性加载到角色脚本中
        _up_attr.Add_attr(info);
        
    }


    // Get函数
    public string Get_ID()
    {
        return c_Value.ID;
    }
    public string Get_name()
    {
        return c_Value.name;
    }
    public int Get_grade()
    {
        return c_Value.grade;
    }
    public int Get_level()
    {
        return c_Value.level;
    }
    public int Get_current_ex()
    {
        return c_Value.current_ex;
    }
    public int Get_max_ex()
    {
        return c_Value.max_ex;
    }
    public int Get_max_hp()
    {
        return c_Value.max_hp + c_Value._equip.max_hp + c_Value._upgrade.max_hp + c_Value._buff.max_hp;
    }
    public int Get_hp()
    {
        return c_Value.hp;
    }
    public float Get_mp_recovery()
    {
        return c_Value.mp_Recovery + c_Value._equip.mp_Recovery + c_Value._upgrade.mp_Recovery + c_Value._buff.mp_Recovery;
    }
    public float Get_stamina()
    {
        return c_Value.stamina + c_Value._equip.stamina + c_Value._upgrade.stamina + c_Value._buff.stamina;
    }
    public int Get_atk()
    {
        return c_Value.atk + c_Value._equip.atk + c_Value._upgrade.atk + c_Value._buff.atk;
    }
    public int Get_def()
    {
        return c_Value.def + c_Value._equip.def + c_Value._upgrade.def + c_Value._buff.def;
    }
    public int Get_avoid()
    {
        return c_Value.avoid + c_Value._equip.avoid + c_Value._upgrade.avoid + c_Value._buff.avoid;
    }
    public int Get_crit()
    {
        return c_Value.crit + c_Value._equip.crit + c_Value._upgrade.crit + c_Value._buff.crit;
    }
    public int Get_crit_atk()
    {
        return c_Value.crit_Atk + c_Value._equip.crit_Atk + c_Value._upgrade.crit_Atk + c_Value._buff.crit_Atk;
    }
    public float Get_attack_speed()
    {
        float speed = c_Value.attack_speed + c_Value._equip.attack_speed + c_Value._upgrade.attack_speed + c_Value._buff.attack_speed;
        return speed;
    }
    public float Get_cool_down()
    {
        return c_Value.cool_down + c_Value._equip.cool_down + c_Value._upgrade.cool_down + c_Value._buff.cool_down;
    }
    public int Get_atk_count()
    {
        return c_Value.atk_count + c_Value._equip.atk_count + c_Value._upgrade.atk_count + c_Value._buff.atk_count;
    }
    public float Get_atk_duration()
    {
        return c_Value.atk_duration + c_Value._equip.atk_duration + c_Value._upgrade.atk_duration + c_Value._buff.atk_duration;
    }
    public float Get_move_speed()
    {
        if (stop_moving)
            return 0f;
        else 
            return c_Value.move_speed + c_Value._equip.move_speed + c_Value._upgrade.move_speed + c_Value._buff.move_speed;
    }
    public float Get_attack_distance()
    {
        return c_Value.attack_distance + c_Value._equip.attack_distance + c_Value._upgrade.attack_distance + c_Value._buff.attack_distance;
    }
    public int Get_PickUp_distance()
    {
        return c_Value.pick_distance + c_Value._equip.pick_distance + c_Value._upgrade.pick_distance + c_Value._buff.pick_distance;
    }
    public float Get_Gold_extra()
    {
        return c_Value.gold_extra + c_Value._equip.gold_extra + c_Value._upgrade.gold_extra + c_Value._buff.gold_extra;
    }
    public float Get_luck()
    {
        return c_Value.luck + c_Value._equip.luck + c_Value._upgrade.luck + c_Value._buff.luck;
    }
    public float Get_ex_extra()
    {
        return c_Value.ex_extra + c_Value._equip.ex_extra + c_Value._upgrade.ex_extra + c_Value._buff.ex_extra;
    }

}
