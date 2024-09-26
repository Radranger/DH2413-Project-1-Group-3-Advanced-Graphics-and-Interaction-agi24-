using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToFirstScene : MonoBehaviour
{
    void Update()
    {
        // 检测 R 键是否被按下
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 加载 Build Settings 中第一个场景
            SceneManager.LoadScene(0);
        }
    }
}
