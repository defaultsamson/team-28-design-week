using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowPlot : MonoBehaviour
{
    public GROWSTATE state = GROWSTATE.None;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlantSeed()
    {
        state = GROWSTATE.Planted;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (state != GROWSTATE.None) return;
        Seed seed = collision.gameObject.GetComponent<Seed>();
        if (seed)
        {
            ShadowObject shadowObject = seed.gameObject.GetComponent<ShadowObject>();
            if(shadowObject.Elevation <= 0f)
            {
                if (!seed.consumed)
                {
                    seed.consumed = true;
                    PlantSeed();
                    Destroy(collision.gameObject);
                }
            }
        }
    }

}
public enum GROWSTATE
{
    None,
    Planted
}