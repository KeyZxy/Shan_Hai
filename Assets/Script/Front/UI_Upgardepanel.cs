using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Upgardepanel : MonoBehaviour
{
    public GameObject opt_1;
    public GameObject opt_2;
    public GameObject opt_3;
    public GameObject buttom_posi;

    public GameObject others;

    private Camera_move _cam;
    private C_base _base;
    private Image _image;
    private bool isPaused = false;
    private Vector3 targetPos_1;
    private Vector3 targetPos_2;
    private Vector3 targetPos_3;
    private Vector3 startOffscreenPos_1;
    private Vector3 startOffscreenPos_2;
    private Vector3 startOffscreenPos_3;
    void Start()
    {
        _image = transform.GetComponent<Image>();
        _cam = GameObject.Find("Main Camera").transform.GetComponent<Camera_move>();
        _base = GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_base>();

        targetPos_1 = opt_1.transform.position;
        targetPos_2 = opt_2.transform.position;
        targetPos_3 = opt_3.transform.position;

        // 设置初始位置，Y 轴低于目标位置一定距离，X 和 Z 保持不变
        startOffscreenPos_1 = new Vector3(targetPos_1.x, buttom_posi.transform.position.y, targetPos_1.z);
        startOffscreenPos_2 = new Vector3(targetPos_2.x, buttom_posi.transform.position.y, targetPos_2.z);
        startOffscreenPos_3 = new Vector3(targetPos_3.x, buttom_posi.transform.position.y, targetPos_3.z);

        // 初始化 UI 到屏幕底部
        opt_1.transform.position = startOffscreenPos_1;
        opt_2.transform.position = startOffscreenPos_2;
        opt_3.transform.position = startOffscreenPos_3;

        // 隐藏 UI
        opt_1.SetActive(false);
        opt_2.SetActive(false);
        opt_3.SetActive(false);
        others.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Key_Check();
    }
    private IEnumerator ShowUIWithAnimation()
    {
        opt_1.SetActive(true);
        opt_2.SetActive(true);
        opt_3.SetActive(true);
        others.SetActive(true);

        // 启动 UI 元素上升动画
        yield return MoveUIToPosition(opt_1, targetPos_1, startOffscreenPos_1);
        yield return MoveUIToPosition(opt_2, targetPos_2, startOffscreenPos_2);
        yield return MoveUIToPosition(opt_3, targetPos_3, startOffscreenPos_3);
    }

    private IEnumerator MoveUIToPosition(GameObject uiObject, Vector3 targetPosition, Vector3 startPosition)
    {
        float duration = 0.2f; // 动画持续时间
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            uiObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            yield return null;
        }

        uiObject.transform.position = targetPosition;
    }


    void Key_Check()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (isPaused)
            {
                Hide_UI();
            }
            else
            {
                Show_UI();
            }
        }
    }

    public void Show_UI()
    {
        isPaused = true;
        Time.timeScale = 0f;
        _image.enabled = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _cam.Set_Paused(true);
        _base.Set_Paused(true);
        StartCoroutine(ShowUIWithAnimation());
    }
    public void Hide_UI()
    {
        isPaused = false;
        Time.timeScale = 1f;
        _image.enabled = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _cam.Set_Paused(false);
        _base.Set_Paused(false);
        opt_1.SetActive(false);
        opt_2.SetActive(false);
        opt_3.SetActive(false);


        opt_1.transform.position = startOffscreenPos_1;
        opt_2.transform.position = startOffscreenPos_2;
        opt_3.transform.position = startOffscreenPos_3;
    }
}
