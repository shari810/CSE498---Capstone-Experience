using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneButton : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        //Debug.Log("Loaded in current unlocked level.");
        SceneLoader.instance.LoadScene(sceneName);
    }
}
