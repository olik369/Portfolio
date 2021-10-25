using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonIdleState : IState<Dragon>
{
    private int attackType;
    public void OnEnter(Dragon sender)
    {
        sender.SetIdleAnim();
        attackType = Random.Range(1, 3);
        sender.moveAgent.Stop();
    }

    public void OnUpdate(Dragon sender)
    {
        if (sender.attackReady == false) { return; }

        sender.CalDistOnUpdate();
        
        if (sender.distToPlayer > sender.attackDist * sender.attackDist)
        {
            sender.state = Dragon.State.HardTrace;
            sender.ChangeState();
        }

        if (sender.distToPlayer <= sender.attackDist * sender.attackDist)
        {
            //0.3초간 회전 후 공격상태로 전환
            sender.StartCoroutine(RotateForAttack(sender));

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
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {

    }

    IEnumerator RotateForAttack(Dragon sender)
    {
        sender.moveAgent.turnInPlace = true;
        yield return new WaitForSeconds(0.3f);
        sender.moveAgent.turnInPlace = false;
    }
}