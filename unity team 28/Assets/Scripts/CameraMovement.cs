using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // The amount of pixels from the edge of the screen to activate scrolling
    private float threshold = 50F;
    // The time it takes for the camera to arrive at its target (e.g. from moving to stopped)
    public float smoothTime = 0.3F;
    // The magnitude of the force applied to the camera by the mouse cursor
    public float speed = 5F;
    // The current velofity of the camera (updated by SmoothDamp function)
    private Vector3 velocity = Vector3.zero;

    //The camera script
    Camera cam;

    //camera dimensions
    float width, height;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        height = cam.orthographicSize * 2;
        width = height * cam.aspect;
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

        // Turn our movement vector into a target point
        Vector3 targetPos = transform.TransformPoint(movementVec);
        Vector2 pos = GameManager.Instance.ClampInBounds((Vector2)targetPos, new Vector2(width/2,height/2));
        targetPos = new Vector3(pos.x, pos.y, targetPos.z);
        // Then use SmoothDamp to ease the camera to that target
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
