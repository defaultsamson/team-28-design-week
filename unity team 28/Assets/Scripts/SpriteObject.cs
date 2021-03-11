using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteObject : MonoBehaviour
{
    static int below = -5, above = 20;
    int orderInLayer = 0;
    public int OrderInLayer {
        get { return orderInLayer; }
        set { orderInLayer = value;
            UpdateSprite();
        }
        }
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.Instance.OnPetMove += UpdateSprite;
    }

    public void UpdateSprite(float petY)
    {
        if(transform.position.y > petY)
        {
            spriteRenderer.sortingOrder = below + OrderInLayer;
        }
        else
        {
            spriteRenderer.sortingOrder = above + OrderInLayer;
        }
    }

    public void UpdateSprite()
    {
        UpdateSprite(GameManager.Instance.pet.transform.position.y);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnPetMove -= UpdateSprite;
    }
}
