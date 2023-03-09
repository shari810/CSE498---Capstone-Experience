using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoFences : MonoBehaviour
{
    ThrottleControl tc;
    // Start is called before the first frame update
    void Start()
    {
        tc = FindObjectOfType<ThrottleControl>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.SetActive(tc.locomotiveGroups.Count > 1);
    }
}
