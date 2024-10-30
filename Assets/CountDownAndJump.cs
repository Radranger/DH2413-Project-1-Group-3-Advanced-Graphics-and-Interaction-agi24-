using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountDownAndJump : MonoBehaviour
{
    public string sceneName = "MAIN_SCENE_2"; 
        private float countdown = 10f; 
        public TextMeshProUGUI countdownText; 

        void Update()
        {
            countdown -= Time.deltaTime; 
            if(countdown >= -1) countdownText.text = Mathf.Ceil(countdown).ToString();
            if (countdown <= 0) SceneManager.LoadScene(sceneName);
        }
}
