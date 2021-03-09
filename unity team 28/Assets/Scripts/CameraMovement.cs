using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    //public Transform target;
    public float smoothTime = 0.3F;
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
        Vector3 targetPos = transform.TransformPoint(new Vector3(1, 1, 0));
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
