using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBag : MonoBehaviour
{
    public GameObject SpawnFab;
    public float delay = 5f;
    bool ready = true;

    public SpriteRenderer glow;

    AudioClip scoopAudio;
    AudioSource audioSource; // The source of the sound in-game (usually attached to the object)

    private void Start()
    {
        glow.enabled = false;

        // Sets up the audio to be 3D
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0.5F;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        scoopAudio = Resources.Load<AudioClip>("scooping_seeds");
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
            audioSource.PlayOneShot(scoopAudio, 0.5F);
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
