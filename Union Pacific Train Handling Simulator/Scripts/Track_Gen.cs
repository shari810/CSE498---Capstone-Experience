using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor.SceneManagement;
#endif
using UnityEngine.UI;
using TMPro;

public class Track_Gen : MonoBehaviour
{
    [Tooltip("Sprite Shape Controller component of the track")]
    [SerializeField] private SpriteShapeController track;

    [Tooltip("Path to the prefabs folder")]
    [SerializeField] private string prefabPath;

    [Tooltip("Do we want the ENTIRE Track File to be generated")]
    [SerializeField] private bool generateWholeTrack = false;

    [Header("Level Values")]
    [Tooltip("What should the name of the level be?")]
    [SerializeField] private string levelName;

    [Tooltip("Start point of our track in miles")]
    [SerializeField] private float startMile;

    [Tooltip("End point of our track in miles")]
    [SerializeField] private float endMile;

    [Tooltip("Samples the dataset for the every pointDenom amount of points")]
    [SerializeField] private int pointDenom = 1;

    private TRK_Parse parse;

    public GameObject milepost;

    void Start()
    {
        parse = GetComponent<TRK_Parse>();

        // Format points to start at x = 0
        List<TRKPoint> points = parse.xys;
        float firstX = points[0].x;
        for (int i = 0; i < points.Count; i++)
        {
            points[i].x -= firstX;
        }

        int start = -1;
        int end = -1;

        float startMeter = 0f;
        float endMeter = 0f;

        // Determine bounds
        if (generateWholeTrack)
        {
            start = 0;
            end = points.Count;
        }
        else
        {
            startMeter = GameManager.S.ConvertMilesToKilometers(startMile) * 1000f;
            endMeter = GameManager.S.ConvertMilesToKilometers(endMile) * 1000f;
            for (int i = 0; i < points.Count; i++)
            {
                if (start < 0)
                {
                    if (points[i].x >= startMeter)
                    {
                        start = i;
                    }
                }
                else
                {
                    if (points[i].x >= endMeter)
                    {
                        end = i;
                        break;
                    }
                }
            }
        }

        // Add sample points to the Sprite Shape
        int index = 0;
        float x = 0f;
        float y = 0f;
        int i_;
        float distanceFromLastMP = 0f;
        float MilepostPosition = 0f;
        int milepostNumber = 0;

        for (i_ = start; i_ <= end; i_ += pointDenom)
        {
            x = GameManager.S.ConvertMetersToUnityMeters(points[i_].x);
            y = GameManager.S.ConvertMetersToUnityMeters(points[i_].y);

            if (index < 4)
            {
                track.spline.SetPosition(index, new Vector3(x, y));
                track.spline.SetTangentMode(index, ShapeTangentMode.Linear);
            }
            else
            {
                track.spline.InsertPointAt(index, new Vector3(x, y));
                track.spline.SetTangentMode(index, ShapeTangentMode.Continuous);
            }
            distanceFromLastMP = points[i_].x - MilepostPosition;
            if (distanceFromLastMP >= 1609.344f || i_ == 0)
            {
              GameObject currentMilepost = Instantiate<GameObject>(milepost, track.transform);
              MilepostPosition = points[i_].x;
              currentMilepost.transform.position = new Vector3(x, y, 0);
              TMP_Text text = currentMilepost.GetComponentInChildren<TMP_Text>();
              text.text = (startMile + milepostNumber).ToString();
              milepostNumber += 1;
            }
            index++;
        }

        // Set the last 2 points of the Sprite Shape
        float finalX = GameManager.S.ConvertMetersToUnityMeters(points[i_-pointDenom].x);
        float startX = GameManager.S.ConvertMetersToUnityMeters(points[start].x);
        track.spline.InsertPointAt(index, new Vector3(finalX, -100));
        track.spline.InsertPointAt(index + 1, new Vector3(startX, -100));
        track.spline.SetTangentMode(index, ShapeTangentMode.Linear);
        track.spline.SetTangentMode(index + 1, ShapeTangentMode.Linear);

        ////smooth out the curve from point to point
        //for (int i = 2; i < 152; i++)
        //{
        //    track.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        //    track.spline.SetLeftTangent(i, new Vector3(-2, 0, 0));
        //    track.spline.SetRightTangent(i, new Vector3(2, 0, 0));

        //}

        // Save track spline as prefab
#if UNITY_EDITOR
        // Create the Prefab for the Train
        string currentTrackPrefabPath = prefabPath + "/Tracks/" + levelName + ".prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(track.gameObject, currentTrackPrefabPath, InteractionMode.UserAction);
#endif
    }
}
