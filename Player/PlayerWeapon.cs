using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling.Multi;

public class PlayerWeapon : MonoBehaviour
{
    public float damage { get; } = 4f;
    public GameObject slashPrefab;
    private string slashPrefabName;
    public Transform slashPos;
    public GameObject attackCollider;

    private void Awake()
    {
        slashPrefabName = slashPrefab.name;
    }

    public void SlashEffect()
    {
        var effect = PoolManagerMulti.Instance.Spawn(slashPrefabName, slashPos.position, slashPos.rotation);
        PoolManagerMulti.Instance.Despawn(effect, 1.0f);
    }
}
