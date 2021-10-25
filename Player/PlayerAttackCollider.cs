using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackCollider : MonoBehaviour
{
    public PlayerWeapon weapon;
    private readonly string enemyTag = "Enemy";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            weapon.SlashEffect();
        }
    }
}
