using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test1 : MonoBehaviour
{
    public Transform _playerTrans;
    public float _speed = 2;
    public float _turnSpeed = 3;

    private NavMeshAgent _agent;
    private Vector3 _desVelocity;
    private CharacterController _charControl;

    void Start()
    {

        this._agent = this.gameObject.GetComponent<NavMeshAgent>();
        //this._charControl = this.gameObject.GetComponent<CharacterController>();

        this._agent.destination = this._playerTrans.position;

        return;
    }

    void Update()
    {

        Vector3 lookPos;
        Quaternion targetRot;

        this._agent.destination = this._playerTrans.position;
        this._desVelocity = this._agent.desiredVelocity;

        this._agent.updatePosition = false;
        this._agent.updateRotation = false;

        lookPos = this._playerTrans.position - this.transform.position;
        lookPos.y = 0;
        targetRot = Quaternion.LookRotation(lookPos);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * this._turnSpeed);

        //this._charControl.Move(this._desVelocity.normalized * this._speed * Time.deltaTime);

        //this._agent.velocity = this._charControl.velocity;

        return;
    }
}
