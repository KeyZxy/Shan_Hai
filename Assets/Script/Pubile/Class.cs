using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class biology_info
{
    public string ID;
    public string name;
    public int grade;
    public int level;
    public int current_ex;
    public int max_ex;

    public int max_hp;
    public int hp;
    public float mp_Recovery;
    public float stamina;

    public int atk;
    public int def;

    public int avoid;

    public int crit;
    public int crit_Atk;

    public float attack_speed;
    public float cool_down;
    public int atk_count;
    public float atk_duration;

    public float move_speed;
    public float fly_up_speed;
    public float attack_distance;
    public int pick_distance;
    public float gold_extra;
    public float luck;
    public float ex_extra;


    // 装备带来的属性
    public biology_info_extent _equip = new biology_info_extent();
    public biology_info_extent _upgrade = new biology_info_extent();
    public biology_info_extent _buff = new biology_info_extent();
}

public class biology_info_extent
{
    public int max_hp;
    public int hp;
    public float mp_Recovery;
    public float stamina;
    public int atk;
    public int def;
    public int avoid;
    public int crit;
    public int crit_Atk;
    public float attack_speed;
    public float cool_down;
    public int atk_count;
    public float atk_duration;
    public float move_speed;
    public float fly_up_speed;
    public float attack_distance;
    public int pick_distance;
    public float gold_extra;
    public float luck;
    public float ex_extra;

}

[System.Serializable]
public struct Player_skill_class
{
    public int ID;
    public string name;
    public skill_type type;
    public int Lv;
    public float CD;
    public float Timer;
    public float cast_time;
    public float duration;
    public int count;
    public int Atk;
    public int crit;
    public int crit_atk;
    public float size;
    public int move_speed;

}

public enum skill_type
{
    None,
    Normal_atk,
    Passive_atk,
    Active_atk,
    Ultimate_atk
}


[System.Serializable]
public struct upgrade_info
{
    public bool act;
    public int attr_ID;
    public string attr_name;
    public string Description;
    public skill_type type;
    public biology_info type_attr;
    public Player_skill_class type_skill;

}