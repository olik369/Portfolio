using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTraceState : IState<Dragon>
{
    private int attackType;
    public void OnEnter(Dragon sender)
    {
        sender.SetMoveAnim();
        attackType = Random.Range(1, 3);
    }

    public void OnUpdate(Dragon sender)
    {
        sender.CalDistOnUpdate();
        sender.moveAgent.traceTarget = PlayerCtrl.Instance.tr.position;

        if (sender.distToPlayer < sender.attackDist * sender.attackDist)
        {
            if (attackType == 1)
            {
                sender.state = Dragon.State.Attack1;
                sender.ChangeState();
            }
            else if (attackType == 2)
            {
                sender.state = Dragon.State.Attack2;
                sender.ChangeState();
            }
        }

        if (sender.distToPlayer > sender.traceDist * sender.traceDist)
        {
            sender.state = Dragon.State.Patrol;
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
