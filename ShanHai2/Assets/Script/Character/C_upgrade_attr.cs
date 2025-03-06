using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class C_upgrade_attr : MonoBehaviour
{
    public List<upgrade_info> infos = new List<upgrade_info>();
    public List<upgrade_info> passive_infos = new List<upgrade_info>();
    private List<upgrade_info> Temp_infos = new List<upgrade_info>();

    private Upgrade_value_sc _upgrade;
    private C_base _base;
    private int attr_count;
    private int attr_max_count = 6;
    private int passive_count;
    private int passive_max_count = 3;
    private int skill_lv = 9;
    // private int active_count;

    void Start()
    {
        _upgrade = GameObject.Find("Upgrade_value").GetComponent<Upgrade_value_sc>();
        _base = transform.GetComponent<C_base>();
        //List < upgrade_info >ins = Random_Info();
        //for (int i = 0; i < ins.Count; i++)
        //{
        ////    Debug.Log(ins[i].attr_ID);
        //}
    }



    public void Add_attr(upgrade_info info)
    {
        upgrade_info existingInfo = infos.Find(existing => existing.attr_ID == info.attr_ID);

        if(info.type == skill_type.None)
        {
            if (EqualityComparer<upgrade_info>.Default.Equals(existingInfo, default(upgrade_info)))
            {
                // 如果不存在，level 设置为 1
                info.type_attr.level = 1;
                infos.Add(info);
                attr_count++;
                //    Debug.Log($"新属性添加: {info.attr_ID}, 等级: {info.type_attr.level}");
                //    Debug.Log(attr_count);
            }
            else
            {
                // 如果存在，等级加 1
                int index = infos.IndexOf(existingInfo);
                existingInfo.type_attr.level += 1;
                infos[index] = existingInfo; // 更新列表中的值

                //    Debug.Log($"属性已存在，等级+1: {info.attr_ID}, 当前等级: {existingInfo.type_attr.level}");
            }
        }
        else if(info.type == skill_type.Passive_atk)
        {
            upgrade_info pass_info = passive_infos.Find(existing => existing.attr_ID == info.attr_ID);
            //Debug.Log(info.type_skill.Lv);
            if (EqualityComparer<upgrade_info>.Default.Equals(pass_info, default(upgrade_info)))
            {
                // 新的被动技能，添加到库里面，然后count +1
                passive_infos.Add(info);
                passive_count++;
                // 触发函数，让下次随机的技能等级增加
                _upgrade.Change_value(info.attr_ID, info.type_skill.Lv);

            }
            else
            {
                // 如果找到匹配项，从 passive_infos 中移除对应项
                upgrade_info itemToRemove = passive_infos.Find(x => x.attr_ID == info.attr_ID);
                passive_infos.Remove(itemToRemove);
                passive_infos.Add(info);
                _upgrade.Change_value(info.attr_ID, info.type_skill.Lv);
            }
        }
        else if(info.type == skill_type.Normal_atk)
        {

            Player_skill_class skill = _base.skill_class[0];
            if(skill.type == skill_type.Normal_atk)
            {
                skill = info.type_skill;
                skill.ID = info.attr_ID;
                skill.name = info.attr_name;
                skill.type = info.type;
                _base.skill_class[0] = skill;
                _upgrade.Change_value(info.attr_ID, info.type_skill.Lv);
            }
            else
            {
                Debug.LogWarning("普通攻击不相同!!!!");
            }

            
        }
    }


    public void Random_Info()
    {
        List<upgrade_info> source_Infos = new List<upgrade_info>();
        bool attr_max = attr_count >= attr_max_count;
        bool skill_max = passive_count >= passive_max_count;
        bool attr_max_level = true;
        bool skill_max_level = true;
        foreach (upgrade_info info in infos)
        {
            if (info.type_attr.level < info.type_attr.grade)
            {
                attr_max_level = false;
                break;
            }
        }
        foreach (upgrade_info passive in passive_infos)
        {
            if (passive.type_skill.Lv <= skill_lv)
            {
                skill_max_level = false;
                break;
            }
        }

        if(attr_max && skill_max)
        {
            if(attr_max_level && skill_max_level)
            {
                source_Infos = _upgrade.Last_3_infos; // 使用最后3个技能
            }else
            {
                // 遍历 passive_infos，找到 _upgrade.infos 中对应的条目并添加到 source_Infos
                foreach (upgrade_info passive in passive_infos)
                {
                    foreach (upgrade_info upgrade in _upgrade.infos)
                    {
                        if (upgrade.attr_ID == passive.attr_ID)
                        {
                            source_Infos.Add(upgrade);
                            break; // 找到后跳出内层循环，避免重复比较
                        }
                    }
                }

                // 将 infos 列表直接添加到 source_Infos
                source_Infos.AddRange(infos);
            }
        }else
        {
            source_Infos = _upgrade.infos;
        }

        // 暂时只有一个普通攻击，暂时写死
        if (_base.skill_class[0].Lv < skill_lv)
        {
            // 检查 source_Infos 是否已存在 attr_ID == 210001
            bool alreadyExists = source_Infos.Exists(info => info.attr_ID == 210001);

            if (!alreadyExists) // 只有当它不存在时才添加
            {
                foreach (upgrade_info info in _upgrade.infos)
                {
                    if (info.attr_ID == 210001)
                    {
                        source_Infos.Add(info);
                        break;
                    }
                }
            }
        }


        // 随机打乱数据源列表
        List<upgrade_info> shuffled_Infos = new List<upgrade_info>(source_Infos);
        shuffled_Infos = shuffled_Infos.OrderBy(info => Random.value).ToList();

        // 筛选不同的 attr_ID 和符合 act 的数据
        HashSet<int> unique_IDs = new HashSet<int>();
        List<upgrade_info> result = new List<upgrade_info>();

        foreach (upgrade_info info in shuffled_Infos)
        {
            //    Debug.Log(info.attr_ID);
            if (!info.act || unique_IDs.Contains(info.attr_ID))
                continue;

            // 条件 1: 如果 attr_max 为真，attr_max_level 为真，且 info.type 为 None，则跳过
            if (attr_max && attr_max_level && info.type == skill_type.None)
            {
                //Debug.Log("检查点1");
                continue;
            }

            // 条件 2: 如果 attr_max 为真，attr_max_level 为假，则不出现新的 type 为 None 的 ID
            if (attr_max && !attr_max_level &&
                info.type == skill_type.None &&
                !infos.Exists(existingInfo => existingInfo.attr_ID == info.attr_ID))
            {
                //Debug.Log("检查点2");
                continue;
            }

            // 条件 3: 如果 skill_max 为真，skill_max_level 为真，且 info.type == skill_type.Passive_atk，则跳过
            if (skill_max && skill_max_level && info.type == skill_type.Passive_atk)
            {
                //Debug.Log("检查点3");
                continue;
            }

            // 条件 4: 如果 skill_max 为真，skill_max_level 为假，则不出现新的 type 为 skill_type.Passive_atk 的 ID
            if (skill_max && !skill_max_level &&
                info.type == skill_type.Passive_atk &&
                !passive_infos.Exists(existingInfo => existingInfo.attr_ID == info.attr_ID))
            {

                //Debug.Log("检查点4");
                continue;
            }

            // 判断是否已有数据满足 level >= grade 的条件
            bool skip = infos.Exists(existingInfo =>
                existingInfo.attr_ID == info.attr_ID &&
                existingInfo.type_attr.level >= info.type_attr.grade);
            if (skip)
                continue;

            result.Add(info);
            unique_IDs.Add(info.attr_ID);

            if (result.Count == 3)
                break;
        }

        // 检查是否有足够数量
        if (result.Count < 3)
        {
            if (attr_max )
            {
                // 如果 max_count 为 true，从 _upgrade.Last_3_infos 中随机选取数据，直到 result 中有 3 个元素
                List<upgrade_info> additionalInfos = _upgrade.Last_3_infos.OrderBy(info => Random.value).ToList();

                foreach (upgrade_info info in additionalInfos)
                {
                    if (result.Count >= 3)
                        break;

                    // 如果 attr_ID 已经存在，跳过
                    if (unique_IDs.Contains(info.attr_ID))
                        continue;

                    // 判断是否已有数据满足 level >= grade 的条件
                    bool skip = infos.Exists(existingInfo =>
                        existingInfo.attr_ID == info.attr_ID &&
                        existingInfo.type_attr.level >= info.type_attr.grade);
                    if (skip)
                        continue;

                    result.Add(info);
                    unique_IDs.Add(info.attr_ID);
                }

                // 如果最后补充的数据不足 3 个，输出警告
                if (result.Count < 3)
                {
                    Debug.LogWarning("从 _upgrade.Last_3_infos 补充数据时，数据数量不足 3 个！");
                }
            }
            else
            {
                Debug.LogError("未找到足够的不同 attr_ID 且 act 为 true 的 upgrade_info！");
            }
        }

        // 更新临时数据
        Temp_infos.Clear();
        Temp_infos = result;

    }

    public upgrade_info Get_info_by_ID(int id)
    {
        upgrade_info foundInfo = passive_infos.Find(info => info.attr_ID == id);
        return foundInfo;
    }

    public upgrade_info Get_info(int index)
    {
        return Temp_infos[index];
    }
}



