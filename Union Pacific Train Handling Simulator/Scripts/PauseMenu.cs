using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update

    //public static bool isPaused = false;
    private static PauseToggle toggle;
    public GameObject pauseMenuUI;
    public GameObject audioMenu;


    // Update is called once per frame
    void Update()
    {
        
        if (!GameManager.isPaused )   //  if true Pause -> Resume
        {
            Resume();


        }
        else
        {
            Pause();
        }




    }



    public void Resume()
    {
        //Debug.Log("Paused the game.");
        pauseMenuUI.SetActive(false);
        GameManager.isPaused = false;
    }
    public void Pause()
    {
        //Debug.Log("Resumed the game.");
        pauseMenuUI.SetActive(true);
        GameManager.isPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("return to menu");
        //Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        Debug.Log("Restarting Level");
        //Time.timeScale = 1f;
        //SceneManager.LoadScene("Current_Scene_Name");

    }

    public void QuitGame()
    {
        Debug.Log("Quitting game from pause menu");
        Application.Quit();
    }


    public void OpenCloseAudioMenu()
    {
        //Debug.Log("Paused the game.");
        //pauseMenuUI.SetActive(false);
        pauseMenuUI.gameObject.SetActive(!pauseMenuUI.gameObject.activeSelf);
        audioMenu.gameObject.SetActive(!audioMenu.gameObject.activeSelf);
    }
}
