using UnityEngine;
using UnityEngine.UI;  // 导入 UI 命名空间以使用 Button 组件

public class ResumeButtonHandler : MonoBehaviour
{
    public GameObject pausePanel;  // 拖放暂停页面 Panel 到此字段

    void Start()
    {
        // 确保按钮被正确设置
        Button resumeButton = GetComponent<Button>();
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
        }
    }

    // 确保这个方法是 public 的，且没有参数
    public void OnResumeButtonClicked()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;  // 恢复游戏时间
        }
    }
}