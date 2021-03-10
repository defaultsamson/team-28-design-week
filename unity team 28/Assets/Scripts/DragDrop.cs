﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShadowObject))]
public class DragDrop : MonoBehaviour
{
    public GameObject ItemFab;
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

            //drop in the inventory
            #region
            if (Inventory.Instance.Acitve && 
                ItemFab != null &&
                Inventory.Instance.items.Count < Inventory.Instance.itemsSlots.Length)
            {
                Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject obj = Instantiate(ItemFab);
                obj.transform.position = mousePos;
                Item item = obj.GetComponent<Item>();
                item.dragging = true;
                Inventory.Instance.Add(item);
                Destroy(gameObject);
            }
            #endregion
            if (Input.GetMouseButtonUp(0)) Drop();
        }

    }


}
