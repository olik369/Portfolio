using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack0_2State : IState<PlayerCtrl>
{
    private readonly string moveState = "Move";
    public void OnEnter(PlayerCtrl sender)
    {
        sender.rotatable = false;
    }

    public void OnUpdate(PlayerCtrl sender)
    {
        if (sender.animator.GetCurrentAnimatorStateInfo(0).IsName(moveState))
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
        sender.ResetTrigger(sender.hashCombo);
        sender.rotatable = true;
    }
}