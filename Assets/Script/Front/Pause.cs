using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI; // 暂停菜单的UI面板  
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
        // 按下ESC键时触发暂停  
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

    // 恢复游戏  
    public void Resume()
    {
        pauseMenuUI.SetActive(false); // 隐藏暂停菜单  
        Time.timeScale = 1f;          // 恢复游戏时间流动  
        GameIsPaused = false;         // 更新暂停状态  
        _cam.Set_Paused(false);
        _base.Set_Paused(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 暂停游戏  
    void Paused()
    {
        pauseMenuUI.SetActive(true);  // 显示暂停菜单  
        Time.timeScale = 0f;          // 暂停游戏时间流动  
        GameIsPaused = true;          // 更新暂停状态  
        _cam.Set_Paused(true);
        _base.Set_Paused(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 退出游戏  
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

#if UNITY_EDITOR
        // 如果在编辑器中，停止播放模式  
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 如果在构建的游戏中，退出应用程序  
        Application.Quit();  
#endif
    }
}