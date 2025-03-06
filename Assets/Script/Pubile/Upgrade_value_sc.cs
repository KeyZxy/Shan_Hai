using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;




public class Upgrade_value_sc : MonoBehaviour
{
    public List<upgrade_info>infos = new List<upgrade_info>();
    public List<upgrade_info> Last_3_infos = new List<upgrade_info>();
    public Dictionary<int, List<upgrade_info>> Read_file = new Dictionary<int, List<upgrade_info>>();

    void Start()
    {
        File_read();
        Insert_value(int.MaxValue , 0);
        Change_value(210001, 1);
    }


    void File_read()
    {
        string path = Application.dataPath + "/Resources/skill_attr.txt";

        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            // 按换行符拆分每一行
            string[] lines = content.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            // 遍历每一行，按 ',' 分隔
            foreach (string line in lines)
            {
                string[] fields = line.Split(',');

                // 确保字段数量正确
                if (fields.Length >= 14) // 根据字段数量调整
                {
                    Player_skill_class type_skill = new Player_skill_class
                    {
                        Lv = int.Parse(fields[4]),
                        CD = float.Parse(fields[5]),
                        Timer = float.Parse(fields[6]),
                        cast_time = float.Parse(fields[7]),
                        duration = float.Parse(fields[8]),
                        count = int.Parse(fields[9]),
                        Atk = int.Parse(fields[10]),
                        crit = int.Parse(fields[11]),
                        crit_atk = int.Parse(fields[12]),
                        size = float.Parse(fields[13]),
                        move_speed = int.Parse(fields[14]),
                    };
                    upgrade_info temp = new upgrade_info
                    {
                        attr_ID = int.Parse(fields[0]),
                        attr_name = fields[1].Trim(),
                        Description = fields[2].Trim(),
                        type = skill_type.Passive_atk,      // 暂时写死
                        type_skill = type_skill,
                    };
                    switch(fields[3].Trim())
                    {
                        case "Passive_atk":
                            temp.type = skill_type.Passive_atk;
                            break;
                        case "Normal_atk":
                            temp.type = skill_type.Normal_atk;
                            break;
                    }

                    // 添加到列表
                    if (!Read_file.ContainsKey(temp.attr_ID))
                    {
                        Read_file[temp.attr_ID] = new List<upgrade_info>();
                    }
                    Read_file[temp.attr_ID].Add(temp);
                }
                else
                {
                    Debug.LogError("字段数量不足：" + line);
                }
            }
            //Debug.Log(Read_file.Count);

        }
        else
        {
            Debug.LogError("文件不存在: " + path);
        }
    }

    void Insert_value(int ID , int lv)
    {
        // 传入max后，会添加所有在字典里面的技能，等级为1
        if(ID == int.MaxValue)
        {
            // 加载所有
            foreach (var keyValuePair in Read_file)
            {
                for (int i = 0; i < keyValuePair.Value.Count; i++)
                {
                    upgrade_info upgradeInfo = keyValuePair.Value[i];
                    if (upgradeInfo.type_skill.Lv == lv + 1)
                    {
                        upgradeInfo.act = true; // 修改副本
                        keyValuePair.Value[i] = upgradeInfo; // 重新赋值回集合
                        infos.Add(upgradeInfo);
                    }
                }
            }
        }
    }

    public void Change_value(int ID , int lv)
    {
        if(lv == 9)
        {
            // 暂时写死为9级
            //Debug.Log("技能暂时最大级别为9");
            upgrade_info intomove = infos.Find(existing => existing.attr_ID == ID);
            infos.Remove(intomove);
            //Debug.Log($"Removed from infos: attr_ID={intomove.attr_ID}, Lv={intomove.type_skill.Lv}");
            return;
        }
        // 在 infos 中找到 ID 相同的结构并移除
        upgrade_info infoToRemove = infos.Find(existing => existing.attr_ID == ID);
        infos.Remove(infoToRemove);
        //Debug.Log($"Removed from infos: attr_ID={infoToRemove.attr_ID}, Lv={infoToRemove.type_skill.Lv}");

        // 在 Read_file 中找到 ID 相同且 Lv 为指定 lv+1 的结构
        if (Read_file.TryGetValue(ID, out List<upgrade_info> upgrades))
        {
            for (int i = 0; i < upgrades.Count; i++)
            {
                if (upgrades[i].type_skill.Lv == lv + 1)
                {
                    // 创建新实例，复制原来的数据并修改 act 属性
                    upgrade_info modifiedUpgrade = upgrades[i];
                    modifiedUpgrade.act = true;

                    // 替换列表中的旧实例
                    upgrades[i] = modifiedUpgrade;

                    // 添加到 infos 列表
                    infos.Add(modifiedUpgrade);

                    //Debug.Log($"Added to infos: attr_ID={upgrades[i].attr_ID}, Lv={upgrades[i].type_skill.Lv}");
                    return; // 找到一个后立即退出，避免重复添加
                }
            }

            // 如果没有找到满足条件的结构
            //Debug.LogWarning($"No matching info found in Read_file for attr_ID={ID} with Lv={lv + 1}");
        }
        else
        {
            //Debug.LogError($"No data found in Read_file for attr_ID={ID}");
        }
    }


}
