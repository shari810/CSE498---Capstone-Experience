using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* This class keeps track of the games current data.*/


public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Config")]

    [SerializeField] private string fileName;

    [SerializeField] private bool useEncryption;

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;

    private FileDataHandler dataHandler;




    public static DataPersistenceManager instance { get; private set; }


    public void Awake()
    {

        if ( instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Mange in the scene.");
        }

        instance = this;

        //LoadGame();
    }


    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load any saved data from data handler
        // Otherwise initialize to default


        // load any saved data from a file using th data handler
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();

        }
            // push the loaded data to all other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {

                dataPersistenceObj.LoadData(gameData);

            }
        Debug.Log("Loaded unlocked level status = " + gameData.levelUnlocked);
        //Debug.Log("Loaded track file = " + gameData.track_file);
        //Debug.Log("Loaded start milepost = " + gameData.start_milepost);
        
        

        //Send data to other scripts
    }

    public void SaveGame()
    {
        // pass data to other scripts to update
        // save data to af ile using data handler

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);

        }
        Debug.Log("Saved level unlock status = " + gameData.levelUnlocked);
        // Debug.Log("Saved track file = " + gameData.track_file);
        //Debug.Log("Current Milepost = " + gameData.start_milepost);


        // save data to a file using data handler
        dataHandler.Save(gameData);

    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit the Application. Saving the application.");
        SaveGame();
    }



    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);

    }


    public void ResetSave()
    {
        gameData.ResetProgress();
        //gameData.levelUnlocked = 0;
        Debug.Log("Reset progress.");
        SaveGame();
        Debug.Log("Save after reset.");
    }
}
