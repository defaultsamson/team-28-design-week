using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public NeedObject needObject;
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
        }
    }

    public void ChaseComplete()
    {
        //Do Need Need Task

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
    Kickin,
    Eating,
    Sleeping
}
