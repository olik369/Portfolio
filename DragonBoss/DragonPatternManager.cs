using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pattern
{
    public List<Dragon.State> patternList;
    public bool pattern;
    public float setCoolTime;
    public float curCoolTime;
}

public class DragonPatternManager : Singleton<DragonPatternManager>
{
    [SerializeField]
    public Pattern[] patterns;

    public int curPatternOrder { get; set; }
    public bool patterning { get; set; }
    private bool nextPattern;
    public float nextPatternCoolTime;
    private readonly float nextPatternTime = 10f;

    public GameObject volcanoPrefab;
    public List<Transform> RandomPosGroup;
    public List<Transform> volcanoPosGroup;

    private void Start()
    {
        curPatternOrder = 0;
        nextPattern = false;
        patterning = false;
        nextPatternCoolTime = nextPatternTime;
        
        InitPosGroup();

        for(int i = 0; i < patterns.Length; i++)
        {
            patterns[i].curCoolTime = patterns[i].setCoolTime;
            patterns[i].pattern = false;
        }
    }
    void Update()
    {
        Timer();

        if (Dragon.Instance.state == Dragon.State.Idle && patterning == false && nextPattern == true)
        {
            DoPattern();
        }
    }

    void InitPosGroup()
    {
        var randomPosGroup_tr = this.transform.Find("RandomPosGroup");
        var volcanoPosGroup_tr = this.transform.Find("VolcanoPosGroup");

        randomPosGroup_tr.GetComponentsInChildren<Transform>(RandomPosGroup);
        RandomPosGroup.Remove(randomPosGroup_tr);
        volcanoPosGroup_tr.GetComponentsInChildren<Transform>(volcanoPosGroup);
        volcanoPosGroup.Remove(volcanoPosGroup_tr);
    }

    void DoPattern()
    {
        //다음 패턴간의 간격을 주는 if문과 같이쓰기
        for(int i = 0; i < patterns.Length; i++)
        {
            if (patterns[i].pattern)
            {
                StartCoroutine(_doPattern(patterns[i]));
                return;
            }
        }
    }
    IEnumerator _doPattern(Pattern _pattern)
    {
        patterning = true;
        Dragon.Instance.StopAllCoroutines();
        Dragon.Instance.moveAgent.turnInPlace = false;
        while (curPatternOrder < _pattern.patternList.Count)
        {
            Dragon.Instance.state = _pattern.patternList[curPatternOrder];
            Dragon.Instance.ChangeState();
            yield return new WaitForSeconds(0.1f);
        }
        _pattern.pattern = false;
        patterning = false;
        nextPattern = false;
        curPatternOrder = 0;

        Dragon.Instance.attackReady = true;
        Dragon.Instance.state = Dragon.State.Idle;
        Dragon.Instance.ChangeState();
    }

    void Timer()
    {
        //다음 패턴으로 갈때의 쿨타임 계산
        if (patterning == false && nextPattern == false)
        {
            nextPatternCoolTime -= Time.deltaTime;
            if (nextPatternCoolTime < 0f)
            {
                nextPattern = true;
                nextPatternCoolTime = nextPatternTime;
            }
        }

        //각 패턴마다의 쿨타임 계산
        for(int i = 0; i < patterns.Length; i++)
        {
            if(patterns[i].pattern == false)
            {
                patterns[i].curCoolTime -= Time.deltaTime;
                if (patterns[i].curCoolTime < 0f)
                {
                    patterns[i].pattern = true;
                    patterns[i].curCoolTime = patterns[i].setCoolTime;
                }
            }
        }
    }
}
