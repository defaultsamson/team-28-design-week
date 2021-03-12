using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrowPlot : MonoBehaviour
{
    //The stats of the growplot. None means its empty.
    public GROWSTATE state = GROWSTATE.None;
    Plant plant;
    int GrowthStage;

    public GameObject plantSprite;


    AudioClip plantingAudio; // When planting the seed
    AudioClip[] growingAudios; // When the plant grows
    AudioClip[] choppingAudios; // When the plant gets chopped
    AudioSource audioSource; // The source of the sound in-game (usually attached to the object)

    // Start is called before the first frame update
    void Start()
    {
        // Sets up the audio to be 3D
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0F;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        plantingAudio = Resources.Load<AudioClip>("planting");
        growingAudios = new AudioClip[3];
        growingAudios[0] = Resources.Load<AudioClip>("growing_1");
        growingAudios[1] = Resources.Load<AudioClip>("growing_2");
        growingAudios[2] = Resources.Load<AudioClip>("growing_3");
        choppingAudios = new AudioClip[3];
        choppingAudios[0] = Resources.Load<AudioClip>("chopping_1");
        choppingAudios[1] = Resources.Load<AudioClip>("chopping_2");
        choppingAudios[2] = Resources.Load<AudioClip>("chopping_3");
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
        audioSource.PlayOneShot(plantingAudio, 0.4F);
    }

    void Grow()
    {
        if(GrowthStage < plant.growthStages.Length)
        {
            // Only play audio if it's not the first time growing
            if (GrowthStage > 0)
            {
                // Select a random growing audio
                int rand = Random.Range(0, growingAudios.Length - 1);
                audioSource.PlayOneShot(growingAudios[rand], 0.15F);
            }
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

            // Select a random chopping audio
            int rand = Random.Range(0, choppingAudios.Length - 1);
            audioSource.PlayOneShot(choppingAudios[rand], 0.4F);
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