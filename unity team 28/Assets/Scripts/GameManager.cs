using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public NeedObject needObject;
    [Serializable]
    class AnimationSettings
    {
        public Vector2 IdleTime = Vector2.zero;
        public float SleepTime = 9.5f;
        public float EatTime = 3f;
        public float CelebrateTime = 1f;
       
    }
    [SerializeField]
    AnimationSettings animSettings;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
    }
    public Pet pet;
    public PETSTATE state = PETSTATE.None;

    AudioSource petAudio;
    AudioClip petAudioCelebrating;
    AudioClip petAudioUpset;
    AudioClip petAudioEating;
    AudioClip petAudioKick;
    AudioClip petAudioSleep;

    //The diffrent needs that will be turned to meters.
    public Need nutrition, energy, mood;
    //The health of the pet
    [Range(0f, 1f)]
    public float health;

    public HealthBar healthBar;

    //An array of the needs to make programming with all of them more simple and clean.
    Need[] needs;

    //The bounds of the world
    public Transform min, max;
    //A Vector 4 that will track it. x/y is the bottom left corner, z/w is the top right cornder.
    public Vector4 bounds;

    public event Action<float> OnPetMove;

    // Start is called before the first frame update
    void Start()
    {
        needs = new Need[] { nutrition, energy, mood };
        bounds = new Vector4(min.position.x, min.position.y, max.position.x, max.position.y);

        petAudio = pet.GetComponent<AudioSource>();

        petAudioCelebrating = Resources.Load<AudioClip>("monster_6");
        petAudioUpset = Resources.Load<AudioClip>("monster_4");
        petAudioEating = Resources.Load<AudioClip>("eating");
        petAudioKick = Resources.Load<AudioClip>("ball_1");
        petAudioSleep = Resources.Load<AudioClip>("monster_8");
    }

   void FixedUpdate()
    {
        int goodMoods = 0;
        foreach (Need need in needs)
        {
            need.DecayTick();
            if (need.Stat <= 0f)
            {
                health -= 0.05f * Time.fixedDeltaTime;
            }
            else if (need.Stat >= 0.8) goodMoods++;
        }

        if (goodMoods == 3) health += 0.05f * Time.fixedDeltaTime;
        health = Mathf.Clamp(health, 0f, 1f);
        if(health == 0f)
        {
            GetComponent<SceneLoader>().LoadScene("GameOver");
        }
        healthBar.SetHealth(health);
    }

    public void PetMove()
    {
        if(OnPetMove != null)
        {
            OnPetMove(pet.transform.position.y);
        }
    }

    //A clamp used to ensure the properties stay in bounds.
    public Vector2 ClampInBounds(Vector2 point)
    {
        float _outx = Mathf.Clamp(point.x, bounds.x, bounds.z),
            _outy = Mathf.Clamp(point.y, bounds.y, bounds.w);
        return new Vector2(_outx, _outy);
    }
    //Adds further constraints to the bounds of the world space. Currently used by the camera.
    public Vector2 ClampInBounds(Vector2 point, Vector2 notWithin)
    {
        float _outx = Mathf.Clamp(point.x, bounds.x + notWithin.x, bounds.z - notWithin.x),
            _outy = Mathf.Clamp(point.y, bounds.y + notWithin.y, bounds.w - notWithin.y);
        return new Vector2(_outx, _outy);
    }

    public void Introduce(NeedObject needObject)
    {
        if(state == PETSTATE.None||
            state == PETSTATE.Wandering ||
            state == PETSTATE.Chasing)
        {
            this.needObject = needObject;
            state = PETSTATE.Chasing;
            pet.Chase(this.needObject,ChaseComplete);
            pet.ResetAnimations("Wander");
        }
    }

    public void ChaseComplete()
    {
        //Do Need Need Task
        pet.ResetAnimations();

        if(needObject == null)
        {
            mood.Stat -= 0.1f;
            Upset();
            return;
        }
        state = needObject.toState;

        nutrition.Stat += needObject.nutritionGain;
        energy.Stat += needObject.energyGain;
        mood.Stat += needObject.moodGain;

        switch (state)
        {
            case PETSTATE.Eating:
                pet.animator.SetBool("Eating", true);
                pet.Wait(animSettings.EatTime, Celebrate);
                needObject.Lock();
                petAudio.PlayOneShot(petAudioEating, 0.4F);
                break;
            case PETSTATE.Sleeping:
                pet.animator.SetBool("Sleep", true);
                pet.Wait(animSettings.SleepTime, Celebrate);
                needObject.Lock();
                petAudio.PlayOneShot(petAudioSleep, 0.7F);
                break;
            case PETSTATE.Kicking:
                needObject.GetComponent<DragDrop>().Drop();
                Vector2 kickVel = Random.insideUnitCircle * Random.Range(12f, 15f);
                needObject.GetComponent<ShadowObject>().Launch(kickVel, 2f);
                petAudio.PlayOneShot(petAudioKick, 0.8F);
                if (Random.Range(0f, 1f) > mood.Stat)
                {
                    pet.Wait(0.5f, ChaseBall);
                }
                else
                {
                    Celebrate();
                }
                break;
        }
    }

    public void ChaseBall()
    {
        if(needObject != null)
        {
            state = PETSTATE.Chasing;
            pet.Chase(this.needObject, ChaseComplete);
            pet.ResetAnimations("Wander");
        }
        else
        {
            Stand();
        }
    }

    public void Celebrate()
    {
        state = PETSTATE.Celebrating;
        if(needObject != null) needObject.Unlock();
        pet.ResetAnimations();
        pet.animator.SetTrigger("Celebrate");
        pet.Wait(animSettings.CelebrateTime, Stand);
        petAudio.PlayOneShot(petAudioCelebrating, 1.0F);
    }

    public void Upset()
    {
        state = PETSTATE.Upset;
        pet.ResetAnimations();
        pet.animator.SetTrigger("Upset");
        pet.Wait(animSettings.CelebrateTime, Stand);
        petAudio.PlayOneShot(petAudioUpset, 1.0F);
    }

    public void Wander()
    {
        state = PETSTATE.Wandering;
        if(health < 0.5)
        {
            pet.ResetAnimations("WanderUnsatisfied");
            pet.Wander(Stand);
        }
        else
        {
            pet.ResetAnimations("Wander");
            pet.Wander(Stand);
        }
    }

    public void Stand()
    {
        state = PETSTATE.None;
        pet.ResetAnimations();
        pet.Wait(Random.Range(animSettings.IdleTime.x, animSettings.IdleTime.y), Wander);
    }

}

[Serializable]
public class Need
{
    [SerializeField]
    [Range(0f, 1f)]
    float stat = 0.8f;
    [Range(0f, 0.01f)]
    public float decayRate = 0.01f;

    public HealthBar bar;

    public float Stat 
    {
        get { return stat; }
        set { stat = Mathf.Clamp(value, 0f, 1f); }
    }

    public void DecayTick()
    {
        Stat -= decayRate * Time.fixedDeltaTime;
        bar.SetHealth(Stat);
    }

}

public enum PETSTATE
{
    None,
    Wandering,
    Chasing,
    Kicking,
    Eating,
    Sleeping,
    Celebrating,
    Upset
}
