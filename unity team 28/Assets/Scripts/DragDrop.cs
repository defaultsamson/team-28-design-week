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

    public void OnMouseDown()
    {
        Drag();
    }

    private void OnMouseUp()
    {
        Drop();
    }


    public void Drag(bool elevated = false)
    {
        dragging = true;
        sprite.sortingOrder++;
        shadowObject.GravityEnabled = false;
        if (elevated) shadowObject.Elevate(dragElevation, false);
    }

    public void Drop()
    {
        if (!dragging) return;
        dragging = false;
        sprite.sortingOrder--;
        shadowObject.GravityEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector2 mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            shadowObject.velocity = mouseDir * dragSpeed;
            if(shadowObject.Elevation < dragElevation)
            {
                shadowObject.Elevate(elevationRate * Time.deltaTime, false);
            }

            if (Input.GetMouseButtonUp(0)) Drop();
        }

    }


}
