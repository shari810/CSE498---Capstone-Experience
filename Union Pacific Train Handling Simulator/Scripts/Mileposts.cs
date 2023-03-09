using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Mileposts : MonoBehaviour
{
    public float xScale = 0.7f;
    private const float mileToMeters = 1609.344f;
    private float positionCheck = 0f;

    public GameObject milemarker;

    public VictoryChecker victorychecker;

    // Start is called before the first frame update
    void Start()
    {
      while(positionCheck <= victorychecker.endline)
      {
        Instantiate(milemarker, new Vector3(positionCheck, 1000000, 1), Quaternion.identity, null);
        positionCheck += mileToMeters * xScale;
      }
    }
}
