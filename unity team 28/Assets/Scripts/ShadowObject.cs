using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowObject : MonoBehaviour
{
    //Shadow Prefab and object
    public GameObject shadowFab;
    [HideInInspector]
    public GameObject shadow;

    //The default distance between the shadow and the object
    public float shadowOffset = -0.2f;

    public IMPACTTYPE impactType = IMPACTTYPE.Bounce;
    public float weight = 1f;
    public float friction = 14f;

    //Gravity Variables. U
    public static float Grav = -9.8f;
    [HideInInspector]
    public bool GravityEnabled = true;
    [SerializeField]
    float elevation;
    public float Elevation
    {
        get { return elevation; }
    }

    bool grounded = true;

    public Vector2 velocity;
    [SerializeField]
    float elevationVelocity = 0f;
    public float ElevationVelocity
    {
        get { return elevationVelocity; }
        set { elevationVelocity = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        shadow = Instantiate(shadowFab);
    }

    // Update is called once per frame
    void Update()
    {
        //move the object based on its velocity
        transform.position += (Vector3)velocity * Time.deltaTime;

        if (elevation <= 0f) {
            //If we were not grounded but gravity is enabled, then we just collided
            if (!grounded && GravityEnabled)
            {
                HitGround();
            }
            else
            {
                //If the object is on the ground and has not stopped, slow it by using the friction variable
                velocity = Vector2.MoveTowards(velocity, Vector2.zero, friction * Time.deltaTime);
            }
        } else
        {
            //Only apply gravity if gravity is enabled, drag and drop stops this.
            if (GravityEnabled)
            {
                elevationVelocity += Grav * Time.deltaTime;
                Elevate(elevationVelocity * Time.deltaTime, elevationVelocity <= 0f);
            }
           
        }
        //The shadow should always be below the object
        shadow.transform.position = new Vector2(transform.position.x, transform.position.y + shadowOffset - elevation);
    }

    //Elevate moves the object up if shadowPivot is true, by compensating for the movement.
    //If false, the shadow simpley moves down.
    public void Elevate(float raise, bool shadowPivot = true)
    {
        grounded = false;
        elevation += raise;
        if (shadowPivot) {
            transform.position += Vector3.up * raise;
        }

    }

    public virtual void HitGround()
    {
        switch (impactType)
        {
            case IMPACTTYPE.Bounce:
                if (Mathf.Abs(elevationVelocity) < 0.6f)
                {
                    grounded = true;
                    elevationVelocity = 0f;
                    elevation = 0f;
                } else
                {
                    elevationVelocity = Mathf.Abs(elevationVelocity) / weight;
                    elevation = 0.01f;
                }
                break;
            case IMPACTTYPE.Slide:
                grounded = true;
                elevationVelocity = 0f;
                elevation = 0f;
                break;
            case IMPACTTYPE.Stop:
                grounded = true;
                velocity = Vector2.zero;
                elevationVelocity = 0f;
                elevation = 0f;
                break;
        }

    }

    public virtual void HitHorizontal()
    {
        switch (impactType)
        {
            case IMPACTTYPE.Bounce:
                velocity = new Vector2(velocity.x * -1, velocity.y);
                break;
            case IMPACTTYPE.Slide:
                velocity = new Vector2(velocity.x * -1, velocity.y);
                break;
            case IMPACTTYPE.Stop:
                velocity = new Vector2(0f, velocity.y);
                break;
        }
    }
    public virtual void HitVertical()
    {
        switch (impactType)
        {
            case IMPACTTYPE.Bounce:
                velocity = new Vector2(velocity.x, velocity.y * -1);
                break;
            case IMPACTTYPE.Slide:
                velocity = new Vector2(velocity.x, velocity.y * -1);
                break;
            case IMPACTTYPE.Stop:
                velocity = new Vector2(velocity.x, 0f);
                break;
        }
    }

    public void Launch(Vector2 force, float elevationForce)
    {
        velocity = force;
        grounded = false;
        if (elevation <= 0f) elevation = 0.1f;
        elevationVelocity = elevationForce;
    }

    private void OnDestroy()
    {
        Destroy(shadow);
    }
}

public enum IMPACTTYPE
{
    Stop,
    Slide,
    Bounce
}