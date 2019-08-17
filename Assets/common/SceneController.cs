using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string sceneName1;
    public string sceneName2;
    public string sceneName3;
    public string sceneName4;
    public string sceneName5;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            this.ChangeScene(sceneName1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            this.ChangeScene(sceneName2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            this.ChangeScene(sceneName3);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            this.ChangeScene(sceneName4);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha5))
        {
            this.ChangeScene(sceneName5);
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (sceneName == "") { return; }
        if (SceneManager.GetActiveScene().name == sceneName) { return; }
        SceneManager.LoadScene(sceneName);
    }
}