using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Item> items = new List<Item>();

    bool active = false;
    public bool Acitve { get { return active; } }
    public GameObject graphics;
    public Vector2 activePos, dormantPos;
    public Vector2 mouseRange;
    public float speed = 7f;
    // Start is called before the first frame update
    void Start()
    {
        graphics.transform.localPosition = dormantPos;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.mousePosition.y <= mouseRange.y
            && Input.mousePosition.x <= Screen.width - mouseRange.x
            && Input.mousePosition.x >= mouseRange.x)
            active = true;
        else active = false;

        if (active)
        {
            graphics.transform.localPosition =
                Vector2.MoveTowards(graphics.transform.localPosition, activePos, speed * Time.deltaTime);
        }
        else
        {
            graphics.transform.localPosition =
                Vector2.MoveTowards(graphics.transform.localPosition, dormantPos, speed * Time.deltaTime);
        }
    }


}
