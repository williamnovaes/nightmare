using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameSession : MonoBehaviour
{

    public Text cronometere;
    private long timer;

    private float time;
    public float localTime;

    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        cronometere.text = "00:30:00";
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
            cronometere.text = "00:" + ((timer / 1000) / 60).ToString("00") + ":" + ((timer / 1000) % 60).ToString("00");
            localTime = 0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
