using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum Anim_state
{
    Idle,
    Idle2,
    Walk,
    Walk_back,
    Run,
    Dizz,
    Jump,
    Sprint,
    Die,
    Attack1,
    Attack2,
    Hit,
    Ready,
}

public class C_anim : MonoBehaviour
{


    private Animator anim;
    private Anim_state Current_anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = transform.Find("body").transform.GetComponent<Animator>();
        Current_anim = Anim_state.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            case Anim_state.Sprint:
                anim.SetInteger("Anim_state", 2);
                Current_anim = Anim_state.Sprint;
                break;
            case Anim_state.Attack1:
                anim.SetInteger("Anim_state", 3);
                Current_anim = Anim_state.Attack1;
                break;
            case Anim_state.Ready:
                anim.SetInteger("Anim_state", 5);
                Current_anim = Anim_state.Ready;
                break;

        }
    }


}
