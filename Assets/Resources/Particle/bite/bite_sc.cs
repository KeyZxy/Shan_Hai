using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class bite_sc : MonoBehaviour
{

    private biology_info source_info;
    Player_skill_class source_skill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init(biology_info s , Player_skill_class sk)
    {
        source_info = s; 
        source_skill = sk;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(SaveKey.Character))
        {
            other.GetComponent<C_base>().Get_damage(source_info , source_skill);
        }
    }


}
