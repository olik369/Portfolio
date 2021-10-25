#undef DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XftWeapon;

public class Dragon : Singleton<Dragon>
{
    public enum State
    {
        Rest,
        Patrol,
        Trace,
        
        Idle,
        Attack1,
        Attack2,
        UnderAttack,
        HardTrace,
        
        //패턴 state
        FireBall,
        MoveRandomPos,
        FlyUpward,
        FlyDownward,
        FlyAround,
        Empty,   //패턴전환 용도
        
        Death,
    }
    public State state = State.Rest;
    public FSM<Dragon> FSM;
    Dictionary<State, IState<Dragon>> DragonState = new Dictionary<State, IState<Dragon>>();

    public void ChangeState()
    {
#if DEBUG
        Debug.Log($"Current Dragon State : {state}");
#endif
        FSM.SetState(DragonState[state]);
    }

    public float distToPlayer { get; set; }
    
    //드래곤보스 스탯값
    public float attackDamage = 5f;
    public float projectileDamage = 10f;
    public float flyingProjectileDamage = 25f;
    public float traceDist = 15f;
    public float attackDist = 5f;
    public float restWaitingTime = 5f;
    public float attackCoolTime = 4f;
    public bool attackReady = true;
    public bool fireProjectileReady { get; set; } = false;

    public float attackExitTime = 0.8f;

    //공중 관련 값
    public float flyAltitude = 40f;
    public float flyUpwardSpeed = 8f;
    public float flyDownwardSpeed = -15f;
    public int revolutionNum = 4;
    public float RPS = 0.5f;    // RPS : revolution / sec
    
    //각종 컴포넌트 값
    [HideInInspector]
    public DragonMoveAgent moveAgent;
    public Transform dragonTr;
    public Animator animator { get; set; }
    public GameObject flyTrailEffect;
    public GameObject attackCollider;
    public XWeaponTrail[] flyTrailEffects { get; set; }

    //애니메이터 해시값
    public readonly int hashMove = Animator.StringToHash("Move");
    public readonly int hashFly = Animator.StringToHash("Fly");
    public readonly int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    public readonly int hashAttackType = Animator.StringToHash("AttackType");
    public readonly int hashGroundAttack = Animator.StringToHash("GroundAttack");
    public readonly int hashIdle = Animator.StringToHash("Idle");
    public readonly int hashDeath = Animator.StringToHash("Death");
    public readonly int hashFlySpeed = Animator.StringToHash("FlySpeed");
    public readonly int hashHit = Animator.StringToHash("Hit");
    public readonly int hashHitType = Animator.StringToHash("HitType");

    public Coroutine runningCoroutine { get; set; }

    private void Start()
    {
        InitFSM();
        InitComponent();

        flyTrailEffect.SetActive(false);
    }

    private void Update()
    {
        FSM.OnUpdate();
        animator.SetFloat(hashMoveSpeed, moveAgent.speed);
    }

    private void FixedUpdate()
    {
        FSM.OnFixedUpdate();
    }

    void InitFSM()
    {
        //기본 몬스터행동
        DragonState.Add(State.Rest, new DragonRestState());
        DragonState.Add(State.Patrol, new DragonPatrolState());
        DragonState.Add(State.Trace, new DragonTraceState());
        
        //전투 시 기본 몬스터행동
        DragonState.Add(State.Attack1, new DragonAttack1State());
        DragonState.Add(State.Attack2, new DragonAttack2State());
        DragonState.Add(State.Idle, new DragonIdleState());
        DragonState.Add(State.HardTrace, new DragonHardTraceState());
        DragonState.Add(State.UnderAttack, new DragonUnderAttackState());
        
        //드래곤 패턴공격 시 행동
        DragonState.Add(State.FireBall, new DragonFireBallState());
        DragonState.Add(State.FlyUpward, new DragonFlyUpwardState());
        DragonState.Add(State.FlyDownward, new DragonFlyDownwardState());
        DragonState.Add(State.FlyAround, new DragonFlyAroundState());
        DragonState.Add(State.MoveRandomPos, new DragonMoveRandomPosState());
        DragonState.Add(State.Empty, new DragonEmptyState());

        DragonState.Add(State.Death, new DragonDeathState());
        
        FSM = new FSM<Dragon>(this, DragonState[State.Rest]);
    }

    void InitComponent()
    {
        dragonTr = this.transform;
        animator = this.GetComponent<Animator>();
        moveAgent = this.GetComponent<DragonMoveAgent>();
        flyTrailEffects = flyTrailEffect.GetComponentsInChildren<XWeaponTrail>();
        attackCollider.SetActive(false);
    }

    public void SetRestAnim()
    {
        animator.SetBool(hashMove, false);
        animator.SetBool(hashFly, false);
        animator.SetBool(hashIdle, false);
    }

    public void SetMoveAnim()
    {
        animator.SetBool(hashMove, true);
        animator.SetBool(hashFly, false);
        animator.SetBool(hashIdle, false);
    }

    public void SetIdleAnim()
    {
        animator.SetBool(hashMove, false);
        animator.SetBool(hashFly, false);
        animator.SetBool(hashIdle, true);
    }

    public void SetFlyAnim(float flySpeed)
    {
        animator.SetBool(hashFly, true);
        animator.SetBool(hashMove, false);
        animator.SetBool(hashIdle, false);
        animator.SetFloat(hashFlySpeed, flySpeed);
    }

    public void SetUnderAttackAnim()
    {
        animator.SetInteger(hashHitType, Random.Range(0, 2));
        animator.SetTrigger(hashHit);
    }

    public void SetDeathAnim()
    {
        animator.SetTrigger(hashDeath);
    }

    public void CalDistToPlayer(float calTime = 0.2f)
    {
        runningCoroutine = StartCoroutine(_calDistToPlayer(calTime));
    }

    IEnumerator _calDistToPlayer(float calTime)
    {
        while (true)
        {
            distToPlayer = Vector3.SqrMagnitude(PlayerCtrl.Instance.tr.position - dragonTr.position);
            yield return new WaitForSeconds(calTime);
        }
    }

    public void StopCalDistToPlayer()
    {
        if(runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }
    
    public void CalDistOnUpdate()
    {
        distToPlayer = Vector3.SqrMagnitude(PlayerCtrl.Instance.tr.position - dragonTr.position);
    }
    
    IEnumerator CalAttackCoolTime(float coolTime)
    {
        attackReady = false;
        yield return new WaitForSeconds(coolTime);
        attackReady = true;
    }

    public void DoGroundAttack(int attackType)
    {
        if(attackType == 1 || attackType == 2)
        {
            StartCoroutine(CalAttackCoolTime(attackCoolTime));
        }
        
        animator.SetInteger(hashAttackType, attackType);
        animator.SetTrigger(hashGroundAttack);
    }

    public bool CheckExitTime(int layerIndex, float exitTime, string currentStateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime > exitTime
                && animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(currentStateName) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //애니메이션 이벤트 매서드
    public void AttackColEnable()
    {
        StartCoroutine(_attackColEnable());
    }

    IEnumerator _attackColEnable()
    {
        attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackCollider.SetActive(false);
    }

    //Gizmo 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(dragonTr.position, traceDist);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dragonTr.position, attackDist);
    }
}