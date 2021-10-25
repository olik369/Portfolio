using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack1State : IState<PlayerCtrl>
{
    readonly string curState = "Attack1";
    public void OnEnter(PlayerCtrl sender)
    {
        sender.DoAttack(PlayerCtrl.State.Attack1);
        sender.rotatable = false;
    }

    public void OnUpdate(PlayerCtrl sender)
    {
        if (sender.CheckExitTime(0, sender.atkExitTime, curState))
        {
            sender.state = PlayerCtrl.State.Move;
            sender.ChangeState();
        }
    }

    public void OnFixedUpdate(PlayerCtrl sender)
    {

    }

    public void OnExit(PlayerCtrl sender)
    {
        sender.ResetTrigger(sender.hashAttack);
        sender.rotatable = true;
    }
}