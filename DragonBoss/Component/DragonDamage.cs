using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling.Multi;

public class DragonDamage : MonoBehaviour
{
    private float HP = 100f;
    private const string weaponTag = "Weapon";
    private string[] bloodFXName = { "Blood1", "Blood2", "Blood3", "Blood4", "Blood5", "Blood6" };
    RaycastHit hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(weaponTag))
        {
            var beforeHP = HP % 20f;
            ShowBloodEffect();
            HP -= PlayerCtrl.Instance.playerWeapon.damage;

            if (HP > 0f && beforeHP < (HP % 20f) && DragonPatternManager.Instance.patterning == false)
            {
                Dragon.Instance.state = Dragon.State.UnderAttack;
                Dragon.Instance.ChangeState();
            }

            if (HP <= 0f)
            {
                Dragon.Instance.state = Dragon.State.Death;
                Dragon.Instance.ChangeState();
            }
        }
    }

    public void ShowBloodEffect()
    {
        var rayPos = PlayerCtrl.Instance.camLookAt;
        if (Physics.Raycast(rayPos.position, rayPos.forward, out hit, 3f) == false) { return; }

        var angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;
        var rot = Quaternion.Euler(0, angle + 90, 0);
        var index = Random.Range(0, bloodFXName.Length);

        var effect = PoolManagerMulti.Instance.Spawn(bloodFXName[index], hit.point, rot);
        PoolManagerMulti.Instance.Despawn(effect, 4f);
    }
}