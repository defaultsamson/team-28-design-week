using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
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
}
