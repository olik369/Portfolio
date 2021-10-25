using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFlyDownwardState : IState<Dragon>
{
    public void OnEnter(Dragon sender)
    {
        sender.moveAgent.Stop();
        sender.SetFlyAnim(0f);
    }

    public void OnUpdate(Dragon sender)
    {
        sender.moveAgent.Fly(sender.flyDownwardSpeed);

        if (sender.moveAgent.altitude >= 0f) { return; }

        sender.state = Dragon.State.Empty;
        sender.ChangeState();
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {
        
    }
}