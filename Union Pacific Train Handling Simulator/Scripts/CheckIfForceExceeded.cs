using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfForceExceeded : MonoBehaviour
{
    // Start is called before the first frame update
    public ThresholdDisplay thresholdDisplayBuff;
    public ThresholdDisplay thresholdDisplayDraft;

    public float bankOfTime = 10f;
    public bool exceeding = false;
    private float exceedTimer = 0f;
    private float exceedThreshold = 0.5f;

    private AudioSource warningSound;
    void Awake()
    {
        thresholdDisplayBuff = LevelManager.S.thresholdDisplayBuff;
        thresholdDisplayDraft = LevelManager.S.thresholdDisplayDraft;
    }

    private void Start()
    {
        warningSound = GameObject.Find("LevelEssentials/SoundBank/WARNINGS/ExcessiveForces").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        foreach (Transform car in transform)
        {
            FailCheck failCheck = car.GetComponent<FailCheck>();
            if (failCheck.forces > failCheck.forceThreshold)
            {
                exceeding = true;
                break;
            }
        }
        if (exceeding && !GameManager.GameisOver)
        {
            if (!warningSound.isPlaying && exceedTimer >= exceedThreshold)
            {
                warningSound.ignoreListenerPause = true;
                warningSound.Play();
                exceedTimer = 0f;
            }
            bankOfTime -= Time.deltaTime;
            // Kevin -- Display timer here using bankOfTime
            ThresholdTimer.DoWarningTimer(bankOfTime);
            //Debug.Log(bankOfTime);
            thresholdDisplayBuff.DoAbsThresh();
            thresholdDisplayDraft.DoAbsThresh();
            exceeding = false;
            exceedTimer += Time.deltaTime;
        }
        else
        {
            thresholdDisplayBuff.DoTimerThresh();
            thresholdDisplayDraft.DoTimerThresh();
            ThresholdTimer.DoNothing();
            exceedTimer = 0f;
        }
        if (bankOfTime <= 0 && !GameManager.GameisOver)
        {
            Debug.Log("FAILURE - FORCES TOO HIGH");
            GameManager.S.GameOver();
        }
    }
}
