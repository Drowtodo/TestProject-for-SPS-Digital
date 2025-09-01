using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    #region Состояния
    [Header("Состояния")]
    [Tooltip("Рекомендуемое состояние, с которого начаинется сцена")]
    public BeginGameState BeginState;
    [Tooltip("Рекомендуемое состояние, в котором происходит основной геймплей")]
    public MainGameState MainState;
    [Tooltip("Рекомендуемое состояние для реализации \"конца\" сцены")]
    public EndGameState EndState;
    #endregion

    public static GameStateMachine Current { get; private set; }
    /// <summary>
    /// Текущее игровое состояние
    /// </summary>
    public AbstractGameState CurrentState { get; private set; }

    protected virtual void Start()
    {
        if( Current != null )
        {
            Debug.LogError("Уже существует другой экземпляр GameStateMachine!\nТекущий объект будет уничтожен.");
            Destroy(gameObject);
            return;
        }

        Current = this;
        ChangeState(BeginState);
    }

    /// <summary>
    /// Переключение текущего GameState на новое. Из старого автоматически выполняется выход
    /// </summary>
    /// <param name="newState">Новое игровое состояние</param>
    public static void ChangeState(AbstractGameState newState)
    {
        if( newState == null )
        {
            Debug.LogWarning("Попытка перейти в null-state.");
            return;
        }

        if(Current.CurrentState !=null) Current.CurrentState.Exit();

        Debug.Log($"[GameStateMachine] Переход в состояние: {newState.GetType().Name}");

        Current.CurrentState = newState;
        Current.CurrentState.Enter();
    }
}
