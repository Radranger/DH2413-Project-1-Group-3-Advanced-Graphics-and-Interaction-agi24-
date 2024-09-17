using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;  // 拖放暂停页面 Panel 到此字段

    private bool isPaused = false;

    void Start()
    {
        // 确保游戏开始时暂停页面是隐藏的
        pausePanel.SetActive(false);
    }

    void Update()
    {
        // 按下 ESC 键时切换暂停状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;  // 暂停或恢复游戏时间
    }
}
