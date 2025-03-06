using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class E_base : MonoBehaviour
{

    public biology_info e_value = new biology_info();
    public int HUDT_offset;
    public bool canFly;

    protected CharacterController C_ctr;
    protected Transform _target;
    protected Anim_Fox _anim;

    protected float move_speed = 1f;
    protected float min_move = 1f;
    protected float max_move = 5f;
    protected float gravityValue = -50f; // 重力加速度值
    protected Vector3 playerVelocity;
    protected bool isDie = false;
    protected bool isAttack = false;

    private Vector3 pushForce = Vector3.zero;
    private float pushDecay = 5f; // 推力衰减速度

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag(SaveKey.Character).transform;
        C_ctr = transform.GetComponent<CharacterController>();
        move_speed = Time_Range(e_value.move_speed, min_move, max_move);
        _anim = transform.GetComponent<Anim_Fox>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyPush(Vector3 force)
    {
        pushForce += force;
    }

    private void FixedUpdate()
    {
        if (isDie)
            return;
        if (pushForce.magnitude > 0.1f)
        {
            // 应用推力
            transform.position += pushForce * Time.deltaTime;

            // 推力衰减
            pushForce = Vector3.Lerp(pushForce, Vector3.zero, pushDecay * Time.deltaTime);
            return;
        }
        if (!isAttack)
            Attack_function();

        if (!canFly)
        {
            // 检查是否在地面上
            if (C_ctr.isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
            // 应用重力
            playerVelocity.y += gravityValue * Time.fixedDeltaTime;
            C_ctr.Move(playerVelocity * Time.fixedDeltaTime);
        }
    }

    float Time_Range(float time, float min, float max)
    {
        // 生成一个介于 0.1 到 1 之间的随机数
        float fluctuation = Random.Range(min, max);
        // 随机确定时间是增加还是减少
        bool increase = Random.value > 0.5f;
        // 根据随机值增加或减少时间
        if (increase)
        {
            time += fluctuation;
        }
        else
        {
            time -= fluctuation;
        }
        return time;
    }

    public virtual void Attack_function()
    {
        Vector3 posi = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        transform.LookAt(posi);

        float distance = Vector3.Distance(transform.position, _target.position);
        if ((distance <= e_value.attack_distance))
        {
            // 到达距离
            isAttack = true;
            _anim.change_anim(Anim_state.Attack1);
            StartCoroutine(delay_Active_attack(0.4f));
            //StartCoroutine(delay_change_anim(1f));
            StartCoroutine(delay_change_state(e_value.attack_speed));

        }
        else
        {
            Vector3 moveDirection = (_target.position - transform.position).normalized;
            Vector3 move = moveDirection * e_value.move_speed * Time.fixedDeltaTime;
            C_ctr.Move(move);
            _anim.change_anim(Anim_state.Run);
        }
    }

    // 受伤害函数
    public void Get_damage(Transform source , Player_skill_class skill)
    {
        if (isDie)
            return;
        C_attribute source_attr = source.GetComponent<C_attribute>();
        bool isCrit = false;

        int damage = Attack_calculate.calculate(source_attr, e_value, ref isCrit, skill);

        if (damage == -1)
        {
            // -1 表示闪避
            reduce_hp(damage, isCrit, true);
            return;
        }
        // 修改生命值
        reduce_hp(damage, isCrit, false);
    }

    // 减少血量
    public void reduce_hp(int value, bool crit, bool isAvoid)
    {

        GameObject Hud_obj = ResourceManager.Instance.GetResource<GameObject>("Prefab/UI/HUDObj");
        GameObject go = null;
        if (Hud_obj != null)
        {
            go = GameObject.Instantiate(Hud_obj, Hud_obj.transform.position, Quaternion.identity);
        }
        if (isAvoid)
        {
            Hud_obj.GetComponent<HUDText>().Init(transform, value, 4, HUDT_offset);
            return;
        }
        if (value == 0)
        {
            Hud_obj.GetComponent<HUDText>().Init(transform, value, 9, HUDT_offset);
            return;
        }
        int hp = e_value.hp - value;
        e_value.hp = hp;
        if (crit)
        {
            go.GetComponent<HUDText>().Init(transform, value, 5, HUDT_offset);
        }
        else
        {
            go.GetComponent<HUDText>().Init(transform, value, 2, HUDT_offset);
        }
        if (e_value.hp <= 0)
        {
            isDie = true;
            _anim.change_anim(Anim_state.Die);
            transform.tag = "Untagged";
            C_ctr.enabled = false;
            GameObject exp_obj = ResourceManager.Instance.GetResource<GameObject>("Particle/EXP/EXP_obj");
            GameObject obj = GameObject.Instantiate(exp_obj, transform.position, transform.localRotation , GameObject.Find("Exp_group").transform);
            obj.GetComponent<Exp_sc>().Set_exp(e_value.current_ex);
            StartCoroutine(delay_destroy(2f));
        }
    }




    protected virtual IEnumerator delay_Active_attack(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject FB = (GameObject)Resources.Load("Particle/bite/bite");
        Vector3 posi = transform.position;
        GameObject go = GameObject.Instantiate(FB, posi, transform.rotation);
        go.transform.position = go.transform.position + go.transform.forward * 2f;
        go.transform.position = go.transform.position + go.transform.up * 0.8f;
    }
    protected IEnumerator delay_change_state(float time)
    {
        yield return new WaitForSeconds(time);
        move_speed = Time_Range(e_value.move_speed, min_move, max_move);
        isAttack = false;
    }


    IEnumerator delay_destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(transform.gameObject);
    }

}
