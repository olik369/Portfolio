using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonHardTraceState : IState<Dragon>
{
    public void OnEnter(Dragon sender)
    {
        sender.SetMoveAnim();
    }

    public void OnUpdate(Dragon sender)
    {
        sender.moveAgent.traceTarget = PlayerCtrl.Instance.tr.position;
        
        sender.CalDistOnUpdate();

        if (sender.distToPlayer <= sender.attackDist * sender.attackDist - 0.4f)
        {
            sender.state = Dragon.State.Idle;
            sender.ChangeState();
        }
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {

    }
}