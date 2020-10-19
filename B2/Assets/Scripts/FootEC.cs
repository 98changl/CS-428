using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootEC : MonoBehaviour
{
    private Animator anim;
    public LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnAnimatorIK()
    {
anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        RaycastHit hit;
        Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot)+Vector3.up, Vector3.down);
        if(Physics.Raycast(ray, out hit, 1, layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += 1;
            anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }

        ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot)+Vector3.up, Vector3.down);
        if(Physics.Raycast(ray, out hit, 1, layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += 1;
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }
    }
}
