using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCarInfo : MonoBehaviour
{
    [SerializeField] private string carName = "";

    [SerializeField] private float lightWeight = -1f;
    [SerializeField] private float loadWeight = -1f;

    [SerializeField] private float length = -1f;

    [SerializeField] private int type = -1;

    [SerializeField] private int direction = 0;

    public void InjectData(string n, float light, float load, float len, int t, int d)
    {
        carName = n;
        lightWeight = light;
        loadWeight = load;
        length = len;
        type = t;
        direction = d;
    }

    /// <summary>
    /// Getter for name
    /// </summary>
    /// <returns>Name of this TrainCar</returns>
    public string GetCarName()
    {
        return carName;
    }

    /// <summary>
    /// Getter for lightWeight
    /// </summary>
    /// <returns>Light Weight of this TrainCar in kilograms</returns>
    public float GetLightWeight()
    {
        return lightWeight;
    }

    /// <summary>
    /// Getter for loadWeight
    /// </summary>
    /// <returns>Load Weight of this TrainCar in kilograms</returns>
    public float GetLoadWeight()
    {
        return loadWeight;
    }

    /// <summary>
    /// Getter for length
    /// </summary>
    /// <returns>Length of this TrainCar in meters</returns>
    public float GetLength()
    {
        return length;
    }


    /// <summary>
    /// Getter for type
    /// </summary>
    /// <returns>Type of this TrainCar</returns>
    public int GetTrainCarType()
    {
        return type;
    }

    /// <summary>
    /// Getter for direction
    /// </summary>
    /// <returns>Direction of this TrainCar</returns>
    public int GetDirection()
    {
        return direction;
    }
}
