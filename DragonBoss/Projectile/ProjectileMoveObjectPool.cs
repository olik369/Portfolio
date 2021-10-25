using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling.Multi;

public class ProjectileMoveObjectPool : MonoBehaviour
{
    public float speed = 15f;
    public GameObject hit;
    public GameObject flash;
    private Rigidbody rb;

    private string hitName;
    private string flashName;

    private bool selfDespawn = true;
    private Coroutine selfDespawnCoroutine;

    private void Awake()
    {
        hitName = hit.name;
        flashName = flash.name;

        rb = GetComponent<Rigidbody>();
        if (flash != null)
        {
            var flashInstance = PoolManagerMulti.Instance.Spawn(flashName, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
            var flashPs = flashInstance.GetComponent<ParticleSystem>();
            if (flashPs != null)
            {
                PoolManagerMulti.Instance.Despawn(flashInstance, flashPs.main.duration);
            }
            else
            {
                var flashPsParts = flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                PoolManagerMulti.Instance.Despawn(flashInstance, flashPsParts.main.duration);
            }
        }
    }

    private void OnEnable()
    {
        selfDespawn = true;
        selfDespawnCoroutine = StartCoroutine(SelfDespawn());
        rb.angularVelocity = Vector3.zero;
    }

    private void OnDisable()
    {
        if(selfDespawn == false)
        {
            StopCoroutine(SelfDespawn());
        }
    }

    IEnumerator SelfDespawn()
    {
        yield return new WaitForSeconds(7f);
        PoolManagerMulti.Instance.Despawn(this.gameObject);
    }

    void FixedUpdate()
    {
        if (speed != 0)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point + contact.normal;

        if (hit != null)
        {
            var hitInstance = PoolManagerMulti.Instance.Spawn(hitName, pos, rot);
            hitInstance.transform.LookAt(contact.point + contact.normal);

            var hitPs = hitInstance.GetComponent<ParticleSystem>();
            if (hitPs != null)
            {
                PoolManagerMulti.Instance.Despawn(hitInstance, hitPs.main.duration);
            }
            else
            {
                var hitPsParts = hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
                PoolManagerMulti.Instance.Despawn(hitInstance, hitPsParts.main.duration);
            }
        }
        
        selfDespawn = false;
        PoolManagerMulti.Instance.Despawn(gameObject);
    }
}