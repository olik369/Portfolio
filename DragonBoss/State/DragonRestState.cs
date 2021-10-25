using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonRestState : IState<Dragon>
{
    float time;
    public void OnEnter(Dragon sender)
    {
        sender.moveAgent.Stop();
        sender.SetRestAnim();
        sender.CalDistToPlayer();
        time = sender.restWaitingTime;
    }

    public void OnUpdate(Dragon sender)
    {
        time -= Time.deltaTime;
        
        if (sender.distToPlayer < sender.traceDist * sender.traceDist)
        {
            sender.state = Dragon.State.Trace;
            sender.ChangeState();
        }

        if (time > 0f) return;

        sender.state = Dragon.State.Patrol;
        sender.ChangeState();
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {
        sender.StopCalDistToPlayer();
    }
}