using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ToggleTablet : MonoBehaviour
{
    private bool goDown = false;
    private bool goUp = false;
    private float originalHeight;
    private RectTransform rectTransform;
    [SerializeField] private float lerpTime = 0.25f;
    private float lerper = 0f;

    //[SerializeField] private CinemachineVirtualCamera tabletCam;
    
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalHeight = Mathf.Abs(rectTransform.anchoredPosition.y);
        //Debug.Log(originalHeight);
    }

    // Update is called once per frame
    void Update()
    {
        if (goUp)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(-originalHeight, originalHeight, lerper / lerpTime));
            if (lerper >= lerpTime)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, originalHeight);
                lerper = 0f;
                goUp = false;
            }
            else
            {
                lerper += Time.unscaledDeltaTime;
            }
        }
        else if (goDown)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Lerp(originalHeight, -originalHeight, lerper / lerpTime));
            if (lerper >= lerpTime)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -originalHeight);
                lerper = 0f;
                goDown = false;
            }
            else
            {
                lerper += Time.unscaledDeltaTime;
            }
        }
        //if (rectTransform.anchoredPosition.y >= 0)
        //{
        //    tabletCam.m_Priority = 11;
        //}
        //else
        //{
        //    tabletCam.m_Priority = 9;
        //}
    }

    public void Toggle()
    {
        if (rectTransform.anchoredPosition.y < 0 && !goUp && !goDown)
        {
            goUp = true;
        }
        else if (rectTransform.anchoredPosition.y >= 0 && !goUp && !goDown)
        {
            goDown = true;
        }
    }
}
