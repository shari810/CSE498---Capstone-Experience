using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour, IDataPersistence
{
    // Singleton
    public static GameManager S = null;

    [Tooltip("How many real life meters represent one unity meter.")]
    [SerializeField] private float metersPerUnityMeter;

    [Tooltip("How many kilograms of cargo will be represented by one unity meter.")]
    [SerializeField] private float kilogramsPerUnityMeter;

    private static float KILOMETERS_PER_MILES = 1.60934f;

    public static bool GameisOver = false;

    public static bool Victory = false;
    public static bool isPaused = false;

    private AudioSource failSound;

    // using Dictionary<TKey,TValue> class
    // Dictionary of all levels are their associated number with levelUnlockedIndicator.
    // This list will be looped through to determine if the user has beaten this level.
    // And then the levelUnlockedIndicator will be incremented if the current level's number
    // is greater than the current levelUnlockedIndicator.
    private static Dictionary<string, int> levelsDict =
                   new Dictionary<string, int>()
                   {
                       {"Tutorial_1", 0}, {"Tutorial_2", 1}, {"Tutorial_3", 2}, {"Tutorial_4", 3}, {"Tutorial_5", 4},
                       {"Lakeside_1", 5},{"Lakeside_2", 6},{"Lakeside_3", 7},{"Lakeside_4", 8},{"Lakeside_5", 9},
                       {"Portland_1", 10},{"Portland_2", 11},{"Portland_3", 12},{"Portland_4", 13},{"Portland_5", 14},
                       {"Alhambra_1", 15},{"Alhambra_2", 16},{"Alhambra_3", 17},{"Alhambra_4", 18},{"Alhambra_5", 19},
                       {"Lordsburg_1", 20}, {"Lordsburg_2", 21},{"Lordsburg_3", 22},{"Lordsburg_4", 23},{"Lordsburg_5", 24},
                   };

    [Header("Level Segment Info")]
    [Tooltip("Each of the level segment scenes.")]
    [SerializeField] private List<string> levelSegmentScenes = new List<string>();

    [Tooltip("How many levels are unlocked?")]
    [SerializeField] public int levelUnlockIndicator = 4;

    void Awake()
    {
        if (GameManager.S == null)
        {
            S = this;
        }


    }

    private void Start()
    {
        ResetGame();
        failSound = GameObject.Find("SoundBank/UI/Fail").GetComponent<AudioSource>();
    }

    /// <summary>
    /// Converts a measurement in real life meters to a measurement in Unity meters.
    /// </summary>
    /// <param name="meters">Measurement in real life meters</param>
    /// <returns>Measurement in Unity meters</returns>
    public float ConvertMetersToUnityMeters(float meters)
    {
        return meters / metersPerUnityMeter;
    }

    /// <summary>
    /// Converts a weight in real life kilograms to a height in Unity meters.
    /// </summary>
    /// <param name="kilograms">Weight in real life kilograms</param>
    /// <returns>Height in Unity meters</returns>
    public float CalculateTrainCarHeight(float kilograms)
    {
        return kilograms / kilogramsPerUnityMeter;
    }

    public float ConvertMilesToKilometers(float miles)
    {
        return miles * KILOMETERS_PER_MILES;
    }

    public void GameOver(string reason="FORCES TOO HIGH")
    {
        GameisOver = true;
        failSound.ignoreListenerPause = true;
        failSound.Play();
        Time.timeScale = 0f;
        GameObject.Find("GameOver/GameOverScreen/FORCES TOO HIGH").GetComponent<Text>().text = reason;
        GameObject.Find("GameOver").GetComponent<Canvas>().enabled = true;
        GameObject.Find("Throttle Control UI").GetComponent<Canvas>().enabled = false;
    }

    public void Winning(string message="You have successfully completed the level!")
    {
        ProgressIncrementCheck();
        Victory = true;
        Time.timeScale = 0f;
        GameObject.Find("Victory/VictoryScreen/SUCCESS").GetComponent<Text>().text = message;
        GameObject.Find("Victory").GetComponent<Canvas>().enabled = true;
        GameObject.Find("Throttle Control UI").GetComponent<Canvas>().enabled = false;
    }

    private void ResetGame()
    {
        //ThrottleControl throttleControl = FindObjectOfType<ThrottleControl>();
        //throttleControl.ResetForces();
        Time.timeScale = 1f;
        MoveFence.fenceValue = 0;
        GameisOver = false;
        Victory = false;
        try
        {
            GameObject.Find("GameOver").GetComponent<Canvas>().enabled = false;
            GameObject.Find("Victory").GetComponent<Canvas>().enabled = false;
            GameObject.Find("Throttle Control UI").GetComponent<Canvas>().enabled = true;
        }
        catch { }
    }

    public void LoadData(GameData data)
    {
        this.levelUnlockIndicator = data.levelUnlocked;
    }

    public void SaveData(ref GameData data)
    {
        data.levelUnlocked = this.levelUnlockIndicator;
        Debug.Log("saved level data: " + this.levelUnlockIndicator);
    }

    public string GetSceneName(int index)
    {
        return levelSegmentScenes[index];
    }

    

    public void ProgressIncrementCheck()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        int currentLevel = levelsDict[sceneName];
        Debug.Log("Current level name is: " + sceneName);
        Debug.Log("Current level number is: " + currentLevel);
        if(currentLevel >= levelUnlockIndicator)
        {
            Debug.Log("Level Indicator was: " + levelUnlockIndicator);
            levelUnlockIndicator = currentLevel+1;
        }
        Debug.Log("Level unlocked is now" + levelUnlockIndicator);
    }

    public void ResetSave()
    {
        levelUnlockIndicator = 0;
        //ResetGame();
        //SceneManager.LoadScene("MainMenu");
    }
}
