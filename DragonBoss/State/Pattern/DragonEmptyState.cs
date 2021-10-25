using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEmptyState : IState<Dragon>
{
    public void OnEnter(Dragon sender)
    {
        DragonPatternManager.Instance.curPatternOrder++;
    }

    public void OnUpdate(Dragon sender)
    {

    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {

    }
}