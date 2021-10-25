using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePosRotate : MonoBehaviour
{
    private void Update()
    {
        rotateToPlayer();
    }

    void rotateToPlayer()
    {
        var rot = Quaternion.LookRotation(PlayerCtrl.Instance.tr.position - this.transform.position);
        Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime);
    }
}
