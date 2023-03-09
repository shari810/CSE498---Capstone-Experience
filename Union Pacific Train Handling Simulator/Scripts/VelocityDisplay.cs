using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSimulation;
using System;

public class VelocityDisplay : MonoBehaviour
{
    public Transform firstTrainCar;
    private Text velocityText;
    private Text limitText;
    private double velocity;
    public float velocityThresholdInMPH = 49;

    // Start is called before the first frame update
    void Start()
    {
        firstTrainCar = LevelManager.S.firstTrainCar.transform;
        velocityText = transform.Find("velocityText").GetComponent<Text>();
        limitText = transform.Find("limitText").GetComponent<Text>();


    }

    // Update is called once per frame
    void Update()
    {
        velocity = firstTrainCar.parent.GetComponent<ThrottleControl>().consist.FirstCar.GetVelocityMPS();
        
        if (velocity < 0)
        {
            velocity = 0.00;
        }

        velocityText.text = Math.Round(MPStoMPH(velocity), 2).ToString("0.00") + " mph";
        if (Math.Round(MPStoMPH(velocity), 2) > velocityThresholdInMPH && !GameManager.GameisOver)
        {
            Debug.Log("FAILURE - SPEED LIMIT EXCEEDED");
            GameManager.S.GameOver("SPEED LIMIT EXCEEDED");
        }

        limitText.text = velocityThresholdInMPH.ToString();
    }

    double MPStoMPH(double velocity)
    {
       return velocity * 2.23694;
    }
}