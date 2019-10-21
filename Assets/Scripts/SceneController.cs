using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LoadNextLevel();  
    }

    private void LoadNextLevel()
    {
        Debug.Log("Load Next Scene");
        int indexCurrentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(indexCurrentScene + 1);
    }
}