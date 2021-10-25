using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling.Multi;

public class DragonFireProjectile : MonoBehaviour
{
    private bool isFireReady = true;
    
    private string firePrefabName;
    public Transform firePos;
    public GameObject firePrefab;
    public float fireDelay = 2f;

    public Transform flyFireCenterPos;
    private string flyFirePrefabName;
    public GameObject flyFirePrefab;
    public float flyFirePosRadius;
    public float flyfireDelay = 2f;
    private Transform flyFirePos;

    private void Start()
    {
        firePrefabName = firePrefab.name;
        flyFirePrefabName = flyFirePrefab.name;
        flyFirePos = flyFireCenterPos.Find("FlyFirePos");
        flyFirePos.position += Vector3.forward * flyFirePosRadius;
    }

    private void Update()
    {
        if (Dragon.Instance.state == Dragon.State.FireBall)
        {
            FirePosRotate(firePos);
            if (Dragon.Instance.fireProjectileReady == true)
            {
                Fire(firePrefabName, firePos, fireDelay);
            }
        }

        if(Dragon.Instance.state == Dragon.State.FlyAround)
        {
            flyFireCenterPos.rotation *= Quaternion.AngleAxis(Random.Range(0, 6) * 60f, Vector3.up);
            FirePosRotate(flyFirePos);
            Fire(flyFirePrefabName, flyFirePos, flyfireDelay);
        }
    }

    void Fire(string prefabName, Transform firePos, float delay)
    {
        if(isFireReady == true)
        {
            PoolManagerMulti.Instance.Spawn(prefabName, firePos.position, firePos.rotation);
            isFireReady = false;

            StartCoroutine(FireDelay(delay));
        }
    }
    
    IEnumerator FireDelay(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        isFireReady = true;
    }

    void FirePosRotate(Transform firePos)
    {
        var rot = Quaternion.LookRotation(PlayerCtrl.Instance.camLookAt.position - firePos.position);
        firePos.rotation = rot * Quaternion.Euler(Vector3.up * Random.Range(-6f, 6f));
    }
}
