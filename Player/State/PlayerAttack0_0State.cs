using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack0_0State : IState<PlayerCtrl>
{
    bool click;
    private readonly string curState = "Attack0_0";
    public void OnEnter(PlayerCtrl sender)
    {
        sender.DoAttack(PlayerCtrl.State.Attack0_0);
        sender.rotatable = false;
        click = false;
    }

    public void OnUpdate(PlayerCtrl sender)
    {
        if (sender.InputDelaying(0, 0.5f, curState)) { return; }

        if (Input.GetKeyDown(sender.Key_LightAttack))
        {
            sender.SetTrigger(sender.hashCombo);
            click = true;
        }

        if (sender.CheckExitTime(0, sender.atkExitTime, curState))
        {
            if (click) { sender.state = PlayerCtrl.State.Attack0_1; }
            else { sender.state = PlayerCtrl.State.Move; }
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