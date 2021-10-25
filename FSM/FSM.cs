using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    public IState<T> curState { get; protected set; }

    private T gameObject;

    public FSM(T _gameObject, IState<T> state)
    {
        gameObject = _gameObject;
        curState = state;
    }

    public void SetState(IState<T> state)
    {
        if(gameObject == null)
        {
            Debug.LogWarning("게임 오브젝트가 할당되지 않았습니다!");
            return;
        }
        
        if(curState == state)
        {
            return;
        }

        if(state != null)
        {
            curState.OnExit(gameObject);
        }

        curState = state;

        curState.OnEnter(gameObject);
    }

    public void OnUpdate()
    {
        curState.OnUpdate(gameObject);
    }

    public void OnFixedUpdate()
    {
        curState.OnFixedUpdate(gameObject);
    }
}
