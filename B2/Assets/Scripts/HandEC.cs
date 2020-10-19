using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandEC : MonoBehaviour
{
    private Animator anim;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnAnimatorIK()
    {
        var diff = transform.position - target.position;
        diff.y = 0;
        if(diff.magnitude < 1)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKPosition(AvatarIKGoal.RightHand, target.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);

        }else
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }
    }
}
