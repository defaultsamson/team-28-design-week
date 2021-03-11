using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBag : MonoBehaviour
{
    public GameObject SpawnFab;
    public float delay = 5f;
    bool ready = true;

    public SpriteRenderer glow;

    private void Start()
    {
        glow.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (ready) glow.enabled = true;
        else glow.enabled = false;
    }

    private void OnMouseExit()
    {
         glow.enabled = false;
    }

    void OnMouseDown()
    {
        if (ready)
        {
            glow.enabled = true;
            GameObject obj = Instantiate(SpawnFab);
            obj.transform.position = GameManager.Instance.ClampInBounds((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector2(4, 4));
            obj.GetComponent<DragDrop>().Drag(true);
            StartCoroutine(Wait(delay));
        }
    }

    IEnumerator Wait(float time)
    {
        ready = false;
        yield return new WaitForSeconds(time);
        ready = true;
    }
}
