﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public GameObject ObjectFab;
    public GameObject InventorySlot;
    bool dragging;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging)
        {
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
            if (!Inventory.Instance.Acitve)
            {
                GameObject obj = Instantiate(ObjectFab);
                obj.transform.position = mousePos;
                obj.GetComponent<DragDrop>().Drag(true);
                dragging = false;
            }
        }
        else
        {
            transform.position = InventorySlot.transform.position;
        }
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void OnMouseUp()
    {
        dragging = false;
    }
}
