public interface IState<T>
{
    void OnEnter(T sender);
    void OnUpdate(T sender);
    void OnFixedUpdate(T sender);
    void OnExit(T sender);
}