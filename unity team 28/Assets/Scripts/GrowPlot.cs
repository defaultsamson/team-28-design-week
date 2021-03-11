using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowPlot : MonoBehaviour
{
    //The stats of the growplot. None means its empty.
    public GROWSTATE state = GROWSTATE.None;
    Plant plant;
    int GrowthStage;

    public GameObject plantSprite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //The function that triggers when a seed is planted.
    void PlantSeed(Plant plant)
    {
        state = GROWSTATE.Planted;
        this.plant = plant;
        GrowthStage = 0;
        Grow();
        // Play the seed plant sound
        GetComponent<AudioSource>().Play();
    }

    void Grow()
    {
        if(GrowthStage < plant.growthStages.Length)
        {
            GrowthStage++;
            plantSprite.GetComponent<SpriteRenderer>().sprite =
                plant.growthStages[GrowthStage - 1].sprite;
            StopAllCoroutines();
            if (GrowthStage < plant.growthStages.Length) StartCoroutine(Wait(plant.growthStages[GrowthStage - 1].timeLasts, Grow));
        }
        
    }

    private void OnMouseEnter()
    {
        if (plant == null) return;
        if(GrowthStage == plant.growthStages.Length)
        {
            plantSprite.GetComponent<SpriteRenderer>().sprite = plant.Glow;
        }
    }

    private void OnMouseExit()
    {
        if (plant == null) return;
        if (plantSprite == null) return;
        if(GrowthStage == plant.growthStages.Length) plantSprite.GetComponent<SpriteRenderer>().sprite =
                plant.growthStages[GrowthStage - 1].sprite;
    }

    private void OnMouseDown()
    {
        if (plant == null) return;
        if (GrowthStage == plant.growthStages.Length)
        {
            plantSprite.GetComponent<SpriteRenderer>().sprite = null;
            GameObject obj = Instantiate(plant.SpawnFab);
            obj.transform.position = transform.position;
            GrowthStage = 0;
            state = GROWSTATE.None;
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //First enusre this plot is not growing something already.
        if (state != GROWSTATE.None) return;
        //Then make sure the object is a seed
        Seed seed = collision.gameObject.GetComponent<Seed>();
        if (seed)
        {
            //Then get its shadow object property to make sure its on the ground.
            ShadowObject shadowObject = seed.gameObject.GetComponent<ShadowObject>();
            if(shadowObject.Elevation <= 0.1f)
            {
                //Only consume the seed if you are the first plot to do so.
                if (!seed.consumed)
                {
                    seed.consumed = true;
                    PlantSeed(seed.plant);
                    Destroy(collision.gameObject);
                }
            }
        }
    }
    

    IEnumerator Wait(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback.Invoke();
    }

}
public enum GROWSTATE
{
    None,
    Planted
}