using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadEC : MonoBehaviour
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
            anim.SetLookAtWeight(1);
            anim.SetLookAtPosition(target.position);
        } else
        {
            anim.SetLookAtWeight(0);
        }
    }
}
