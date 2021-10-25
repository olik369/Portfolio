using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFireBallState : IState<Dragon>
{
    private readonly string curState = "FireBall";
    public void OnEnter(Dragon sender)
    {
        sender.DoGroundAttack(3);
        sender.moveAgent.Stop();
        sender.moveAgent.turnInPlace = true;
    }

    public void OnUpdate(Dragon sender)
    {
        if (sender.CheckExitTime(0, 0.25f, curState) == true)
        {
            sender.fireProjectileReady = true;
        }

        if (sender.CheckExitTime(0, 0.8f, curState) == false)
        {
            return;
        }

        sender.state = Dragon.State.Empty;
        sender.ChangeState();
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {
        sender.moveAgent.turnInPlace = false;
        sender.fireProjectileReady = false;
    }
}