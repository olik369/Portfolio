using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState<PlayerCtrl>
{
    public void OnEnter(PlayerCtrl sender)
    {

    }

    public void OnUpdate(PlayerCtrl sender)
    {
        sender.SetFloatMove();

        if (Input.GetKeyDown(sender.Key_LightAttack))
        {
            sender.state = PlayerCtrl.State.Attack0_0;
            sender.ChangeState();
        }
        else if (Input.GetKeyDown(sender.Key_DashAttack))
        {
            sender.state = PlayerCtrl.State.Attack1;
            sender.ChangeState();
        }
        else if (Input.GetKeyDown(sender.Key_HeavyAttack))
        {
            sender.state = PlayerCtrl.State.Attack2;
            sender.ChangeState();
        }
        else if (Input.GetKeyDown(sender.Key_Dodge))
        {
            sender.state = PlayerCtrl.State.Dodge;
            sender.ChangeState();
        }
    }

    public void OnFixedUpdate(PlayerCtrl sender)
    {

    }

    public void OnExit(PlayerCtrl sender)
    {
        sender.animator.SetFloat(sender.hashMoveSpeed, 0f);
    }
}