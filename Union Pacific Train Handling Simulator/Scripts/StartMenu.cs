using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject audioMenu;

    public GameObject[] levels;
    public GameObject[] subLevels;
    public int selectedLevel = 0;

    private AudioSource clickSound;
    private AudioSource spec2Sound;
    private AudioSource spec3Sound;
    private AudioSource spec5Sound;

    private static bool initializedApp = false;

    public void NextLevel()
    {
        clickSound.ignoreListenerPause = true;
        clickSound.Play();

        levels[selectedLevel].SetActive(false);
        selectedLevel = (selectedLevel + 1) % levels.Length;
        levels[selectedLevel].SetActive(true);

        UpdateText();
    }

    public void PreviousLevel()
    {
        clickSound.ignoreListenerPause = true;
        clickSound.Play();

        levels[selectedLevel].SetActive(false);
        selectedLevel--;
        if (selectedLevel < 0)
        {
            selectedLevel += levels.Length;
        }
        levels[selectedLevel].SetActive(true);

        UpdateText();
    }

    /// <summary>
    /// Updates the track and train info box next to the track picture
    /// </summary>
    private void UpdateText()
    {
        TMPro.TextMeshProUGUI trackInfo = levelMenu.transform.GetChild(6).GetComponent<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI trainInfo = levelMenu.transform.GetChild(9).GetComponent<TMPro.TextMeshProUGUI>();
        switch (selectedLevel)
        {
            case 0:  // coal
                trackInfo.text = levelMenu.transform.GetChild(10).GetChild(0).name;
                trainInfo.text = "2x0x2\n135 Cars";
                break;
            case 1:  // stone 68
                trackInfo.text = levelMenu.transform.GetChild(10).GetChild(1).name;
                trainInfo.text = "3x0x0\n68 Cars";
                break;
            case 2:  // grain 85
                trackInfo.text = levelMenu.transform.GetChild(10).GetChild(2).name;
                trainInfo.text = "2x0x0\n85 Cars";
                break;
            case 3:  // frt mixed
                trackInfo.text = levelMenu.transform.GetChild(10).GetChild(3).name;
                trainInfo.text = "2x0x0\n93 Cars";
                break;
            case 4:  // mixed frt heavy long
                trackInfo.text = levelMenu.transform.GetChild(10).GetChild(4).name;
                trainInfo.text = "3x0x0\n132 Cars";
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(false);
        audioMenu.gameObject.SetActive(false);
        UnlockLevels();

        // Initialize sounds
        clickSound = GameObject.Find("SoundBank/UI/Click").GetComponent<AudioSource>();
        spec2Sound = GameObject.Find("SoundBank/UI/Special2").GetComponent<AudioSource>();  // select level
        spec3Sound = GameObject.Find("SoundBank/UI/Special3").GetComponent<AudioSource>();
        spec5Sound = GameObject.Find("SoundBank/UI/Special5").GetComponent<AudioSource>();

        if (!initializedApp)
            FixThrottles();
    }

    /// <summary>
    /// Toggles between the main menu and the level select menu
    /// </summary>
    public void OnStartToLevelSelect()
    {
        spec5Sound.ignoreListenerPause = true;
        spec5Sound.Play();

        mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
        levelMenu.gameObject.SetActive(!levelMenu.gameObject.activeSelf);
    }

    /// <summary>
    /// Toggles between the main menu and the options menu
    /// </summary>
    public void OnStartToOptionsMenu()
    {
        spec5Sound.ignoreListenerPause = true;
        spec5Sound.Play();

        mainMenu.gameObject.SetActive(!mainMenu.gameObject.activeSelf);
        optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeSelf);
    }

    /// <summary>
    /// Toggles between the options menu and the audio menu
    /// </summary>
    public void OnOptionsMenutoAudioMenu()
    {
        spec5Sound.ignoreListenerPause = true;
        spec5Sound.Play();

        optionsMenu.gameObject.SetActive(!optionsMenu.gameObject.activeSelf);
        audioMenu.gameObject.SetActive(!audioMenu.gameObject.activeSelf);
    }

    /// <summary>
    /// Button event to exit the game
    /// </summary>
    public void OnQuitButton()
    {
        spec5Sound.ignoreListenerPause = true;
        spec5Sound.Play();

        Application.Quit();
        UnityEngine.Debug.Log("Quitting game now");
    }

    /// <summary>
    /// Unlocks the levels based on the save file or the level unlock indicator in the game manager
    /// </summary>
    private void UnlockLevels()
    {
        int levelsToUnlock = GameManager.S.levelUnlockIndicator;
        Button subLevel = subLevels[0].GetComponent<Button>();

        if (levelsToUnlock == 0)
        {
            int curr = 0;
            subLevel = subLevels[curr].GetComponent<Button>();
            subLevel.interactable = true;
            subLevel.transform.GetChild(1).GetComponent<RawImage>().enabled = false;

            subLevel.onClick.AddListener(delegate {
                SceneLoader.instance.LoadScene(GameManager.S.GetSceneName(curr));
                spec2Sound.ignoreListenerPause = true;
                spec2Sound.Play();
            });
        }

        for (int i = 0; i < levelsToUnlock; i++)
        {
            int curr = i;
            subLevel = subLevels[curr].GetComponent<Button>();
            subLevel.interactable = true;
            subLevel.transform.GetChild(1).GetComponent<RawImage>().enabled = false;
            
            subLevel.onClick.AddListener(delegate { 
                SceneLoader.instance.LoadScene(GameManager.S.GetSceneName(curr));
                spec2Sound.ignoreListenerPause = true;
                spec2Sound.Play();
            });
        }

    }

    /// <summary>
    /// Runs the powershell script to unblock the DieselElectrics.ndll file
    /// </summary>
    public void FixThrottles()
    {
        //string pathToExe = Application.dataPath.Replace(@"/",@"\");
        //string command = "Set-Location \"SimulationBuild\\General Content\\Trains\"" +
        //"$ndllpath = Resolve - Path - Path \"DieselElectrics.ndll\"" +
        //"sp $ndllpath IsReadOnly $false" +
        //"unblock - file - path $ndllpath - Confirm:$false - Verbose\"";

        //Process process = Process.Start("powershell.exe", command);
        //process.WaitForExit();
        //process.Close();
        
        // Modified from above since fixing method changed
        string dataPath = Application.dataPath;
        dataPath += "/../";  // since we're on Windows
        //UnityEngine.Debug.Log(dataPath + "RUN_ME_IF_THROTTLES_MISSING.bat");
        Process.Start(dataPath + "RUN_ME_IF_THROTTLES_MISSING.bat");  // run the fixer, thanks Nhat for the how-to above
        initializedApp = true;
    }

}
