using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ThresholdDisplay : MonoBehaviour
{
    public Transform firstTrainCar;
    private float forceThreshold;
    private float absForceThreshold;
    private const float NewtonsToKilopounds = 0.0002248089f;

    private Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        firstTrainCar = LevelManager.S.firstTrainCar.transform;
        text = GetComponent<Text>();
        forceThreshold = Mathf.RoundToInt(firstTrainCar.GetComponent<FailCheck>().forceThreshold * NewtonsToKilopounds);
        absForceThreshold = Mathf.RoundToInt(firstTrainCar.GetComponent<FailCheck>().absForceThreshold * NewtonsToKilopounds);
        DoTimerThresh();
    }

    public void DoAbsThresh()
    {
        text.text = absForceThreshold.ToString();
        text.color = Color.black;
        text.gameObject.GetComponent<Outline>().effectColor = Color.yellow;
    }

    public void DoTimerThresh()
    {
        text.text = forceThreshold.ToString();
        text.color = Color.white;
        text.gameObject.GetComponent<Outline>().effectColor = Color.black;
    }
}
