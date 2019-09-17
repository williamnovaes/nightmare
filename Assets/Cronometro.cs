using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cronometro : MonoBehaviour
{
    Text text;
    long timer;

    float time;
    float localTime;

    void Awake()
    {
        text.text = "00:30:00";    
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 1000 * 60 * 30;
        text = GetComponent<Text>();
        time = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        localTime += Time.deltaTime;
        if (localTime >= time)
        {
            timer -= 1000;
            text.text = "00" + ((timer / 1000) / 60).ToString("00") + ":" + ((timer / 1000) % 60).ToString("00");
            localTime = 0f;
        }
    }
}
