using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainInfo : MonoBehaviour
{
    [SerializeField] private string trainName = "";

    [SerializeField] private float totalLightWeight = -1f;
    [SerializeField] private float totalLoadWeight = -1f;

    [SerializeField] private int totalNumCars = -1;
    [SerializeField] private float totalLength = -1f;

    public void SetName(string n)
    {
        trainName = n;
    }

    public string GetName()
    {
        return trainName;
    }

    public void CalculateInfo()
    {
        // Init totals
        totalLightWeight = 0f;
        totalLoadWeight = 0f;
        totalNumCars = 0;
        totalLength = 0f;

        // Calculate totals
        TrainCarInfo[] trainCarsInfo = GetComponentsInChildren<TrainCarInfo>();
        foreach (TrainCarInfo carInfo in trainCarsInfo)
        {
            totalLightWeight += carInfo.GetLightWeight();
            totalLoadWeight += carInfo.GetLoadWeight();

            totalNumCars++;
            totalLength += carInfo.GetLength();
        }
    }
}
