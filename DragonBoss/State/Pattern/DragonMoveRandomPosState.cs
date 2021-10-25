using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMoveRandomPosState : IState<Dragon>
{
    private float remainDist;
    private int randomIdx;
    public void OnEnter(Dragon sender)
    {
        randomIdx = Random.Range(0, DragonPatternManager.Instance.RandomPosGroup.Count);

        sender.moveAgent.traceTarget = DragonPatternManager.Instance.RandomPosGroup[randomIdx].position;
        sender.SetMoveAnim();
    }

    public void OnUpdate(Dragon sender)
    {
        remainDist = Vector3.Distance(sender.dragonTr.position, sender.moveAgent.traceTarget);

        if (remainDist < 4f)
        {
            sender.moveAgent.Stop();
            sender.state = Dragon.State.Empty;
            sender.ChangeState();
        }
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {
        sender.moveAgent.Stop();
    }
}