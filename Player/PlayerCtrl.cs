#undef DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : Singleton<PlayerCtrl>
{
    public enum State
    {
        Move,
        Attack0_0,
        Attack0_1,
        Attack0_2,
        Attack1,
        Attack2,
        UnderAttack,
        Dodge
    }
    public State state = State.Move;
    public FSM<PlayerCtrl> FSM;
    Dictionary<State, IState<PlayerCtrl>> PlayerState = new Dictionary<State, IState<PlayerCtrl>>();

    public void ChangeState()
    {
#if DEBUG
        Debug.Log($"Current State : {state}");
#endif
        FSM.SetState(PlayerState[state]);
    }
    
    //컴포넌트
    public Transform tr { get; set; }
    public Animator animator { get; set; }
    private Camera mainCam;
    private Transform mainCamTr;
    public Transform camLookAt;

    //키코드 설정
    public KeyCode Key_Run = KeyCode.LeftShift;
    public KeyCode Key_Dodge = KeyCode.Space;
    public KeyCode Key_LightAttack = KeyCode.Mouse0;
    public KeyCode Key_HeavyAttack = KeyCode.Mouse1;
    public KeyCode Key_DashAttack = KeyCode.V;

    //이동, 회전, 체력
    public float HP = 100f;
    public Vector3 lookDirection { get; set; }
    private Vector3 moveDir;
    private Vector2 inputMoveKey;
    private float moveSpeed;
    private float rotSpeed = 8.0f;
    private float acceleration;
    public bool rotatable = true;
    public bool underAttackDir { get; set; }    //true : front, false : rear

    //애니메이터 해시값
    public readonly int hashCombo = Animator.StringToHash("Combo");
    public readonly int hashAttack = Animator.StringToHash("Attack");
    public readonly int hashAttackType = Animator.StringToHash("AttackType");
    public readonly int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    public readonly int hashAcceleration = Animator.StringToHash("Acceleration");
    public readonly int hashDamage = Animator.StringToHash("Damage");
    public readonly int hashDamageDirection = Animator.StringToHash("DamageDirection");
    public readonly int hashDodge = Animator.StringToHash("Dodge");

    //공격
    public float atkExitTime = 0.8f;
    public PlayerWeapon playerWeapon;
    private GameObject weaponColl;

    private void Start()
    {
        InitComponent();
        InitFSM();
    }

    private void Update()
    {
        PlayerRotate();
        InputMoveKey();
        FSM.OnUpdate();
    }

    private void FixedUpdate()
    {
        FSM.OnFixedUpdate();
    }

    void InitFSM()
    {
        PlayerState.Add(State.Move, new PlayerMoveState());
        PlayerState.Add(State.Attack0_0, new PlayerAttack0_0State());
        PlayerState.Add(State.Attack0_1, new PlayerAttack0_1State());
        PlayerState.Add(State.Attack0_2, new PlayerAttack0_2State());
        PlayerState.Add(State.Attack1, new PlayerAttack1State());
        PlayerState.Add(State.Attack2, new PlayerAttack2State());
        PlayerState.Add(State.UnderAttack, new PlayerUnderAttackState());
        PlayerState.Add(State.Dodge, new PlayerDodgeState());
        FSM = new FSM<PlayerCtrl>(this, PlayerState[State.Move]);
    }

    void InitComponent()
    {
        tr = this.transform;
        animator = this.GetComponent<Animator>();
        mainCam = Camera.main;
        mainCamTr = mainCam.transform;

        weaponColl = playerWeapon.attackCollider;
        weaponColl.SetActive(false);
    }

    void InputMoveKey()
    {
        inputMoveKey = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(inputMoveKey != Vector2.zero)
        {
            moveSpeed += Time.deltaTime * 2f;
        }
        else
        {
            moveSpeed -= Time.deltaTime * 2f;
        }
        moveSpeed = Mathf.Clamp(moveSpeed, 0f, 1f);

        if (moveSpeed > 0.9f && Input.GetKey(Key_Run))
        {
            acceleration += Time.deltaTime * 4f;
        }
        else
        {
            acceleration -= Time.deltaTime * 4f;
        }

        acceleration = Mathf.Clamp(acceleration, 0f, 1f);
    }

    void PlayerRotate()
    {
        if (rotatable)
        {
            if (moveDir != Vector3.zero)
            {
                var rot = Quaternion.LookRotation(moveDir);
                tr.rotation = Quaternion.Lerp(tr.rotation, rot, rotSpeed * Time.deltaTime);
            }
        }
    }

    public void SetFloatMove()
    {
        var lookForward = new Vector3(mainCamTr.forward.x, 0f, mainCamTr.forward.z).normalized;
        var lookRight = new Vector3(mainCamTr.right.x, 0f, mainCamTr.right.z).normalized;
        moveDir = lookForward * inputMoveKey.y + lookRight * inputMoveKey.x;
        if (moveDir.sqrMagnitude > 0.2f) { lookDirection = moveDir; }

        animator.SetFloat(hashMoveSpeed, moveSpeed);
        animator.SetFloat(hashAcceleration, acceleration);
    }

    public void DoAttack(State _state)
    {
        var type = -1;
        switch (_state)
        {
            case State.Attack0_0:
                type = 0;
                break;
            case State.Attack1:
                type = 1;
                break;
            case State.Attack2:
                type = 2;
                break;
            default:
                Debug.LogWarning("없는 공격상태를 입력했습니다!");
                break;
        }

        if(type == -1)
        {
            return;
        }
        animator.SetTrigger(hashAttack);
        animator.SetInteger(hashAttackType, type);
    }

    public void ResetTrigger(int hashId)
    {
        animator.ResetTrigger(hashId);
    }

    public void SetTrigger(int hashId)
    {
        animator.SetTrigger(hashId);
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

    //콤보공격 시 다음 공격키 입력을 현재 애니메이션이 얼마만큼 지난 후 받을 지 확인하는 매서드
    public bool InputDelaying(int layerIndex, float normalizedTime, string currentStateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(currentStateName) == false)
        {
            return true;
        }
        else
        {
            if (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < normalizedTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //애니메이션 이벤트 메서드
    public void AttackColEnable()
    {
        StartCoroutine(_attackColEnable());
    }

    IEnumerator _attackColEnable()
    {
        weaponColl.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        weaponColl.SetActive(false);
    }
}