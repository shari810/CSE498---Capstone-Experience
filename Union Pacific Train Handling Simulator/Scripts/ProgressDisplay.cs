using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ProgressDisplay : MonoBehaviour
{
    public Transform firstTrainCar;
    private VictoryChecker victoryChecker;

    [Tooltip("How often to ask for the progress")]
    public float pingingTime = 1f;
    private float timer = 0f;

    public float xScale = 0.7f;

    private Text text;

    private float startingDistance;

    [Tooltip("Set this to what the mile starts at instead of 0")]
    public float startingModifier = 0;
    // Start is called before the first frame update
    void Start()
    {
        firstTrainCar = LevelManager.S.firstTrainCar.transform;
        timer = pingingTime;
        text = GetComponent<Text>();
        victoryChecker = firstTrainCar.GetComponent<VictoryChecker>();
        startingDistance = victoryChecker.distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (victoryChecker != null)
        {
            if (timer >= pingingTime)
            {
                text.text = (Math.Round(((victoryChecker.distance-startingDistance) / 1609.344f)/xScale, 2) - startingModifier).ToString("0.00") + " mi";
                timer = 0f;
            }
            timer += Time.deltaTime;
        }
        else
        {
            Debug.LogWarning("No victory checker attached to front car.");
        }
    }
}
