using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim_Fox : MonoBehaviour
{

    private Animator anim;
    private Anim_state Current_anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = transform.Find("Body").transform.GetComponent<Animator>();
        Current_anim = Anim_state.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 检查攻击动画是否在播放
        if (stateInfo.IsName("Atk") && stateInfo.normalizedTime >= 1.0f)
        {
            change_anim(Anim_state.Idle);
        }
        if (stateInfo.IsName("Die") && stateInfo.normalizedTime >= 1.0f)
        {
            anim.ResetTrigger("Die");
        }
    }

    public void change_anim(Anim_state state)
    {
        if (state == Current_anim)
            return;
        switch (state)
        {
            case Anim_state.Idle:
                anim.SetInteger("Anim_state", 0);
                Current_anim = Anim_state.Idle;
                break;
            case Anim_state.Run:
                anim.SetInteger("Anim_state", 1);
                Current_anim = Anim_state.Run;
                break;
            case Anim_state.Attack1:
                anim.SetInteger("Anim_state", 2);
                Current_anim = Anim_state.Attack1;
                break;
            case Anim_state.Die:
                //anim.SetInteger("Anim_state", 10);
                anim.SetTrigger("Die");
                Current_anim = Anim_state.Die;
                break;
            case Anim_state.Idle2:
                anim.SetInteger("Anim_state", 11);
                Current_anim = Anim_state.Idle2;
                break;

        }
    }


}
