using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pauseMenuUI; // ��ͣ�˵���UI���
    public static bool GameIsPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
    }

    // ��ͣ��Ϸ
    void Paused()
    {
        pauseMenuUI.SetActive(true);  // ��ʾ��ͣ�˵�
        Time.timeScale = 0f;          // ��ͣ��Ϸʱ������
        GameIsPaused = true;          // ������ͣ״̬
    }
    // �˳���Ϸ
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
