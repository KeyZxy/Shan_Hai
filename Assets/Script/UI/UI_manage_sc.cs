using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_manage_sc : MonoBehaviour
{

    public GameObject _Attr_panel;

    // Start is called before the first frame update
    void Start()
    {
        _Attr_panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Key_Check();
    }

    void Key_Check()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if(!_Attr_panel.activeSelf)
                _Attr_panel.SetActive(true);
            else
                _Attr_panel.SetActive(false);
        }
    }

}
