using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndUIState : AbstractUIState
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
    /// Закрытие игры
    /// </summary>
    public void CloseGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Перезапуск игрового процесса
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
