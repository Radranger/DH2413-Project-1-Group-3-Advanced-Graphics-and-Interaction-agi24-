using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // 这个方法将被调用来加载 Build Settings 中的第一个场景
    public void LoadFirstScene()
    {
        // 加载 Build Settings 中索引为 0 的场景
        SceneManager.LoadScene(0);
    }
}
