public class BeginUIState : AbstractUIState
{
    public override void Enter()
    {
        gameObject.SetActive(true);
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Переход в MainGameState
    /// </summary>
    public void MoveToMainState()
    {
        GameStateMachine.ChangeState(GameStateMachine.Current.MainState);
    }
}
