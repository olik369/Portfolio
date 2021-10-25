using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeState : IState<PlayerCtrl>
{
    private readonly string curState = "Dodge";
    private readonly string moveState = "Move";
    private bool dodgeAnimStart;
    public void OnEnter(PlayerCtrl sender)
    {
        sender.animator.SetTrigger(sender.hashDodge);
        dodgeAnimStart = false;
        sender.rotatable = false;
    }

    public void OnUpdate(PlayerCtrl sender)
    {
        if(dodgeAnimStart == false)
        {
            if (sender.animator.GetCurrentAnimatorStateInfo(0).IsName(curState))
            {
                dodgeAnimStart = true;
            }
            return;
        }
        
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
        sender.rotatable = true;
        sender.animator.SetFloat(sender.hashMoveSpeed, 0.2f);
    }
}