using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonUnderAttackState : IState<Dragon>
{
    float time;
    public void OnEnter(Dragon sender)
    {
        sender.SetUnderAttackAnim();
        time = 0.5f;
    }

    public void OnUpdate(Dragon sender)
    {
        time -= Time.deltaTime;
        
        if (time > 0f) { return; }

        if (sender.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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