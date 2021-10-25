using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling.Multi;

public class FlyProjectileMoveObjectPool : MonoBehaviour
{
    public float speed;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    private string muzzleName;
    private string hitName;

    public bool collided;
    private Rigidbody rb;

    private bool selfDespawn = true;
    private Coroutine runningCoroutine = null;
    private void OnEnable()
    {
        selfDespawn = true;
        runningCoroutine = StartCoroutine(SelfDespawn());
        rb.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        if(selfDespawn == false)
        {
            StopCoroutine(runningCoroutine);
        }
    }
    
    IEnumerator SelfDespawn()
    {
        yield return new WaitForSeconds(7f);
        PoolManagerMulti.Instance.Despawn(this.gameObject);
    }

    void Awake()
    {
        muzzleName = muzzlePrefab.name;
        hitName = hitPrefab.name;
        rb = GetComponent<Rigidbody>();

        if (muzzlePrefab != null)
        {
            var muzzleVFX = PoolManagerMulti.Instance.Spawn(muzzleName, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                PoolManagerMulti.Instance.Despawn(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                PoolManagerMulti.Instance.Despawn(muzzleVFX, psChild.main.duration);
            }
        }
    }

    void FixedUpdate()
    {
        if (speed != 0 && rb != null)
            rb.position += transform.forward * (speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.tag != "Bullet" && !collided)
        {
            ContactPoint contact = co.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            if (hitPrefab != null)
            {
                var hitVFX = PoolManagerMulti.Instance.Spawn(hitName, pos, rot) as GameObject;

                var ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    PoolManagerMulti.Instance.Despawn(hitVFX, psChild.main.duration);
                }
                else
                    PoolManagerMulti.Instance.Despawn(hitVFX, ps.main.duration);
            }

            StartCoroutine(DestroyParticle(0f));
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        selfDespawn = false;
        PoolManagerMulti.Instance.Despawn(gameObject);
    }
}