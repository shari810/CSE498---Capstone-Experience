using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGradient : MonoBehaviour
{
    SpriteRenderer colorRend;

    // Color constants
    /*
    private Color32 RED = new Color32(255, 0, 0, 255);
    private Color32 BLUE = new Color32(0, 0, 255, 255);*/
    private Color32 WHITE = new Color32(255, 255, 255, 255);

    // Lerp values for use in linear interpolation between colors
    /*
    private float redLerpValue = 0f;
    private float blueLerpValue = 0f;
    */

    //Force amount
    public float forces = 0f;

    public float forcesThreshold = 1000000;

    /*
    public void InjectData(float forceAmount)
    {
      forces = forceAmount;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
      colorRend = GetComponent<SpriteRenderer>(); //getting sprite renderer component
      colorRend.color = WHITE; //initialize color to white
    }

    // Update is called once per frame
    void Update()
    {
        // If last child (last car), mimic car in front since last car doesn't have forces
        if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
            forces = transform.parent.GetChild(transform.parent.childCount - 2).GetComponent<ColorGradient>().forces;
        if (forces > forcesThreshold)
        {
            forces = forcesThreshold;
        }
        else if (forces < -forcesThreshold)
        {
            forces = -forcesThreshold;
        }

      if (forces > 0)
      {
        /*
        redLerpValue = Mathf.Lerp(WHITE.r, RED.r, (forcesThreshold - forces) / forcesThreshold);
        blueLerpValue = Mathf.Lerp(WHITE.b, RED.b, (forcesThreshold - forces) / forcesThreshold);
        */
        colorRend.color = new Color32(255, (byte)(255 * ((forcesThreshold - forces)) / forcesThreshold), (byte)(255 * ((forcesThreshold - forces) / forcesThreshold)), 255);
      }

      else if(forces <= 0)
      {
        /*
        redLerpValue = Mathf.Lerp(WHITE.r, BLUE.r, (forcesThreshold - Mathf.Abs(forces)) / forcesThreshold);
        blueLerpValue = Mathf.Lerp(WHITE.b, BLUE.b, (forcesThreshold - Mathf.Abs(forces)) / forcesThreshold);
        */
        colorRend.color = new Color32((byte)(255 * ((forcesThreshold - Mathf.Abs(forces))) / forcesThreshold), (byte)(255 * ((forcesThreshold - Mathf.Abs(forces))) / forcesThreshold), 255, 255);
      }

      /*
      colorRend.color = new Color32((byte)redLerpValue, 0, (byte)blueLerpValue, 255);*/
    }
}
