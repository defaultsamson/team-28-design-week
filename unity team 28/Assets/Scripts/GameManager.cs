using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(this);
    }

    public Need nutrition, energy, mood;
    [Range(0f, 1f)]
    public float health;

    Need[] needs;
    public Transform min, max;

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

    public Vector2 ClampInBounds(Vector2 point)
    {
        float _outx = Mathf.Clamp(point.x, bounds.x, bounds.z),
            _outy = Mathf.Clamp(point.y, bounds.y, bounds.w);
        return new Vector2(_outx, _outy);
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