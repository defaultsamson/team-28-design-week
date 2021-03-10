using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShadowObject))]
public class DragDrop : MonoBehaviour
{
    public GameObject ItemFab;
    bool dragging;
    ShadowObject shadowObject;
    SpriteRenderer sprite;
    public float dragElevation = 1.5f, elevationRate = 1f;

    public AudioClip liftingAudio; // When picking up an object
    public AudioClip[] landingAudios;
    AudioSource audioSource; // The source of the sound in-game (usually attached to the object)
    bool landed = true;

    public bool Dragging
    {
        get { return dragging; }
    }

    public float dragSpeed = 5f;
    private void Awake()
    {
        shadowObject = GetComponent<ShadowObject>();
        sprite = GetComponent<SpriteRenderer>();

        // Sets up the audio to be 3D
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0F;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }

    public void OnMouseDown()
    {
        Drag();
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
                int rand = Random.Range(0, landingAudios.Length);
                audioSource.PlayOneShot(landingAudios[rand], volume);
                landed = true;
            }
        }
        else
        {
            landed = false;
        }

    }


}
