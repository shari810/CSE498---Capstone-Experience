using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    public ThrottleControl throttleControl;

    [Tooltip("The background will parallax at the speed of the train times this number.")]
    public float parallaxMultiplier = 1f;

    [Tooltip("Distance along the x axis to travel before looping back to the original position.")]
    public float parallaxLoopDistance = 800f;

    private float originalX;

    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        throttleControl = LevelManager.S.throttleControl;
        rectTransform = GetComponent<RectTransform>();
        originalX = rectTransform.offsetMin.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x - (float)throttleControl.consist.FirstCar.GetVelocityMPS() * parallaxMultiplier * Time.deltaTime, rectTransform.offsetMin.y);
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMin.x, rectTransform.offsetMax.y);
        if (originalX - rectTransform.offsetMin.x > parallaxLoopDistance)
        {
            rectTransform.offsetMin = new Vector2(originalX, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(originalX, rectTransform.offsetMax.y);
        }
    }
}
