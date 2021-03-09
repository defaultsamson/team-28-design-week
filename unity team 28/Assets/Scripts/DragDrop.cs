using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShadowObject))]
public class DragDrop : MonoBehaviour
{
    bool dragging;
    ShadowObject shadowObject;
    SpriteRenderer sprite;
    public float dragElevation = 1.5f, elevationRate = 1f;

    public bool Dragging
    {
        get { return dragging; }
    }

    public float dragSpeed = 5f;
    private void Awake()
    {
        shadowObject = GetComponent<ShadowObject>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        dragging = true;
        sprite.sortingOrder++;
    }

    private void OnMouseUp()
    {
        dragging = false;
        sprite.sortingOrder--;
    }


    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector2 mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            shadowObject.GravityEnabled = false;
            shadowObject.velocity = mouseDir * dragSpeed;

            if(shadowObject.Elevation < dragElevation)
            {
                shadowObject.Elevate(elevationRate * Time.deltaTime, false);
            }
        }
        else
        {
            shadowObject.GravityEnabled = true;
        }
        

    }


}
