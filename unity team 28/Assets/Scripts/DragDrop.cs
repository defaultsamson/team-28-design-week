using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    bool dragging;
    public bool Dragging
    {
        get { return dragging; }
    }
    Rigidbody2D rigid;
    public float dragSpeed = 5f;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector2 mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            rigid.velocity = mouseDir * dragSpeed;
        }


    }


}
