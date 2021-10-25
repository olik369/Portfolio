using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling.Multi;

public class PlayerUnderAttackState : IState<PlayerCtrl>
{
    private readonly string curStateTag = "UnderAttack";
    private readonly string moveState = "Move";
    private bool underAttackAnimStart;
    public void OnEnter(PlayerCtrl sender)
    {
        if(sender.underAttackDir == true)
        {
            //정면에서 맞을때
            sender.animator.SetInteger(sender.hashDamageDirection, 0);
            sender.animator.SetTrigger(sender.hashDamage);
        }
        else
        {
            //후방에서 맞을때
            sender.animator.SetInteger(sender.hashDamageDirection, 1);
            sender.animator.SetTrigger(sender.hashDamage);
        }
        underAttackAnimStart = false;
        sender.rotatable = false;
        Physics.IgnoreLayerCollision(3, 7, true);
    }

    public void OnUpdate(PlayerCtrl sender)
    {
        if(underAttackAnimStart == false)
        {
            if (sender.animator.GetCurrentAnimatorStateInfo(0).IsTag(curStateTag))
            {
                underAttackAnimStart = true;
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
        Physics.IgnoreLayerCollision(3, 7, false);
    }
}
