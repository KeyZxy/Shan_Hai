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
    public bool Free_move;
    public float Atk_distance = 15;
    public List<GameObject> _areas = new List<GameObject>();

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
    protected bool Out_area = false;
    protected Vector3 original_posi;
    protected bool In_atk_are = false;
    protected bool walking = false;
    protected float walk_time;
    protected float walk_timer;
    protected Vector3 walk_destination;
    protected bool target_die = false;

    protected Vector3 pushForce = Vector3.zero;
    protected float pushDecay = 5f; // 推力衰减速度

    void OnEnable()
    {
        // 订阅主角死亡事件
        C_base.OnPlayerDeath += OnPlayerDeath;
    }

    void OnDisable()
    {
        // 取消订阅，避免内存泄漏
        C_base.OnPlayerDeath -= OnPlayerDeath;
    }

    // 接受广播
    private void OnPlayerDeath()
    {
        //Debug.Log("收到玩家死亡广播！执行游戏结束逻辑！");
        target_die = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag(SaveKey.Character).transform;
        C_ctr = transform.GetComponent<CharacterController>();
        move_speed = Time_Range(e_value.move_speed, min_move, max_move);
        _anim = transform.GetComponent<Anim_Fox>();
        original_posi = transform.position;
        e_value.hp = e_value.max_hp;
        Random_walk_time();
        StartCoroutine(CheckDistanceRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttack || isDie || In_atk_are || Out_area || walking)
        {
            Random_walk_time();
            walk_timer = 0;
            return;
        }
        walk_timer += Time.deltaTime;
        if (walk_timer > walk_time)
        {
            walking = true;
            walk_destination = GetRandomPosi(original_posi, 5, 5);
        }
    }

    public void ApplyPush(Vector3 force)
    {
        pushForce += force;
    }

    private void FixedUpdate()
    {
        if (isDie || target_die)
            return;
        if (pushForce.magnitude > 0.1f)
        {
            // 应用推力
            transform.position += pushForce * Time.deltaTime;

            // 推力衰减
            pushForce = Vector3.Lerp(pushForce, Vector3.zero, pushDecay * Time.deltaTime);
            return;
        }
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
        if (Out_area)
        {
            Go_back();
            return;
        }
        if (Free_move)
        {
            if (In_atk_are)
            {
                walking = false;
                walk_timer = 0;
                if (Out_of_area())
                {
                    Out_area = true;
                    In_atk_are = false;
                    return;
                }
                if (!isAttack)
                    Attack_function();
            }
            else
            {
                if (walking)
                {
                    Free_walking();
                }
            }
        }
        else
        {
            if (!isAttack)
                Attack_function();
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

    private void Free_walking()
    {
        Vector3 posi = new Vector3(walk_destination.x, transform.position.y, walk_destination.z);
        transform.LookAt(posi);

        float distance = Vector3.Distance(transform.position, walk_destination);
        if ((distance <= 1))
        {
            // 到达距离
            walking = false;
            walk_timer = 0;
            Random_walk_time();
            _anim.change_anim(Anim_state.Idle);
        }
        else
        {
            Vector3 moveDirection = (walk_destination - transform.position).normalized;
            Vector3 move = moveDirection * e_value.move_speed * Time.fixedDeltaTime;
            C_ctr.Move(move);
            _anim.change_anim(Anim_state.Run);
        }
    }
    private void Go_back()
    {
        Vector3 posi = new Vector3(original_posi.x, transform.position.y, original_posi.z);
        transform.LookAt(posi);

        float distance = Vector3.Distance(transform.position, original_posi);
        if ((distance <= 1))
        {
            // 到达距离
            Out_area = false;
            e_value.hp = e_value.max_hp;
            _anim.change_anim(Anim_state.Idle);
        }
        else
        {
            Vector3 moveDirection = (original_posi - transform.position).normalized;
            Vector3 move = moveDirection * e_value.move_speed * Time.fixedDeltaTime * 2f;
            C_ctr.Move(move);
            _anim.change_anim(Anim_state.Run);
        }
    }

    // 受伤害函数
    public void Get_damage(Transform source, Player_skill_class skill)
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
            GameObject obj = GameObject.Instantiate(exp_obj, transform.position, transform.localRotation, GameObject.Find("Exp_group").transform);
            obj.GetComponent<Exp_sc>().Set_exp(e_value.current_ex);
            StartCoroutine(delay_destroy(2f));
        }
    }

    bool Out_of_area()
    {
        bool isOutsideAll = true;

        foreach (GameObject area in _areas)
        {
            E_area_sc areaScript = area.GetComponent<E_area_sc>();
            if (areaScript != null && areaScript.IsInsideCollider(gameObject))
            {
                isOutsideAll = false;
                break;
            }
        }

        if (isOutsideAll)
        {
            isOutsideAll = true;
        }
        return isOutsideAll;
    }

    Vector3 GetRandomPosi(Vector3 originalPos, float minRange, float maxRange)
    {
        // 获取原始位置的 X 和 Z 坐标，忽略 Y 轴
        float randomX = Random.Range(originalPos.x - minRange, originalPos.x + maxRange);
        float randomZ = Random.Range(originalPos.z - minRange, originalPos.z + maxRange);

        // 生成新的随机位置，Y 坐标保持为原始位置的 Y 坐标
        return new Vector3(randomX, originalPos.y, randomZ);
    }

    void Random_walk_time()
    {
        walk_time = Random.Range(3f, 8f);
    }

    protected virtual IEnumerator delay_Active_attack(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject FB = (GameObject)Resources.Load("Particle/bite/bite");
        Vector3 posi = transform.position;
        GameObject go = GameObject.Instantiate(FB, posi, transform.rotation);
        go.transform.position = go.transform.position + go.transform.forward * 2f;
        go.transform.position = go.transform.position + go.transform.up * 0.8f;
        Player_skill_class ss = new Player_skill_class();
        go.GetComponent<bite_sc>().Init(e_value , ss);
    }
    protected IEnumerator delay_change_state(float time)
    {
        yield return new WaitForSeconds(time);
        move_speed = Time_Range(e_value.move_speed, min_move, max_move);
        isAttack = false;
    }

    IEnumerator CheckDistanceRoutine()
    {
        while (true)
        {
            if (!Out_area)
            {
                float dis = Vector3.Distance(_target.position, transform.position);
                if (dis < Atk_distance)
                {
                    In_atk_are = true;
                }
            }
            yield return new WaitForSeconds(0.2f); // 每 0.2 秒检测一次
        }
    }

    IEnumerator delay_destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(transform.gameObject);
    }

}
