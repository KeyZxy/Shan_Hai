using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDText : MonoBehaviour
{

    public Transform target;
    private Transform HUD;
    private Rigidbody2D ri;
    public string damage;
    public int state;       // 1.角色掉血  2.怪物掉血  3.治疗  4.miss  
                            // 5.怪物被暴击掉血  6.魔法 7.角色被暴击掉血 
                            // 8.暴击治疗 9.吸收伤害 10.毒伤害



    private float Start_offset;
    private int throwForceX = 3000;
    private int throwForceY = 10000;
    private bool isThrown = false;
    private float originalY;
    private float disappear_distance = 75f;

    private float currentSize = 50f;
    private float targetSize = 140f;
    private bool siza_up = true;
    private Vector3 original_posi;

    private Vector3 floatingOffset;

    // Use this for initialization
    void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").transform.Find("HUD_Group").transform);
        ri = transform.GetComponent<Rigidbody2D>();
        // 初始化位置偏移，用于控制字体飘动效果
        floatingOffset = new Vector3(0, 50f, 0);  // 根据需要调整初始偏移量
    }


    void FixedUpdate()
    {

        try
        {
            if (isThrown)
            {
                switch (state)
                {
                    case 1:
                        ri.AddForce(new Vector2(throwForceX, throwForceY));
                        break;
                    case 2:
                        ri.AddForce(new Vector2(-throwForceX, throwForceY));
                        break;
                    case 9:
                        ri.AddForce(new Vector2(0, throwForceY));
                        break;
                    case 10:
                        ri.AddForce(new Vector2(throwForceX, throwForceY));
                        break;
                }

                isThrown = false;
            }
        }
        catch
        {
            Destroy(transform.gameObject);
        }
        if (state == 3)
        {
            transform.Translate(Vector3.up * 150f * Time.deltaTime);
            if (transform.position.y - originalY >= 150f)
            {
                Destroy(transform.gameObject);
            }
        }

        if (state == 5)
        {
            enemy_get_crit();
        }
        if (state == 7)
        {
            character_get_crit();
        }
        if (state == 8)
        {
            heal_crit();
        }
        if (originalY - transform.position.y >= disappear_distance)
        {
            Destroy(transform.gameObject);
        }
    }

    void heal_crit()
    {
        // 暴击，先让字体变大再缩小
        if (siza_up)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 10f);
            HUD.GetComponent<Text>().text = string.Format("<color=#00FF00><size={0}>{1}</size></color>", Mathf.RoundToInt(currentSize), damage);
            if (currentSize >= 138)
            {
                siza_up = false;
                targetSize = 50f;
            }
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 10f);
            HUD.GetComponent<Text>().text = string.Format("<color=#00FF00><size={0}>{1}</size></color>", Mathf.RoundToInt(currentSize), damage);
        }
        transform.Translate(Vector3.up * 150f * Time.deltaTime);
        if (transform.position.y - originalY >= 200f)
        {
            Destroy(transform.gameObject);
        }
    }

    void enemy_get_crit()
    {
        // 暴击，先让字体变大再缩小
        if (siza_up)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 10f);
            HUD.GetComponent<Text>().text = string.Format("<color=#FF923EFF><size={0}>{1}</size></color>", Mathf.RoundToInt(currentSize), damage);
            if (currentSize >= 138)
            {
                siza_up = false;
                targetSize = 50f;
            }
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 10f);
            HUD.GetComponent<Text>().text = string.Format("<color=#FF923EFF><size={0}>{1}</size></color>", Mathf.RoundToInt(currentSize), damage);
        }
        // 暴击时移动字体
        transform.position = Vector3.Lerp(transform.position, original_posi, Time.deltaTime * 10f);
        if (transform.position.x + 0.01f >= original_posi.x)
        {
            Destroy(gameObject);
        }
    }

    void character_get_crit()
    {
        // 暴击，先让字体变大再缩小
        if (siza_up)
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 10f);
            HUD.GetComponent<Text>().text = string.Format("<color=#FFFFFF><size={0}>{1}</size></color>", Mathf.RoundToInt(currentSize), damage);
            if (currentSize >= 138)
            {
                siza_up = false;
                targetSize = 50f;
            }
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * 10f);
            HUD.GetComponent<Text>().text = string.Format("<color=#FFFFFF><size={0}>{1}</size></color>", Mathf.RoundToInt(currentSize), damage);
        }
        // 暴击时移动字体
        transform.position = Vector3.Lerp(transform.position, original_posi, Time.deltaTime * 10f);
        if (transform.position.x - 0.01f <= original_posi.x)
        {
            Destroy(gameObject);
        }
    }

    void WorldToUIPoint(Transform worldGo)
    {
        Vector3 v_v3 = Camera.main.WorldToScreenPoint(worldGo.position);
        transform.position = Camera.main.WorldToScreenPoint(worldGo.position);
    }

    public void Init(Transform tr, int dam, int s, int offset)
    {
        target = tr;
        damage = dam.ToString();
        state = s;
        Start_offset = offset;
        Vector3 v_v3 = Camera.main.WorldToScreenPoint(target.position);
        transform.position = new Vector3(v_v3.x, v_v3.y + Start_offset, v_v3.z);
        originalY = transform.position.y;
        isThrown = true;

        if (HUD == null)
        {
            HUD = transform.Find("HUD").transform;
            //			HUD.GetComponent<Text> ().text = damage;
            switch (state)
            {
                case 1: // 1.角色掉血
                    HUD.GetComponent<Text>().text = string.Format("<color=#FFFFFF><size=50>" + damage + "</size></color>");
                    break;
                case 2: // 2.怪物掉血
                    HUD.GetComponent<Text>().text = string.Format("<color=#FF923EFF><size=50>" + damage + "</size></color>");
                    break;
                case 3: // 3.治疗
                    HUD.GetComponent<Text>().text = string.Format("<color=#00FF00><size=50>" + damage + "</size></color>");
                    transform.GetComponent<Rigidbody2D>().isKinematic = true;
                    break;
                case 4: // 4.miss
                    HUD.GetComponent<Text>().text = string.Format("<color=#FFF300><size=50>miss</size></color>");
                    break;
                case 5: // 5.怪物被暴击掉血
                    HUD.GetComponent<Text>().text = string.Format("<color=#FF923EFF><size=50>" + damage + "</size></color>");
                    v_v3 = Camera.main.WorldToScreenPoint(target.position);
                    original_posi = new Vector3(v_v3.x, v_v3.y + Start_offset, v_v3.z);
                    transform.position = new Vector3(v_v3.x - 90f, v_v3.y + Start_offset, v_v3.z);
                    transform.GetComponent<Rigidbody2D>().isKinematic = true;
                    break;
                case 6:// 6.魔法
                    HUD.GetComponent<Text>().text = string.Format("<color=#006EFF><size=50>" + damage + "</size></color>");
                    break;
                case 7:// 7.角色被暴击掉血 
                    HUD.GetComponent<Text>().text = string.Format("<color=#FFFFFF><size=50>" + damage + "</size></color>");
                    v_v3 = Camera.main.WorldToScreenPoint(target.position);
                    original_posi = new Vector3(v_v3.x, v_v3.y + Start_offset, v_v3.z);
                    transform.position = new Vector3(v_v3.x + 90f, v_v3.y + Start_offset, v_v3.z);
                    transform.GetComponent<Rigidbody2D>().isKinematic = true;
                    break;
                case 8: // 8.暴击治疗
                    HUD.GetComponent<Text>().text = string.Format("<color=#00FF00><size=50>" + damage + "</size></color>");
                    transform.GetComponent<Rigidbody2D>().isKinematic = true;
                    break;
                case 9: // 9.吸收伤害
                    HUD.GetComponent<Text>().text = string.Format("<color=#C03EFF><size=50>吸收</size></color>");
                    break;
                case 10: // 10.毒伤害
                    HUD.GetComponent<Text>().text = string.Format("<color=#B000C1><size=50>" + damage + "</size></color>");
                    break;
            }
        }
    }



}
