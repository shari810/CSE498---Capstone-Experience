using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveFence : MonoBehaviour
{
    [SerializeField] Button fenceButton;

    // 1 = Right Fence
    // -1 = Left Fence
   public static int fenceValue = 0; 

    // Start is called before the first frame update
    void Start()
    {
        fenceButton.onClick.AddListener(Move);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move()
    {
        // Set Fence Value = fenceValue
        // Need Jeff to Send Us Code for Fences
        // Debug.Log("Fence = " + fenceValue);
    }

    public void DecreaseFenceValue()
    {
        if (fenceValue > -1)
            fenceValue--;
        Move();
    }

    public void IncreaseFenceValue()
    {
        if (fenceValue < 1)
            fenceValue++;
        Move();
    }
}
