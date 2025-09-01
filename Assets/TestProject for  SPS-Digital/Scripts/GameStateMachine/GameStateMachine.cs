using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    #region ���������
    [Header("���������")]
    [Tooltip("������������� ���������, � �������� ���������� �����")]
    public BeginGameState BeginState;
    [Tooltip("������������� ���������, � ������� ���������� �������� ��������")]
    public MainGameState MainState;
    [Tooltip("������������� ��������� ��� ���������� \"�����\" �����")]
    public EndGameState EndState;
    #endregion

    public static GameStateMachine Current { get; private set; }
    /// <summary>
    /// ������� ������� ���������
    /// </summary>
    public AbstractGameState CurrentState { get; private set; }

    protected virtual void Start()
    {
        if( Current != null )
        {
            Debug.LogError("��� ���������� ������ ��������� GameStateMachine!\n������� ������ ����� ���������.");
            Destroy(gameObject);
            return;
        }

        Current = this;
        ChangeState(BeginState);
    }

    /// <summary>
    /// ������������ �������� GameState �� �����. �� ������� ������������� ����������� �����
    /// </summary>
    /// <param name="newState">����� ������� ���������</param>
    public static void ChangeState(AbstractGameState newState)
    {
        if( newState == null )
        {
            Debug.LogWarning("������� ������� � null-state.");
            return;
        }

        if(Current.CurrentState !=null) Current.CurrentState.Exit();

        Debug.Log($"[GameStateMachine] ������� � ���������: {newState.GetType().Name}");

        Current.CurrentState = newState;
        Current.CurrentState.Enter();
    }
}
