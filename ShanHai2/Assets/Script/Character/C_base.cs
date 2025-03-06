using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class C_base : MonoBehaviour
{
    public LayerMask enemyLayer; // 敌人层
    public List<Player_skill_class> skill_class = new List<Player_skill_class>();
    //public bool canFly;

    private CharacterController _ctr;
    private Transform _cam;
    private C_attribute _attr;
    private Transform _target;
    private C_anim _anim;
    private C_Pick_up_sc _pickup;
    private Target_lock_sc _Lock_Sc;

    private bool isStop = false;
    private bool sprint = false;
    private float move_x;
    private float move_y;
    private bool Key_move_click = false;
    private Vector3 playerVelocity;
    private float gravityValue = -50f;
    //private bool stamina_enpty = false;
    //private bool isFlying = false;
    private bool Free_view = true;
    private float maxDistanceFromCenter = 200.0f; // 最大允许的距离，超出此距离的敌人不检测.UI上的距离
    private bool Target_lock = false;
    private bool CD_atk = false;
    private bool Attacking = false;
    private bool isPaused = false;
    private GameObject Temp_jiguangbo_obj;
    private GameObject Temp_huongjue_obj;
    private GameObject Temp_miaozhun_obj;
    private bool jiguangbo_ready = false;
    private bool huongJue_ready = false;
    private GameObject Temp_shuihuan_obj;
    private float skill_move_speed = 2f;
    private bool skill_1_cd = false;
    private bool skill_2_cd = false;



    // Start is called before the first frame update
    void Start()
    {
        // 隐藏鼠标指针
        Cursor.visible = false;
        // 锁定鼠标指针到游戏窗口中央
        Cursor.lockState = CursorLockMode.Locked;
        // 锁定分辨率
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);

        _anim = transform.GetComponent<C_anim>();
        _ctr = transform.GetComponent<CharacterController>();
        _cam = GameObject.Find("Main Camera").transform;
        _attr = transform.gameObject.GetComponent<C_attribute>();
        _pickup = transform.GetComponent<C_Pick_up_sc>();
        _Lock_Sc = GameObject.Find("target_lock").GetComponent<Target_lock_sc>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPaused) 
            return;
        Key_Check();
        if (!isStop && !sprint)
        {
            Key_Control_Move();
        }
        Find_enemy();
    }

    void FixedUpdate()
    {
        Gravity();
    }

    // 找到距离摄像机中心最近的敌人
    Transform GetClosestEnemyToCenter(Collider[] enemies)
    {
        Transform closestEnemy = null;
        float minDistanceToCenter = Mathf.Infinity;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        foreach (var enemy in enemies)
        {
            // 将敌人的世界坐标转换为屏幕坐标
            Vector3 enemyScreenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);

            // 检查敌人是否在摄像机的视锥体内，并且没有超出最大允许距离
            if (enemyScreenPos.z > 0)
            {
                // 计算敌人与屏幕中心的距离
                float distanceToCenter = Vector2.Distance(new Vector2(enemyScreenPos.x, enemyScreenPos.y), new Vector2(screenCenter.x, screenCenter.y));

                // 检查敌人是否在允许的距离范围内
                if (distanceToCenter <= maxDistanceFromCenter)
                {
                    // 如果敌人距离更近，则更新最接近的敌人
                    if (distanceToCenter < minDistanceToCenter)
                    {
                        minDistanceToCenter = distanceToCenter;
                        closestEnemy = enemy.transform;
                    }
                }
            }
        }

        return closestEnemy;
    }

    // 移动函数
    void Key_Control_Move()
    {
        if (Free_view)
        {
            Free_view_move();

        }
        else
        {
            //if(!Target_lock )
            //    Ray_select_target();
            Fixed_view_move();
        }
        
    }


    // 固定视角，让角色跟随摄像机转动
    void Fixed_view_move()
    {
        // 让角色跟随摄像机的旋转
        // 固定视角，让角色跟随摄像机转动
        float y = _cam.rotation.eulerAngles.y;  // 获取摄像机的Y轴旋转角度
        float rotationOffset = 8f;  // 偏移角度
        transform.rotation = Quaternion.Euler(0, y + rotationOffset, 0);  // 设置角色的Y轴旋转与摄像机一致，并增加偏移

        if (Key_move_click)
        {
            float h = move_x; // 获取水平输入
            float v = move_y; // 获取垂直输入

            if (h != 0 || v != 0)
            {
                // 创建目标方向向量
                Vector3 moveDirection = new Vector3(h, 0, v).normalized;

                // 使用角色当前的前向方向移动
                Vector3 moveVector = transform.forward * moveDirection.z + transform.right * moveDirection.x;

                // 移动角色
                if (Temp_jiguangbo_obj)
                {
                    _ctr.Move(moveVector * _attr.Get_move_speed() * Time.deltaTime);
                }else
                {
                    _ctr.Move(moveVector * skill_move_speed * Time.deltaTime);
                }
                
                
                // 切换动画状态为运行
                _anim.change_anim(Anim_state.Walk);
            }
        }
        else
        {
            // 如果没有移动输入，切换动画状态为闲置
            if (!Anim_lock())
                _anim.change_anim(Anim_state.Idle);
        }
    }

    // 自由视角移动
    void Free_view_move()
    {
        //设置角色的面向位置
        // 自由视角,角色跟摄像机自由
        if (Key_move_click)
        {
            float h = move_x;
            float v = move_y;
            if (h != 0 || v != 0)
            {
                Vector3 targetDirection = new Vector3(h, 0, v);
                float y = _cam.rotation.eulerAngles.y;
                targetDirection = Quaternion.Euler(0, y, 0) * targetDirection;
                transform.rotation = Quaternion.LookRotation(targetDirection);
                if(!Anim_lock())
                    _anim.change_anim(Anim_state.Run);
                _ctr.Move(transform.forward * _attr.Get_move_speed() * Time.deltaTime);
            }
        }
        else
        {
            if (!Attacking)
                _anim.change_anim(Anim_state.Idle);
        }
    }

    // 重力函数
    void Gravity()
    {
        // 手动地面检测
        bool isGrounded = _ctr.isGrounded;
        if (!isGrounded)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.83f))
            {
                isGrounded = true;
            }
        }
        // 如果角色在地面上且未跳跃，重置Y轴速度为0，防止重力累积
        if (isGrounded)
        {
            if (playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }
        }
        // 如果角色不在地面上，继续累加重力
        playerVelocity.y += gravityValue * Time.fixedDeltaTime;
        _ctr.Move(playerVelocity * Time.fixedDeltaTime);
    }


    void Normal_atk()
    {
        // 如果已经在攻击冷却中，直接返回
        if (CD_atk)
            return;
        if (_target == null)
            return;
        Attacking = true;
        Fu_start();
        for (int i = 0; i < skill_class[0].count + _attr.Get_atk_count(); i++)
        {
            Fu(_target);
        }


        // 进入冷却状态
        CD_atk = true;
        float cd_time = skill_class[0].CD - _attr.Get_attack_speed();
        StartCoroutine(delay_change_CD(cd_time , 0));
        StartCoroutine(delay_change_CD(0.35f, 3));

    }


    void Fu_start()
    {
        _anim.change_anim(Anim_state.Attack1);
        // 生成开始特效
        Vector3 posi = transform.position + transform.forward * 1f + transform.up * 1f;
        GameObject fbStart = ResourceManager.Instance.GetResource<GameObject>("Particle/Fu/start");
        if (fbStart != null)
        {
            GameObject.Instantiate(fbStart, posi, transform.localRotation);
        }
    }
    void Fu(Transform target)
    {

        // 生成攻击物体并初始化目标
        Vector3 posi = transform.position + transform.forward * 1f + transform.up * 1f;
        GameObject fu = ResourceManager.Instance.GetResource<GameObject>("Particle/Fu/Fu");
        if (fu != null)
        {
            GameObject go = GameObject.Instantiate(fu, posi, transform.localRotation);
            go.GetComponent<Fu_sc>().Init(target.gameObject, transform , skill_class[0]); // 将目标传递给攻击物体
        }
    }

    void jiguangbo_start()
    {
        if (jiguangbo_ready)
            return;
        _anim.change_anim(Anim_state.Ready);
        Free_view = false;
        jiguangbo_ready = true;
        _cam.GetComponent<Camera_move>().Act_second_view(true);
    }
    void huongjue_start()
    {
        if (huongJue_ready)
            return;
        _anim.change_anim(Anim_state.Ready);
        huongJue_ready = true;
        Free_view = false;
        _cam.GetComponent<Camera_move>().Act_second_view(true);
        GameObject miaozhun_FB = ResourceManager.Instance.GetResource<GameObject>("Particle/mianzhun2/miaozhunqiu_M");
        Vector3 posi = transform.position + transform.forward * 10f;
        Temp_miaozhun_obj = GameObject.Instantiate(miaozhun_FB, posi, miaozhun_FB.transform.rotation);
        Temp_miaozhun_obj.GetComponent<miaozhun_qiu_sc>().Init(transform , skill_class[2]);
    }
    void jiguangbo()
    {
        //    Free_view = false;
        _attr.Stop_move(true);
        skill_1_cd = true;
        GameObject jiguangbo = ResourceManager.Instance.GetResource<GameObject>("Particle/jiguangbo/jiguangb");
        Vector3 posi = transform.position + transform.forward * 0.5f + transform.up * 1f;
        Quaternion adjustedRotation = transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
        Temp_jiguangbo_obj = GameObject.Instantiate(jiguangbo, posi, adjustedRotation);
        Temp_jiguangbo_obj.transform.parent = transform;

        Player_skill_class s = skill_class[1];
        Temp_jiguangbo_obj.GetComponent<jiguangbo_sc>().Init(transform, s);
        StartCoroutine(DelayFunctionTrigger(s.duration, jiguangbo_end));
        float cd_time = skill_class[1].CD - _attr.Get_cool_down();
        StartCoroutine(delay_change_CD(cd_time , 1));
    }
    void huongjue()
    {
        skill_2_cd = true;
        Vector3 posi = Temp_miaozhun_obj.transform.position;
        GameObject FB = ResourceManager.Instance.GetResource<GameObject>("Particle/huongjue/huongjue");
        Temp_huongjue_obj = GameObject.Instantiate(FB, posi, FB.transform.rotation);
        Temp_huongjue_obj.GetComponent<huongjue_sc>().Init(transform, skill_class[2]);
        Destroy(Temp_miaozhun_obj);
        Temp_miaozhun_obj = null;
        float cd_time = skill_class[2].CD - _attr.Get_cool_down();
        StartCoroutine(delay_change_CD(cd_time, 2));
    }
    void huongjue_end()
    {
        _cam.GetComponent<Camera_move>().Act_second_view(false);
        huongJue_ready = false;
        Free_view = true;
    }
    void jiguangbo_end()
    {
        GameObject jiesu = ResourceManager.Instance.GetResource<GameObject>("Particle/jiguangbo/jieshu_M");
        GameObject.Instantiate(jiesu, Temp_jiguangbo_obj.transform.position, Temp_jiguangbo_obj.transform.rotation, transform);
        if (Temp_jiguangbo_obj != null)
            Destroy(Temp_jiguangbo_obj);
        StartCoroutine(DelayFunctionTrigger(0.3f, jiguangbo_destory));
        _cam.GetComponent<Camera_move>().Act_second_view(false);
        jiguangbo_ready = false;
    }
    void jiguangbo_destory()
    {
        _attr.Stop_move(false);
        Free_view = true;
    }


    void Find_enemy()
    {
        // 获取准心的位置
        Vector3 crosshairPosition = new Vector3(Screen.width / 2, Screen.height / 2);

        // 获取攻击距离
        float attackDistance = _attr.Get_attack_distance();
        float coneAngle = 20f; // 检测圆锥的角度（度）

        // 检测范围内的所有碰撞体
        Collider[] colliders = Physics.OverlapSphere(Camera.main.transform.position, attackDistance);

        List<Transform> potentialTargets = new List<Transform>();

        // 收集所有符合条件的敌人
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(SaveKey.Enemy))
            {
                Vector3 toCollider = collider.transform.position - Camera.main.transform.position;

                // 判断角度是否在圆锥范围内
                if (Vector3.Angle(Camera.main.transform.forward, toCollider.normalized) <= coneAngle)
                {
                    potentialTargets.Add(collider.transform);
                }
            }
        }

        // 如果找到多个敌人，选择离准心最近的
        if (potentialTargets.Count > 0)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (Transform target in potentialTargets)
            {
                // 获取目标的屏幕空间坐标
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);

                // 计算目标与准心的距离
                float distanceToCrosshair = Vector3.Distance(crosshairPosition, screenPos);

                // 如果当前敌人更接近准心，则更新
                if (distanceToCrosshair < closestDistance)
                {
                    closestDistance = distanceToCrosshair;
                    closestEnemy = target;
                }
            }

            Set_target(closestEnemy);
        }
        else
        {
            Set_target(null);
        }
    }


    void Set_target(Transform tr)
    {
        if(tr == _target)
            return;
        if(tr == null)
        {
            _target = null;
            _Lock_Sc.Remove_target();
        }
        else if (tr != null)
        {
            _target = tr;
            _Lock_Sc.Set_target(_target);
        }
            

    }

    IEnumerator DashForward()
    {
        //int cost = 10;
        //if (_attr.c_Value.stamina < cost)
        //    yield break;
        //_attr.c_Value.stamina -= cost;
        //_attr.Set_stamina();
        float dashTime = 0.2f; // 快速移动的时间
        float dashSpeed = 30f; // 快速移动的速度
        float startTime = Time.time;

        if (!Free_view && !Target_lock)
        {
            yield return null;
        }else if(Free_view && !Target_lock)
        {
            sprint = true;
            while (Time.time < startTime + dashTime)
            {
                _ctr.Move(transform.forward * dashSpeed * Time.deltaTime);
                yield return null;
            }
            sprint = false;
        }else if(!Free_view && Target_lock)
        {
            sprint = true;
            // 根据输入确定冲刺方向
            Vector3 dashDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            if (dashDirection.magnitude == 0)
            {
                dashDirection = transform.forward; // 如果没有方向输入，则默认向前冲刺
            }
            else
            {
                // 将相对方向转换为世界坐标系方向
                dashDirection = transform.TransformDirection(dashDirection);
            }

            while (Time.time < startTime + dashTime)
            {
                _ctr.Move(dashDirection * dashSpeed * Time.deltaTime);
                yield return null;
            }

            sprint = false;
        }

    }



    void Key_Check()
    {
        if (Input.GetKey(KeyCode.A))
        {
            move_x = -1;
            Key_move_click = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move_x = 1;
            Key_move_click = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            move_y = 1;
            Key_move_click = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move_y = -1;
            Key_move_click = true;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            move_x = 0;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            move_y = 0;
        }
        if (move_y == 0 && move_x == 0)
        {
            Key_move_click = false;
        }
        // 检查飞行键
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (stamina_enpty || !canFly)
            //    return;
            //isFlying = true;
            playerVelocity.y = 0; // 重置垂直速度
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //if (canFly)
            //    isFlying = false;
            //else
            if (jiguangbo_ready || huongJue_ready)
                return;
            _anim.change_anim(Anim_state.Sprint);
            StartCoroutine(DashForward());
        }
        // 鼠标左键
        if (Input.GetMouseButtonUp(0))
        {
            if(jiguangbo_ready)
            {
                jiguangbo();
            }
            else if(huongJue_ready)
            {
                huongjue();
                huongjue_end();
            }
            else
            {
                Normal_atk();
            }
            
            
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            if (skill_1_cd || huongJue_ready)
                return;
            jiguangbo_start();
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            if (skill_2_cd || jiguangbo_ready)
                return;
            huongjue_start();
        }
        if (Input.GetMouseButtonDown(2))
        {
            Free_view = false;
            if (_target)
                _cam.GetComponent<Camera_move>().SetView(_cam.position, _cam.rotation);
            _cam.GetComponent<Camera2>().enabled = false;
            _cam.GetComponent<Camera_move>().enabled = true;
            _cam.GetComponent<Camera2>().target = null;
            Target_lock = false;
        }
        if (Input.GetMouseButtonUp(2))
        {
            if (_target != null)
            {
                transform.LookAt(_target);
                _cam.GetComponent<Camera_move>().enabled = false;
                _cam.GetComponent<Camera2>().enabled = true;
                _cam.GetComponent<Camera2>().target = _target;
                Target_lock = true;
            }
            else
            {
                Free_view = true;
            }
        }
        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
        //    _anim.change_anim(Anim_state.Jump);
        }
    }

    public void Set_Paused(bool p)
    {
        isPaused = p;
    }


    IEnumerator delay_change_CD(float time , int state)
    {
        yield return new WaitForSeconds(time);
        switch(state)
        {
            case 0:
                CD_atk = false;
                break;
            case 1:
                skill_1_cd = false;
                break;
            case 2:
                skill_2_cd = false;
                break;
            case 3:
                Attacking = false;
                break;
        }
        
    }

    bool Anim_lock()
    {
        bool locker = false;
        if(Attacking)
            locker = true;
        if (huongJue_ready)
            locker = true;
        if (jiguangbo_ready)
            locker = true;
        return locker;
    }

    // 延迟触发函数
    private IEnumerator DelayFunctionTrigger(float delay, System.Action callback)
    {
        // 延迟指定时间
        yield return new WaitForSeconds(delay);

        // 延迟结束后，调用回调函数
        callback?.Invoke();
    }

    //private void OnDrawGizmosSelected()
    //{

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _attr.Get_attack_distance());
    //}



}
