﻿using System;
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
    //The diffrent needs that will be turned to meters.
    public Need nutrition, energy, mood;
    //The health of the pet
    [Range(0f, 1f)]
    public float health;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

   void FixedUpdate()
    {
        foreach (Need need in needs) need.DecayTick();
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

        switch (state)
        {
            case PETSTATE.Eating:
                nutrition.Stat += 0.2f;
                pet.animator.SetBool("Eating", true);
                pet.Wait(animSettings.EatTime, Celebrate);
                needObject.Lock();
            break;
            case PETSTATE.Sleeping:
                energy.Stat += 0.3f;
                pet.animator.SetBool("Sleep", true);
                pet.Wait(animSettings.SleepTime, Celebrate);
                needObject.Lock();
            break;
            case PETSTATE.Kicking:
                mood.Stat += 0.1f;
                needObject.GetComponent<DragDrop>().Drop();
                Vector2 kickVel = Random.insideUnitCircle * Random.Range(12f, 15f);
                needObject.GetComponent<ShadowObject>().Launch(kickVel, 2f);
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
    }

    public void Upset()
    {
        state = PETSTATE.Celebrating;
        pet.ResetAnimations();
        pet.animator.SetTrigger("Upset");
        pet.Wait(animSettings.CelebrateTime, Stand);
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

    public float Stat 
    {
        get { return stat; }
        set { stat = Mathf.Clamp(value, 0f, 1f); }
    }

    public void DecayTick()
    {
        Stat -= decayRate * Time.fixedDeltaTime;
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
