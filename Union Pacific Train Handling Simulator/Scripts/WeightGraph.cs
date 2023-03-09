using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WeightGraph : MonoBehaviour
{
    [SerializeField]
    private Sprite dotSprite;
    private RectTransform graphContainer;
    private float sizeDelta;
    private RectTransform weightTemplate;
    private RectTransform dashTemplate;

    private const float kgToTons = 0.00110231f;

    [SerializeField]
    private Transform train;

    private void Awake()
    {
        train = LevelManager.S.train.transform;

        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();

        weightTemplate = graphContainer.Find("WeightLabelTemplate").GetComponent<RectTransform>();
        dashTemplate = graphContainer.Find("DashTemplate").GetComponent<RectTransform>();

        List<float> valueList = new List<float>();

        int index = 0;
        foreach (Transform car in train.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()))
        {
            TrainCarInfo info = car.GetComponent<TrainCarInfo>();
            valueList.Add(info.GetLightWeight() + info.GetLoadWeight());
            ++index;
        }
        sizeDelta = Mathf.Min(Mathf.Max(graphContainer.sizeDelta.x / valueList.Count / 2, 1),2);
        valueList.Reverse();
        ShowGraph(valueList);
    }

    private GameObject CreateDot(Vector2 anchoredPosition)
    {
        GameObject dot = new GameObject("dot", typeof(Image));
        dot.GetComponent<Image>().color = Color.red;
        dot.transform.SetParent(graphContainer, false);
        dot.GetComponent<Image>().sprite = dotSprite;
        RectTransform rectTransform = dot.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(sizeDelta, sizeDelta);  
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return dot;
    }

    private void ShowGraph(List<float> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        //Debug.Log(graphHeight);
        float yMaximum = valueList.Max()*kgToTons;
        float yMinimum = 0 * kgToTons;
        float xSize = graphContainer.sizeDelta.x/(valueList.Count);

        //GameObject lastDot = null;
        for (int i = 0; i < valueList.Count; ++i)
        {
            float xPosition = i * xSize;
            float yPosition = (0.95f*((valueList[i] * kgToTons - yMinimum) / (yMaximum-yMinimum)) * graphHeight);
            CreateBar(new Vector2(xPosition + xSize/2,yPosition), xSize * 0.9f);
            //GameObject dotGameObject = CreateDot(new Vector2(xPosition, yPosition));
            //if (lastDot != null)
            //{
            //    CreateDotConnection(lastDot.GetComponent<RectTransform>().anchoredPosition, dotGameObject.GetComponent<RectTransform>().anchoredPosition);
            //}
            //lastDot = dotGameObject;
        }
        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; ++i)
        {
            RectTransform labelY = Instantiate(weightTemplate);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-4f, normalizedValue*graphHeight*0.95f);
            labelY.GetComponent<Text>().text = string.Format("{0:0.0}", normalizedValue * (yMaximum-yMinimum) + yMinimum);

            RectTransform dashY = Instantiate(dashTemplate);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(0f, normalizedValue * graphHeight * 0.95f);
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject line = new GameObject("dotConnection", typeof(Image));
        line.transform.SetParent(graphContainer, false);
        line.GetComponent<Image>().color = Color.red;
        RectTransform rectTransform = line.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, sizeDelta/2);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        int multiplier = dir.y > 0 ? 1 : -1; 
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg*multiplier*Mathf.Acos(Vector2.Dot(Vector2.right, dir)));
    }

    private GameObject CreateBar(Vector2 graphPosition, float barWidth)
    {
        GameObject dot = new GameObject("bar", typeof(Image));
        dot.GetComponent<Image>().color = Color.red;
        dot.transform.SetParent(graphContainer, false);
        RectTransform rectTransform = dot.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(graphPosition.x,0);
        rectTransform.sizeDelta = new Vector2(barWidth, graphPosition.y);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0.5f, 0);
        return dot;
    }
}
