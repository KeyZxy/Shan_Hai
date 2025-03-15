using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI; // ��ͣ�˵���UI���  
    public static bool GameIsPaused = false;
    private Camera_move _cam;
    private C_base _base;

    void Start()
    {
        _cam = GameObject.Find("Main Camera").transform.GetComponent<Camera_move>();
        _base = GameObject.FindGameObjectWithTag(SaveKey.Character).GetComponent<C_base>();
    }

    void Update()
    {
        // ����ESC��ʱ������ͣ  
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Paused();
            }
        }
    }

    // �ָ���Ϸ  
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // ������ͣ�˵�  
        Time.timeScale = 1f;          // �ָ���Ϸʱ������  
        GameIsPaused = false;         // ������ͣ״̬  
        _cam.Set_Paused(false);
        _base.Set_Paused(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // ��ͣ��Ϸ  
    void Paused()
    {
        pauseMenuUI.SetActive(true);  // ��ʾ��ͣ�˵�  
        Time.timeScale = 0f;          // ��ͣ��Ϸʱ������  
        GameIsPaused = true;          // ������ͣ״̬  
        _cam.Set_Paused(true);
        _base.Set_Paused(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // �˳���Ϸ  
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

#if UNITY_EDITOR
        // ����ڱ༭���У�ֹͣ����ģʽ  
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ����ڹ�������Ϸ�У��˳�Ӧ�ó���  
        Application.Quit();  
#endif
    }
}