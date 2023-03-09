using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
    // Start is called before the first frame update
    public string track_file;
    public double start_milepost;
    //public int levelID;
    public int levelUnlocked;


    // initial values in constructor are default values
    // perform normal startup if no data to load
    public GameData()
    {
        this.track_file = "";  //possibly not needed anymore
        this.start_milepost = 0;
        this.levelUnlocked = 4;  //default setting
    }

    public void ResetProgress()
    {
        levelUnlocked = 0;
    }
}
