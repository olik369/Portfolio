using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonMoveAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform dragonTr;
    private Transform playerTr;

    public int currentIdx { get; set; }
    public bool turnInPlace { get; set; } = false;
    private readonly float patrolSpeed = 3f;
    private readonly float traceSpeed = 8f;
    public float moveRandomPosSpeed;
    private float rotDamping = 3f;
    private float attackRotDamping = 10f;

    private bool _patrolling;
    public bool patrolling
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                MoveWayPoint();
            }
        }
    }

    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            if(Dragon.Instance.state == Dragon.State.MoveRandomPos)
            {
                agent.speed = moveRandomPosSpeed;
            }
            else
            {
                agent.speed = traceSpeed;
            }
            rotDamping = 7f;
            
            if (agent.isPathStale) return;
            
            agent.destination = _traceTarget;
            agent.isStopped = false;
        }
    }

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    void Start()
    {
        InitComponent();

        currentIdx = Random.Range(0, DragonMoveWayPointGroup.Instance.wayPoints.Count);

        AgentSetting();
    }

    void Update()
    {
        if(turnInPlace == true)
        {
            var rot = Quaternion.LookRotation(playerTr.position - dragonTr.position);
            dragonTr.rotation = Quaternion.Slerp(dragonTr.rotation, rot, attackRotDamping * Time.deltaTime);

            return;
        }

        if (agent.isStopped == false)
        {
            var rot = Quaternion.LookRotation(agent.desiredVelocity);
            dragonTr.rotation = Quaternion.Slerp(dragonTr.rotation, rot, rotDamping * Time.deltaTime);
        }

        if (patrolling == false) return;

        if (agent.remainingDistance <= 0.5f)
        {
            currentIdx = Random.Range(0, DragonMoveWayPointGroup.Instance.wayPoints.Count);

            MoveWayPoint();
        }
    }

    void InitComponent()
    {
        agent = this.GetComponent<NavMeshAgent>();
        dragonTr = Dragon.Instance.transform;
        playerTr = PlayerCtrl.Instance.transform;
    }

    void MoveWayPoint()
    {
        if (agent.isPathStale) return;

        var wayPoint = DragonMoveWayPointGroup.Instance.GetPoint(currentIdx);

        agent.destination = wayPoint.position;
        agent.isStopped = false;
    }

    void AgentSetting()
    {
        agent.autoBraking = false;
        agent.updateRotation = false;
        agent.speed = patrolSpeed;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    public float altitude
    {
        get { return agent.baseOffset; }
    }

    public void Fly(float speed)
    {
        agent.baseOffset += speed * Time.deltaTime;
    }
}
