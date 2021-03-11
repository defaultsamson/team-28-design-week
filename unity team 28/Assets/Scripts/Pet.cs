using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pet : MonoBehaviour
{
    public Animator animator;

    //Speed the pet is currently moving at
    public float speed = 0f;
    //Pets max speed
    public float maxSpeed = 3f;
    //Pets minimum speed
    public float minSpeed = 1f;

    //The target the pet moves to
    public Transform target;

    /*How lenient are we with where the pet should be standing. 
     * Increase this number if the pet jitters when it reaches the target
     * Decrease this number if the pet stops abruptly at its target location.
     */
    public float acceptableVariance = 0.05f;
    //How far does the pet need to be to its target location before it slows down?
    public float distToSlow = 2f;
    //What is the animation curve it uses to accelerate.
    public AnimationCurve accelCurve;

    public Coroutine coroutine;

    float scale;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        scale = transform.localScale.x;
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


    public void Chase(NeedObject needObject, Action callBack)
    {
        if(coroutine!=null)StopCoroutine(coroutine);
        coroutine = StartCoroutine(MoveWithin(needObject, callBack));
    }

    public void Wander()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        Vector2 randomPoint = GameManager.Instance.ClampInBounds(
                (Vector2)transform.position + (Random.insideUnitCircle * Random.Range(3f, 7f))
                );
        StartCoroutine(MoveTo(randomPoint));
    }

    //Coroutine to move with a certain distance of a target
    IEnumerator MoveWithin(NeedObject target, Action callBack)
    {
        float speedRef = 0f;
        float distance = Vector2.Distance(target.ClosestPos((Vector2)transform.position).position, transform.position);
        while (distance > acceptableVariance)
        {
            distance = Vector2.Distance(target.ClosestPos((Vector2)transform.position).position, transform.position);

            if (distance > distToSlow)
            {
                speedRef = Mathf.MoveTowards(speedRef, 1, Time.deltaTime);
            }
            else speedRef = Mathf.MoveTowards(speedRef, 0, Time.deltaTime);

            speed = minSpeed + accelCurve.Evaluate(speedRef) * maxSpeed;
            if (target.gameObject.transform.position.x > transform.position.x)
                transform.localScale = new Vector3(-scale, scale, 1f);
            else
                transform.localScale = new Vector3(scale, scale, 1f);
            transform.position = Vector2.MoveTowards(transform.position, target.ClosestPos((Vector2)transform.position).position, speed * Time.deltaTime);


            yield return null;
        }
        callBack.Invoke();
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
    }

    public void ResetAnimations()
    {
        animator.SetBool("Wander", false);
        animator.SetBool("WanderUnsatisfied", false);
        animator.SetBool("Eating", false);
        animator.SetBool("Sleep", false);
    }
    public void ResetAnimations(string anim)
    {
        ResetAnimations();
        animator.SetBool(anim, true);
    }
}
