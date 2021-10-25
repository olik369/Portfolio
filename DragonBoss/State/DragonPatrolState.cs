using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonPatrolState : IState<Dragon>
{
    int currentIdx;
    public void OnEnter(Dragon sender)
    {
        sender.moveAgent.patrolling = true;
        sender.SetMoveAnim();
        sender.CalDistToPlayer();
        currentIdx = sender.moveAgent.currentIdx;
    }

    public void OnUpdate(Dragon sender)
    {
        var nextIdx = sender.moveAgent.currentIdx;

        if(currentIdx != nextIdx)
        {
            sender.state = Dragon.State.Rest;
            sender.ChangeState();
        }

        if (sender.distToPlayer < sender.traceDist * sender.traceDist)
        {
            sender.state = Dragon.State.Trace;
            sender.ChangeState();
        }
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {
        sender.StopCalDistToPlayer();
    }
}
