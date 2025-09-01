using UnityEngine;

public class MainGameState : AbstractGameState
{
    protected override void OnEnter()
    {
    }

    protected override void OnExit()
    {
        GameplayController.GameEnd();
    }
}
