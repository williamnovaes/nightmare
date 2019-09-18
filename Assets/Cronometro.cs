using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cronometro : MonoBehaviour
{
    public Text text;
    private long timer;

    private float time;
    public float localTime;

    void Awake()
    {
        text.text = "00:30:00";
        localTime = 0f;
        timer = 1000 * 60 * 30;
        time = 1f;
    }

    void FixedUpdate()
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
