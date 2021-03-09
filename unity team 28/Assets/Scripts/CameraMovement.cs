using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private float threshold = 50F;
    public float smoothTime = 0.3F;
    public float speed = 5F;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //target = new Transform();
        //target.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementVec = Vector3.zero;

        // Right
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - threshold)
        {
            movementVec += new Vector3(speed, 0, 0);
        }
        // Left
        else if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= threshold)
        {
            movementVec += new Vector3(-speed, 0, 0);
        }
        
        // Up
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - threshold)
        {
            movementVec += new Vector3(0, speed, 0);
        }
        // Down
        else if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= threshold)
        {
            movementVec += new Vector3(0, -speed, 0);
        }

        Vector3 targetPos = transform.TransformPoint(movementVec);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
