using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonDeathState : IState<Dragon>
{
    private float remainDummyTime = 10f;
    public void OnEnter(Dragon sender)
    {
        sender.SetDeathAnim();
        sender.moveAgent.Stop();
        sender.moveAgent.turnInPlace = false;
        sender.StopAllCoroutines();
    }

    public void OnUpdate(Dragon sender)
    {
        remainDummyTime -= Time.deltaTime;

        if (remainDummyTime < 0f)
        {
            sender.gameObject.SetActive(false);
        }
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {

    }
}
