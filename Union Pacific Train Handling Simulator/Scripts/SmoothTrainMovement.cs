using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Marker
{
    public Vector3 position;
    public Quaternion rotation;

    public Marker(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
}

public class SmoothTrainMovement : MonoBehaviour
{
    [Tooltip("Check the layer of the terrain and put it here.")]
    public int terrainLayer = 8;

    //[Tooltip("This will be determined by the physics engine.")]
    //public float movementSpeed = 1f;

    [Tooltip("How fast to snap to the correct rotation on the track.")]
    public float rotationSpeed = 10f;

    [Tooltip("How fast to snap the train car to the track.")]
    public float snapSpeed = 10f;

    //private Vector3 direction = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Use FixedUpdate for any physics-related things!
    void Update()
    {
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"));
        // If it hits something...
        if (hit.collider != null)
        {
            //Rotate sprite according to normal
            float angle = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotationSpeed);

            // Set position to hit point, smoothly :)
            transform.position = Vector2.Lerp(transform.position, hit.point + Vector2.up * transform.localScale.y / 2 * 1.01f, Time.deltaTime * snapSpeed);

            // Set movement direction
            //direction = (Quaternion.Euler(0, 0, -90) * hit.normal);

            // Debug some helpful vectors (point of contact and direction to move train, respectively)
            Color color = new Color(0, 0, 1.0f);
            Debug.DrawLine(hit.point, hit.point + 5*Vector2.up, color);
            Debug.DrawLine(hit.point, hit.point + 5 * (Vector2)(Quaternion.Euler(0,0,-90)*hit.normal), color);
        }
    }
}
