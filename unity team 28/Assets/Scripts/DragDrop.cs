using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShadowObject))]
public class DragDrop : MonoBehaviour
{
    bool dragging;
    ShadowObject shadowObject;
    SpriteRenderer sprite;
    public float dragElevation = 1.5f, elevationRate = 1f;

    public AudioClip landingAudio; // When the object hits the ground
    public AudioClip liftingAudio; // When picking up an object
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

    private void OnMouseDown()
    {
        dragging = true;
        sprite.sortingOrder++;
        shadowObject.GravityEnabled = false;

        audioSource.PlayOneShot(liftingAudio, 0.3F);
    }

    private void OnMouseUp()
    {
        Drop();
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
        } 
        
        // Since the collision event happens in ShadowObject, try to track collisions here
        if (shadowObject.Elevation < 0.03)
        {
            if (!landed)
            {
                // Play the sound at different volumes depending on the impact speed
                float volume = Mathf.Clamp(Mathf.Abs(shadowObject.ElevationVelocity / 4F), 0.0F, 0.25F);
                audioSource.PlayOneShot(landingAudio, volume);
                landed = true;
            }
        }
        else
        {
            landed = false;
        }

    }


}
