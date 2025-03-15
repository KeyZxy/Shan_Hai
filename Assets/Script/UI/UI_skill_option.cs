using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class UI_skill_option : MonoBehaviour
{
    public upgrade_info _upgrade_info;

    private Upgrade_value_sc _upgrade;
    private Image _image;
    private Up_grade_panel_sc _upgrade_panel;
    private C_attribute _attr;
    private C_upgrade_attr _upgrade_attr;



    // Start is called before the first frame update
    void Start()
    {
        _upgrade_panel = transform.parent.GetComponent<Up_grade_panel_sc>();
        _attr = GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_attribute>();
     
        Button _button = GetComponent<Button>();
        if (_button != null)
        {
            // 绑定点击事件
            _button.onClick.AddListener(BTN_UI_Click);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if(_upgrade == null)
            _upgrade = GameObject.Find("Upgrade_value").GetComponent<Upgrade_value_sc>();
        if (_image == null)
            _image = transform.GetComponent<Image>();

        Get_Card();
        Upgrade_processing();
    }

    void Upgrade_processing()
    {
        switch(_upgrade_info.type)
        {
            case skill_type.None:
                Attr_processing();
                break;
            case skill_type.Normal_atk:
                Attr_processing();
                break;
            case skill_type.Passive_atk:
                Attr_processing();
                break;
            case skill_type.Active_atk:

                break;
            case skill_type.Ultimate_atk:

                break;
        }
    }

    void Attr_processing()
    {
        string path = $"UI/Upgrade1/{_upgrade_info.attr_ID}";
        Sprite newSprite = Resources.Load<Sprite>(path);
        if (newSprite != null)
        {
            _image.sprite = newSprite; // 替换图片
        }
    }

    void Get_Card()
    {
        if(_upgrade_attr == null)
            _upgrade_attr = GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_upgrade_attr>();

        if (transform.name == "option_1")
        {
            _upgrade_attr.Random_Info();
            _upgrade_info = _upgrade_attr.Get_info(0);

        }
        if (transform.name == "option_2")
        {
            _upgrade_info = _upgrade_attr.Get_info(1);

        }
        if (transform.name == "option_3")
        {
            _upgrade_info = _upgrade_attr.Get_info(2);

        }
        // 重复判断，保证不重复
        //if (transform.name == "option_1")
        //{
        //    int randomIndex = Random.Range(0, _upgrade.infos.Count);
        //    _upgrade_info = _upgrade.infos[randomIndex];
        //}
        //else
        //{
        //    bool isUnique = false;
        //    do
        //    {
        //        if (transform.name == "option_2")
        //        {
        //            int randomIndex = Random.Range(0, _upgrade.infos.Count);
        //            _upgrade_info = _upgrade.infos[randomIndex];
        //            int option1Info_ID = GameObject.Find("option_1").GetComponent<UI_skill_option>()._upgrade_info.attr_ID;
        //            isUnique = _upgrade_info.attr_ID != option1Info_ID;
        //        }
        //        else if (transform.name == "option_3")
        //        {
        //            int option1Info_ID = GameObject.Find("option_1").GetComponent<UI_skill_option>()._upgrade_info.attr_ID;
        //            int option2Info_ID = GameObject.Find("option_2").GetComponent<UI_skill_option>()._upgrade_info.attr_ID;
        //            int randomIndex = Random.Range(0, _upgrade.infos.Count);
        //            _upgrade_info = _upgrade.infos[randomIndex];
        //            isUnique = _upgrade_info.attr_ID != option1Info_ID && _upgrade_info.attr_ID != option2Info_ID;
        //        }
        //        else
        //        {
        //            isUnique = true;
        //        }
        //    } while (!isUnique);

        //}
    }

    public void BTN_UI_Click()
    {
        _upgrade_panel.Hide_UI();
        _attr.C_Upgrade_Fun(_upgrade_info);
    }


}
