using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    //Speed the pet is currently moving at
    public float speed = 0f;
    //Pets max speed
    public float maxSpeed = 3f;
    //Pets minimum speed
    public float minSpeed = 1f;

    //The target the pet moves to
    public Transform target;

    //How far does the pet want to stand from its target? 
    public float standDist = 2f;
    /*How lenient are we with where the pet should be standing. 
     * Increase this number if the pet jitters when it reaches the target
     * Decrease this number if the pet stops abruptly at its target location.
     */
    public float acceptableVariance = 0.05f;
    //How far does the pet need to be to its target location before it slows down?
    public float distToSlow = 2f;
    //What is the animation curve it uses to accelerate.
    public AnimationCurve accelCurve;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DebugMove", 2f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Finds a point that is a specific distance away from the target 
    Vector2 TargetPoint(Vector2 point, float dist)
    {
        Vector2 _out;
        if (Vector2.Distance(transform.position, point) > dist)
        {
            _out = point - (Vector2)transform.position;
        }
        else
        {
            _out = (Vector2)transform.position - point;
        }
        return (Vector2)transform.position + _out;
    }

    void DebugMove()
    {

        float rn = Random.Range(0f, 1f);
        Debug.Log(rn);
        if (rn > 0.5f)
        {
            StartCoroutine(MoveWithin(target, standDist));
        }
        else
        {
            Vector2 randomPoint = GameManager.Instance.ClampInBounds(
                (Vector2)transform.position + (Random.insideUnitCircle * Random.Range(3f, 7f))
                );
            StartCoroutine(MoveTo(randomPoint));
        }
    }

    //Coroutine to move with a certain distance of a target
    IEnumerator MoveWithin(Transform target, float dist)
    {
        float speedRef = 0f;
        float distance = Vector2.Distance(TargetPoint(target.position, dist), transform.position);
        while (distance > acceptableVariance)
        {
            distance = Vector2.Distance(TargetPoint(target.position, dist), transform.position);

            if (distance > distToSlow)
            {
                speedRef = Mathf.MoveTowards(speedRef, 1, Time.deltaTime);
            }
            else speedRef = Mathf.MoveTowards(speedRef, 0, Time.deltaTime);

            speed = minSpeed + accelCurve.Evaluate(speedRef) * maxSpeed;

            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            yield return null;
        }

        Invoke("DebugMove", Random.Range(1f, 3f));
    }

    IEnumerator MoveTo(Vector2 point)
    {

        float speedRef = 0f;
        float distance = Vector2.Distance(point, transform.position);
        while (distance > acceptableVariance)
        {
            distance = Vector2.Distance(point, transform.position);

            if (distance > distToSlow || speed < minSpeed)
            {
                speedRef = Mathf.MoveTowards(speedRef, 1, Time.deltaTime);
            }
            else
            {
                speedRef = Mathf.MoveTowards(speedRef, 0f, Time.deltaTime);
            }

            speed = minSpeed + accelCurve.Evaluate(speedRef) * maxSpeed;

            transform.position = Vector2.MoveTowards(transform.position, point, speed * Time.deltaTime);
            yield return null;
        }
        Invoke("DebugMove", Random.Range(1f, 3f));
    }
}
