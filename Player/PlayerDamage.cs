using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private readonly string dragonAttackColTag = "DragonAttackCollider";
    private readonly string projectileTag = "Projectile";
    private PlayerCtrl player;

    private void Start()
    {
        player = this.GetComponent<PlayerCtrl>();
    }

    //무적처리
    public bool invincibility
    {
        get
        {
            return (player.state == PlayerCtrl.State.Attack2 ||
                player.state == PlayerCtrl.State.UnderAttack ||
                player.state == PlayerCtrl.State.Dodge);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(dragonAttackColTag))
        {
            if (invincibility == true) { return; }

            //정면에서 맞았는지 후면에서 맞았는지 체크
            var dragonLookDir = player.camLookAt.position - Dragon.Instance.dragonTr.position;
            
            //맞을때 정면과 후면에서 맞을때 각각 플레이어가 드래곤위치에서 멀어지는 방향으로 밀어지게끔 함
            if (Vector3.Dot(player.lookDirection, dragonLookDir) <= 0f)
            {
                player.underAttackDir = true;
                player.tr.forward = Dragon.Instance.dragonTr.position - player.tr.position;
            }
            else
            {
                player.underAttackDir = false;
                player.tr.forward = -(Dragon.Instance.dragonTr.position - player.tr.position);
            }
            player.HP -= Dragon.Instance.attackDamage;

            player.state = PlayerCtrl.State.UnderAttack;
            player.ChangeState();

            //Debug.LogWarning($"Player HP = {player.HP}");
        }
    }

    //Projectile Damage
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(projectileTag))
        {
            if (invincibility == true) { return; }

            if (Vector3.Dot(player.lookDirection, collision.GetContact(0).normal) >= 0f)
            {
                //정면에서 맞을때
                player.underAttackDir = true;
            }
            else
            {
                //후방에서 맞을때
                player.underAttackDir = false;
            }

            //데미지 처리
            switch (Dragon.Instance.state)
            {
                case Dragon.State.FlyAround:
                    player.HP -= Dragon.Instance.flyingProjectileDamage;
                    break;
                case Dragon.State.FireBall:
                    player.HP -= Dragon.Instance.projectileDamage;
                    break;
                default:
                    Debug.LogWarning("Not Exist Damage");
                    break;
            }

            player.state = PlayerCtrl.State.UnderAttack;
            player.ChangeState();
        }
    }
}
