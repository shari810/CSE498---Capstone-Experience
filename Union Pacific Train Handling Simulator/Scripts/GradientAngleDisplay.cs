using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GradientAngleDisplay : MonoBehaviour
{
    public Transform firstTrainCar;
    [Tooltip("This should match what the terrain was scaled by on the y-axis")]
    public float yAmplification;

    [Tooltip("How often to ask for the gradient")]
    public float pingingTime = 1f;
    private float timer = 0f;

    private Text text;

    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        firstTrainCar = LevelManager.S.firstTrainCar.transform;
        timer = pingingTime;
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= pingingTime)
        {
            angle = firstTrainCar.eulerAngles.z;
            angle = (angle > 180) ? angle - 360 : angle;  // Get negative numbers over large angles
            angle = Mathf.Tan(Mathf.Deg2Rad * angle) * 100;
            text.text = Math.Round(angle / yAmplification, 2).ToString("0.00") + "%";
            timer = 0f;
        }
        timer += Time.deltaTime;
    }
}
