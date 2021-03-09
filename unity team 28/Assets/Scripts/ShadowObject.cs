using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowObject : MonoBehaviour
{
    public GameObject shadow;
    public float shadowOffset = -0.2f;
    public float weight = 1f;
    public static float Grav = -9.8f;
    public bool GravityEnabled = true;
    [SerializeField]
    float elevation;
    public float Elevation
    {
        get { return elevation; }
    }
    public float friction;

    public Vector2 velocity;
    [SerializeField]
    float elevationVelocity = 0f;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;

        if (elevation <= 0f) {
            velocity = Vector2.MoveTowards(velocity, Vector2.zero, friction * Time.deltaTime);
            elevationVelocity = Mathf.MoveTowards(elevationVelocity, 0f, 5f * Time.deltaTime);
        } else
        {
            if (GravityEnabled)
            {
                elevationVelocity += Grav * Time.deltaTime;
                Elevate(elevationVelocity * Time.deltaTime, elevationVelocity <= 0f);
            }
           
        }

        shadow.transform.position = new Vector2(transform.position.x, transform.position.y + shadowOffset - elevation);
    }



    public void Elevate(float raise, bool shadowPivot = true)
    {
        elevation += raise;
        if (shadowPivot) {
            transform.position += Vector3.up * raise;
        }

    }
}
