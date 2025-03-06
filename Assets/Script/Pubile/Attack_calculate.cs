using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_calculate : MonoBehaviour
{
    public static int calculate(C_attribute attr, biology_info target, ref bool isCrit, Player_skill_class skill)
    {
        int damage = 0;
        int atk = attr.Get_atk() + skill.Atk;
        int def = target.def;
        int avoid = target.avoid;
        int crit = attr.Get_crit() + skill.crit;
        float crit_atk = (attr.Get_crit_atk() * 0.01f) + (skill.crit_atk * 0.01f);

        // 先判断是否闪避
        int randomAvoid = Random.Range(1, 101);
        if (avoid >= randomAvoid)
            return -1;

        // 判断是否暴击
        int randomValue = Random.Range(1, 101);
        isCrit = crit >= randomValue;

        if (isCrit)
        {
            float temp_atk = atk;
            temp_atk *= crit_atk;
            atk = (int)temp_atk;
        }
        damage = atk - def;
        if (damage <= 0)
            damage = 1;

        return damage;
    }



}
