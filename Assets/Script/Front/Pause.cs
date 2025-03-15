using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI; // 暂停菜单的UI面板
    public static bool GameIsPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
    }

    // 暂停游戏
    void Paused()
    {
        pauseMenuUI.SetActive(true);  // 显示暂停菜单
        Time.timeScale = 0f;          // 暂停游戏时间流动
        GameIsPaused = true;          // 更新暂停状态
    }
    // 退出游戏
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
