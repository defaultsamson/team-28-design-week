using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShadowObject))]
public class DragDrop : MonoBehaviour
{
    public GameObject ItemFab;
    bool dragging;
    ShadowObject shadowObject;
    NeedObject needObject;
    SpriteRenderer sprite;
    public float dragElevation = 1.5f, elevationRate = 1f;

    AudioClip liftingAudio; // When picking up an object
    AudioClip[] landingAudios; // When hitting the ground
    AudioSource audioSource; // The source of the sound in-game (usually attached to the object)
    bool landed = true;

    bool locked = false;
    public bool Locked
    {
        get { return locked; }
        set
        {
            if (value)
            {
                Drop();
                if (shadowObject)
                {
                    shadowObject.velocity = Vector2.zero;
                }
            }
            locked = value;
        }
    }

    public bool Dragging
    {
        get { return dragging; }
    }

    public float dragSpeed = 5f;
    private void Awake()
    {
        shadowObject = GetComponent<ShadowObject>();
        sprite = GetComponent<SpriteRenderer>();
        needObject = GetComponent<NeedObject>();

        // Sets up the audio to be 3D
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0F;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        // Loads the audio clips
        liftingAudio = Resources.Load<AudioClip>("bloop");
        landingAudios = new AudioClip[3];
        landingAudios[0] = Resources.Load<AudioClip>("grass_1");
        landingAudios[1] = Resources.Load<AudioClip>("grass_2");
        landingAudios[2] = Resources.Load<AudioClip>("grass_3");
    }

    public void OnMouseDown()
    {
        if(!locked) Drag();
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

        audioSource.PlayOneShot(liftingAudio, 0.3F);
    }

    public void Drop()
    {
        if (!dragging) return;
        dragging = false;
        sprite.sortingOrder--;
        shadowObject.GravityEnabled = true;
        if(needObject) GameManager.Instance.Introduce(needObject);
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

        // Since the collision event happens in ShadowObject, try to track collisions here
        if (shadowObject.Elevation < 0.03)
        {
            if (!landed)
            {
                // Play the sound at different volumes depending on the impact speed
                float volume = Mathf.Clamp(Mathf.Abs(shadowObject.ElevationVelocity / 7F), 0.0F, 0.4F);
                // Select a random landing audio
<<<<<<< Updated upstream
                int rand = Random.Range(0, landingAudios.Length);
                audioSource.PlayOneShot(landingAudios[rand], volume);
                landed = true;
=======
                if (landingAudios.Length > 0)
                {
                    int rand = Random.Range(0, landingAudios.Length - 1);
                    audioSource.PlayOneShot(landingAudios[rand], volume);
                    landed = true;
                }
>>>>>>> Stashed changes
            }
        }
        else
        {
            landed = false;
        }

    }


}
