using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ObjectPooling.Multi;
using XftWeapon;

public class DragonFlyAroundState : IState<Dragon>
{
    private float time = 0f;
    private Vector3 center;
    
    public void OnEnter(Dragon sender)
    {
        SetTrailEffect(sender, true);
        sender.StartCoroutine(OccurVolcano(sender));
        sender.moveAgent.Stop();
        sender.SetFlyAnim(1f);

        time = sender.revolutionNum / sender.RPS;
        
        //드래곤 머리 방향 바꾸기
        center = DragonPatternManager.Instance.transform.position;
        center.y = sender.dragonTr.position.y;
        sender.dragonTr.right = center - sender.dragonTr.position;
    }

    public void OnUpdate(Dragon sender)
    {
        if (time > 0f)
        {
            time -= Time.deltaTime;
            sender.dragonTr.RotateAround(center, Vector3.up, 360f * sender.RPS * Time.deltaTime);
            return;
        }

        sender.state = Dragon.State.Empty;
        sender.ChangeState();
    }

    public void OnFixedUpdate(Dragon sender)
    {

    }

    public void OnExit(Dragon sender)
    {
        SetTrailEffect(sender, false);
    }

    IEnumerator OccurVolcano(Dragon sender)
    {
        while(sender.state == Dragon.State.FlyAround)
        {
            CinemachineShake.Instance.ShakeCamera(4f, 0.8f / sender.RPS);
            foreach (var pos in DragonPatternManager.Instance.volcanoPosGroup)
            {
                var volcanoPrefabName = DragonPatternManager.Instance.volcanoPrefab.name;
                var effect = PoolManagerMulti.Instance.Spawn(volcanoPrefabName, pos.position, pos.rotation);
                PoolManagerMulti.Instance.Despawn(effect, 5f);
            }

            yield return new WaitForSeconds(1 / sender.RPS);
        }
    }

    void SetTrailEffect(Dragon sender, bool value)
    {
        sender.flyTrailEffect.SetActive(value);
        
        foreach(var effect in sender.flyTrailEffects)
        {
            effect.gameObject.SetActive(value);
        }
    }
}