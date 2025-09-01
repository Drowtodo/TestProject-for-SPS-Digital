using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractGameState : MonoBehaviour
{
    [SerializeField,Tooltip("Используется, если для игрового состояния требуется специфичный UI")]
    protected AbstractUIState UIState;

    [Tooltip("Вызов просиходит по окончанию всех действий при входе в состояние")]
    public UnityEvent OnStateEnter;
    [Tooltip("Вызов просиходит по окончанию всех действий при выходе из состояния")]
    public UnityEvent OnStateExit;

    /// <summary>
    /// Вход в GameState. По окончанию происходит срабатывание события OnStateEnter
    /// </summary>
    public void Enter()
    {
        if(UIState != null) UIState.Enter();
        OnEnter();
        OnStateEnter?.Invoke();
    }

    protected abstract void OnEnter();

    /// <summary>
    /// Вход из GameState. По окончанию происходит срабатывание события OnStateExit
    /// </summary>
    public void Exit()
    {
        if (UIState != null) UIState.Exit();
        OnExit();
        OnStateExit?.Invoke();
    }

    protected abstract void OnExit();
}
