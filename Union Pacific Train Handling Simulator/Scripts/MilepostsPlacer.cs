using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilepostsPlacer : MonoBehaviour
{
  private GameObject firstTrain;
  private float scaleModifier = 0.695f / 165;
  public float height = 5f;

    // Start is called before the first frame update
    void Start()
    {
        firstTrain = GameObject.Find("GlueSpriteShape").transform.GetChild(0).GetChild(0).gameObject;
        transform.localScale = new Vector3(firstTrain.transform.localScale.y, firstTrain.transform.localScale.y, firstTrain.transform.localScale.y) * scaleModifier;

        // Cast a ray straight down
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"));


        transform.position = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
    }

}
