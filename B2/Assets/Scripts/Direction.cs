using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Direction : MonoBehaviour
{
    public Transform head = null;
    public Vector3 lookAtTarget;
    public float blendTime = 0.2f;

    private Vector3 lookAtPosition;
    private float lookAtWeight = 0.0f;
    private float speed = 1.0f;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        lookAtTarget = head.position + transform.forward;
        lookAtPosition = lookAtTarget;
    }

    void OnAnimatorIK()
    {
        lookAtTarget.y = head.position.y;

        float lookAtWeightTarget = 1f;
        
        Vector3 current = lookAtPosition - head.position;
        Vector3 target = lookAtTarget - head.position;

        // update rotate positions
        current = Vector3.RotateTowards(current, target, speed * Time.deltaTime, float.PositiveInfinity);
        lookAtPosition = head.position + current;
        
        // set look at for animator
        lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtWeightTarget, Time.deltaTime / blendTime);
        animator.SetLookAtWeight(lookAtWeight, 0.8f, 0.75f, 0.2f, 0f);
        animator.SetLookAtPosition(lookAtPosition);
    }
}