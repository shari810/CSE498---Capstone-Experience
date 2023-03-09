using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class FileDataHandler
{

    // directory where data is stored
    private string dataDirPath = "SaveFiles";
    // name of file where data is stored
    private string dataFileName = "";

    private bool useEncryption = false;
    private readonly string encryptionCode = "Jeff";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;

    }

    public GameData Load()
    {
        // use Path.Combine to acount for different OS's having different path separators

        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                // load the serialized data from file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }


                //decrypt data
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                //deserialize the data from Json back into the C# object
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file." + fullPath + "\n" + e);
            }
        }

        Debug.Log("Data loaded from file: " + fullPath);
        return loadedData;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            //create directory path in case it doesn't exist yet
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize the C# game data into JSON
            string dataToStore = JsonUtility.ToJson(data, true);

            //encrypt data
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }


            //write to File
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file." + fullPath + "\n" + e);
        }

        Debug.Log("Data saved to file: " + fullPath);
    }

     private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        // Change data with XOR operation with data and Code word
        for(int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCode[i % encryptionCode.Length]);
        }

        return modifiedData;
    }
}
