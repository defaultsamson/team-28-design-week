using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public enum WALLTYPE
    {
        Horizontal,
        Vertical
    }
    public WALLTYPE type = WALLTYPE.Horizontal;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ShadowObject shadowObject = collision.gameObject.GetComponent<ShadowObject>();
        if (shadowObject)
        {
            if (!shadowObject.GravityEnabled) DropIt(shadowObject.gameObject);
            if(type == WALLTYPE.Horizontal)
            {
                shadowObject.HitHorizontal();
            }
            else if(type == WALLTYPE.Vertical)
            {
                shadowObject.HitVertical();
            }
        }
    }
    //If the player is trying to shove objects through the map, make them drop it.
    void DropIt(GameObject drop)
    {
        DragDrop dropMe = drop.GetComponent<DragDrop>();
        if (dropMe) dropMe.Drop();
    }
}
