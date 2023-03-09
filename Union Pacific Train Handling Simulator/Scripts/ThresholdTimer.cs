using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThresholdTimer : MonoBehaviour
{
    // Start is called before the first frame update
    
    private static Text timerText;
    public static float roundSeconds = 0f;


    void Start()
    {
        timerText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public static void DoWarningTimer(float roundSecond)
    {
        roundSeconds = Mathf.Round(roundSecond * 10) / 10f;
        timerText.color = new Color(1, 0, 0, 1f);
        timerText.text = $"WARNING: \nMAXIMUM THRESHOLD EXCEEDED \nCATASTROPHIC FAILURE IN: \n{roundSeconds:F1}";
    }

    public static void DoNothing()
    {
        timerText.text = "";
    }
}
