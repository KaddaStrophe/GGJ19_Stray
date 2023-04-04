using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameJamInputController : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("Title");
           // SceneManager.UnloadSceneAsync("Level1");
           // SceneManager.UnloadSceneAsync("Level2");

        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("Level1");
            //SceneManager.UnloadSceneAsync("Title");           
            //SceneManager.UnloadSceneAsync("Level2");
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene("Level2");
            //SceneManager.UnloadSceneAsync("Level1");
           // SceneManager.UnloadSceneAsync("Title");
            
        }
    }
}
