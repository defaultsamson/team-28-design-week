using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public float speed = 2f;
    public Transform food;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveTo(food.position);
    }


    void MoveTo(Vector2 point)
    {
        float move;
        if (transform.position.x > point.x) move = -1f; else move = 1f;
        if (Vector2.Distance(point, transform.position) < 1.1f) move = 0f;

        transform.position = new Vector3(transform.position.x + move * Time.deltaTime * speed, transform.position.y);
    }
}
