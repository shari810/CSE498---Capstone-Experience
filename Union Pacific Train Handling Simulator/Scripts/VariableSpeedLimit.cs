using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableSpeedLimit : MonoBehaviour
{
    public Transform firstCar;
    public Transform lastCar;
    public bool isRandom = false;
    public bool speedRestriction = false;
    public int restrictionStart;
    public int restrictionEnd;
    public int restrictionSpeedLimit;

    private Transform RestrictionZone;
    private VelocityDisplay vDisplay;
    private float defaultLimit;
    private bool firstCarInZone;
    private bool lastCarInZone;



    // Start is called before the first frame update
    void Start()
    {
        RestrictionZone = transform.Find("/LevelEssentials/RestrictionZone");
        vDisplay = transform.Find("/LevelEssentials/Canvas/Velocity").GetComponent<VelocityDisplay>();
        defaultLimit = vDisplay.velocityThresholdInMPH;

        if (isRandom)
        {
            System.Random rndm = new System.Random();
            speedRestriction = rndm.NextDouble() > 0.5 ? true : false;
        }

        if (speedRestriction)
        {
            RestrictionZone.GetComponent<Canvas>().enabled = true;

            Vector3 restrictionStartPosition = RestrictionZone.Find("restrictionLimit").position;
            Vector3 restrictionEndPosition = RestrictionZone.Find("defaultLimit").position;
            Quaternion restrictionStartRotation = RestrictionZone.Find("restrictionLimit").rotation;
            Quaternion restrictionEndRotation = RestrictionZone.Find("defaultLimit").rotation;

            restrictionStartPosition.x = restrictionStart;
            restrictionEndPosition.x = restrictionEnd;
            RestrictionZone.Find("restrictionLimit").SetPositionAndRotation(restrictionStartPosition, restrictionStartRotation);
            RestrictionZone.Find("defaultLimit").SetPositionAndRotation(restrictionEndPosition, restrictionEndRotation);

            RestrictionZone.Find("restrictionText").SetPositionAndRotation(restrictionStartPosition, restrictionStartRotation);
            RestrictionZone.Find("defaultText").SetPositionAndRotation(restrictionEndPosition, restrictionEndRotation);
            RestrictionZone.Find("restrictionText").GetComponent<Text>().text = restrictionSpeedLimit.ToString();
            RestrictionZone.Find("defaultText").GetComponent<Text>().text = defaultLimit.ToString();
        }
        else { RestrictionZone.GetComponent<Canvas>().enabled = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if (speedRestriction)
        {
            firstCarInZone = ((firstCar.position.x >= restrictionStart) && (firstCar.position.x <= restrictionEnd));
            lastCarInZone = ((lastCar.position.x >= restrictionStart) && (lastCar.position.x <= restrictionEnd));
            vDisplay.velocityThresholdInMPH = (firstCarInZone || lastCarInZone) ? restrictionSpeedLimit : defaultLimit;
        }
    }
}
