using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonMoveWayPointGroup : Singleton<DragonMoveWayPointGroup>
{
    public List<Transform> wayPoints;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        this.GetComponentsInChildren<Transform>(wayPoints);
        wayPoints.Remove(this.transform);
    }

    public Transform GetPoint(int currentIdx)
    {
        return wayPoints[currentIdx];
    }
}
