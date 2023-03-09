using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager S;

    public GameObject throttleControlCanvas;

    public ThresholdDisplay thresholdDisplayBuff;

    public ThresholdDisplay thresholdDisplayDraft;

    public ThrottleControl throttleControl;

    [HideInInspector] public GameObject train;

    public GameObject firstTrainCar;

    public GameObject terrain;

    private void Awake()
    {
        S = this;

        train = firstTrainCar.transform.parent.gameObject;
    }
}
