using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttack1State : IState<Dragon>
{
    private readonly string curState = "Attack1";
    public void OnEnter(Dragon sender)
    {
        sender.DoGroundAttack(1);
        sender.moveAgent.Stop();
    }

    public void OnUpdate(Dragon sender)
    {
        if (!sender.CheckExitTime(0, sender.attackExitTime, curState)) { return; }

        sender.state = Dragon.State.Idle;
        sender.ChangeState();
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {

    }
}