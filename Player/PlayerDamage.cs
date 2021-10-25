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

    //����ó��
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

            //���鿡�� �¾Ҵ��� �ĸ鿡�� �¾Ҵ��� üũ
            var dragonLookDir = player.camLookAt.position - Dragon.Instance.dragonTr.position;
            
            //������ ����� �ĸ鿡�� ������ ���� �÷��̾ �巡����ġ���� �־����� �������� �о����Բ� ��
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
                //���鿡�� ������
                player.underAttackDir = true;
            }
            else
            {
                //�Ĺ濡�� ������
                player.underAttackDir = false;
            }

            //������ ó��
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
