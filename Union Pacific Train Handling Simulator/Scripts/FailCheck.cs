using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FailCheck : MonoBehaviour
{
    // Force amount
    public float forces = 0f;
    public float forceThreshold = 300;
    public float absForceThreshold = 1000;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.GameisOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(forces) > absForceThreshold && transform.parent.GetComponent<ThrottleControl>().consist.FirstCar.GetVelocityMPS() > 1 && !GameManager.GameisOver)
        {
            Debug.Log("FAILURE - FORCES TOO HIGH");
            GameManager.S.GameOver("FORCES WAY TOO HIGH");
        }
    }
}
