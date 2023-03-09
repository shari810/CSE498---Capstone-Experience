using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;

public class TrainGapFiller : MonoBehaviour
{
    private SpriteShapeController filler;

    private SpriteShapeRenderer fillerRend;

    private List<Transform> relevantCars = new List<Transform>();

    private bool firstPass = true;

    public int relevantCarFrequency = 5;

    Transform consist;

    float forcesThreshold;

    float maxForces = 0;
    public bool doTransparency = false;

    // Start is called before the first frame update
    void Start()
    {
        filler = GetComponent<SpriteShapeController>();
        fillerRend = GetComponent<SpriteShapeRenderer>();

        consist = transform.GetChild(0);
        int i = 0;
        int childCount = consist.childCount;
        foreach (Transform car in consist.Cast<Transform>().OrderBy(t => t.GetSiblingIndex()))
        {
            if (i%relevantCarFrequency == 0 || car.GetSiblingIndex() == childCount-1)
                relevantCars.Add(car);
            ++i;
        }
        //Debug.Log(relevantCars.Count);
        //foreach (var j in includedIndices)
        //{
        //    Debug.Log(j);
        //}
    }

    void Update()
    {
        if (doTransparency)
        {
            forcesThreshold = consist.GetChild(0).GetComponent<ColorGradient>().forcesThreshold;
            maxForces = 0;
            foreach (Transform car in consist)
            {
                var forces = car.GetComponent<ColorGradient>().forces;
                if (forces > maxForces)
                {
                    maxForces = forces;
                }
            }
            fillerRend.color = new Color(1, 1, 1, (forcesThreshold - maxForces) / forcesThreshold);
        }
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for (int i = 0; i < relevantCars.Count; ++i)
        {
            if ((firstPass && i < 4) || !firstPass)
            {
                filler.spline.SetPosition(i, relevantCars[i].position + relevantCars[i].up * relevantCars[i].localScale.y / 2);  // get top middle of train car
            }
            else
            {
                filler.spline.InsertPointAt(i, relevantCars[i].position + relevantCars[i].up * relevantCars[i].localScale.y / 2);
            }
        }
        for (int i = 0; i < relevantCars.Count; ++i)
        {
            if ((firstPass && relevantCars.Count+i < 4) || !firstPass)
            {
                filler.spline.SetPosition(relevantCars.Count + i, relevantCars[relevantCars.Count - 1 - i].position - relevantCars[relevantCars.Count - 1 - i].up * relevantCars[relevantCars.Count - 1 - i].localScale.y / 2);  // get bottom middle of train car
            }
            else
            {
                filler.spline.InsertPointAt(relevantCars.Count + i, relevantCars[relevantCars.Count - 1 - i].position - relevantCars[relevantCars.Count - 1 - i].up * relevantCars[relevantCars.Count - 1 - i].localScale.y / 2);
            }
        }
        firstPass = false;
    }
}
