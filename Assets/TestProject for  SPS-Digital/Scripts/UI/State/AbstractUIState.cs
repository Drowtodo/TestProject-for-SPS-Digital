using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public abstract class AbstractUIState : MonoBehaviour
{
    /// <summary>
    /// Вход в используемое UIState
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// Выход из используемого UIState
    /// </summary>
    public abstract void Exit();
}
