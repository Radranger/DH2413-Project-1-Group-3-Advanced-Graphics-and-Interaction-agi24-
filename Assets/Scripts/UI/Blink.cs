using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    private TMPro.TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            text.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(2.0f);
            text.color = new Color(0.1f, 0.1f, 0.13f, 1);
            yield return new WaitForSeconds(0.4f);
        }

    }
}
