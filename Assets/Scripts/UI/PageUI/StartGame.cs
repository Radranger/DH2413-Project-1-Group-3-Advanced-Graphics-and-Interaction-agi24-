using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void StartMenu()
    {
        SceneManager.LoadScene("Scenes/MAIN_SCENE");
    }
}

