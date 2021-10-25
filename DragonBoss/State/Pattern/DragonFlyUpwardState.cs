using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFlyUpwardState : IState<Dragon>
{
    public void OnEnter(Dragon sender)
    {
        sender.moveAgent.Stop();
        sender.SetFlyAnim(0f);
    }

    public void OnUpdate(Dragon sender)
    {
        sender.moveAgent.Fly(sender.flyUpwardSpeed);

        if (sender.moveAgent.altitude < sender.flyAltitude) { return; }

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
