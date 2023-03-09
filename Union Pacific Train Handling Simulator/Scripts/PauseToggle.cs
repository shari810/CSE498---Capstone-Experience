using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseToggle : MonoBehaviour
{
    float prevTimeScale = 50;

    [SerializeField] Button pauseButton;
    [SerializeField] Sprite pauseSprite;
    [SerializeField] Sprite continueSprite;

    Transform throttles;


    // Start is called before the first frame update
    void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
        GameManager.isPaused = false;
        //throttles = transform.parent.parent.Find("Throttle Control UI");
        Transform parent = transform.parent;
        Transform grandparent = parent.parent;
        //throttles = grandparent.Find("Throttle Control UI");

        var buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            button.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePause()
    {
        // Pause
        if (Time.timeScale > 0)
        {
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0;
            //AudioListener.pause = true;
            GameManager.isPaused = true;
            pauseButton.GetComponent<Image>().sprite = continueSprite;

            var buttons = FindObjectsOfType<Button>();

            foreach (Button button in buttons)
            {
                if (!button.GetComponent<PauseToggle>())
                {
                    button.enabled = false;
                }
            }
        }

        // Resume
        else if (Time.timeScale == 0)
        {
            Time.timeScale = prevTimeScale;
            AudioListener.pause = false;
            GameManager.isPaused = false;
            pauseButton.GetComponent<Image>().sprite = pauseSprite;

            var buttons = FindObjectsOfType<Button>();

            foreach (Button button in buttons)
            {
                button.enabled = true;
            }
        }
    }
}
