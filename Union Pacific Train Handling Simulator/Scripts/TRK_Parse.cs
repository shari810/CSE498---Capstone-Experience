using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;                   
using System.Xml;                                   
using System.Xml.Serialization;                     
using System.IO;
using System.Xml.Linq;                              

public class TRK_Parse : MonoBehaviour, IDataPersistence
{
    [Tooltip("Path to the desired Track File")]
    [SerializeField] private string trackPath = "Assets/Routes/straight100/General Content/World/straight100.trk";

    [Tooltip("Do we want to print out all points to the console?")]
    [SerializeField] private bool showInConsole = false;

    public List<TRKPoint> xys = new List<TRKPoint>();      // Initialize List of XMLData object

    XDocument TrackFile;                            // Create Xdocument used to read TRK file
    IEnumerable<XElement> pts;                      // Create an Ienumerable list used to store TRK pts
    private int point = 0;
    private float x = 0, y = 0;
    private bool finishedLoading = false;

    private void Start()
    {
        LoadTRK();                                  // Loads TRK File
        StartCoroutine(AssignData());               // Starts assigning TRK data to xys List
    }
    
    private void LoadTRK()
    {
        // Loads the TRK file from the file path into XDocument TrackFile
        TrackFile = XDocument.Load(trackPath);

        // Breaks down TRK Document into points
        pts = TrackFile.Descendants("pt");
    }
    
    // Coroutine that will read and assign the TRK data to xys List
    IEnumerator AssignData()
    {
        // foreach looks at every point in TRK file
        foreach(var pt in pts)
        { 
           
            x = float.Parse(pt.Attribute("x").Value);
            y = float.Parse(pt.Attribute("y").Value);

            // Add Track point TRKPoint Object to xys List
            xys.Add(new TRKPoint(x, y));
                
            // Print out the point, x value, and y value for debugging
            if (showInConsole)
            {
                Debug.Log("Point: " + point + "\nx: " + xys[point].x + " y: " + xys[point].y);
            }

            point++;
        }

        finishedLoading = true; // Tell program we've finished loading points
        yield return null;
    }

    public bool IsFinishedLoading()
    {
        return finishedLoading;
    }



    public void LoadData(GameData data)
    {
        this.trackPath = data.track_file;
    }

    public void SaveData(ref GameData data)
    {
        data.track_file = this.trackPath;
    }
}




// Assigns TRK points to objects in a list
public class TRKPoint {
    
    public float x, y;
    
    // Constructor that accepts an x and y value
    public TRKPoint(float xVal, float yVal)
    {
        x = xVal;
        y = yVal;
    }
}



