using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceVisualIndicator : MonoBehaviour
{
    public GameObject[] fences;

    // Start is called before the first frame update
    void Start()
    {
        if (fences.Length > 3)
        {
            Debug.LogError("You have more than 3 fences");
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < fences.Length; i++)
        {
            if ((MoveFence.fenceValue * -1 + 1)== i)
            {
                fences[i].SetActive(true);
            }
            else
            {
                fences[i].SetActive(false);
            }
        }
    }
}
