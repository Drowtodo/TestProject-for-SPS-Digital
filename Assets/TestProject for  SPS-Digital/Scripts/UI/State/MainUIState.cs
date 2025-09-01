using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainUIState : AbstractUIState
{
    [Tooltip("Картинка используемая для индикации загрузки")]
    public Image GeneratingBar;
    [Tooltip("Заглушка для ожидания клика перед продолжением")]
    public GameObject ClickWaitingPanel;
    [Tooltip("Родительский объект для всех связанных с загрузкой")]
    public GameObject LoadingHandler;
    [Tooltip("Родительский объект для все связанных с основным геймлпеем")]
    public GameObject GameHandler;

    private bool _clickRelease = false;

    public override async void Enter()
    {
        gameObject.SetActive(true);
        LoadingHandler.SetActive(true);
        var progress = new Progress<float>(x => GeneratingBar.fillAmount = x);
        await EnemyPool.Generate(progress); //Генерируем пул объектов с передачей прогресса в полоску загрузки
        ClickWaitingPanel.SetActive(true);
        await UniTask.WaitWhile(() => !_clickRelease);//Как только закнчивается генерация включаем заглушку с ожиданием клика по ней для продолжения
        GameHandler.SetActive(true);
        ClickWaitingPanel.SetActive(false);
        LoadingHandler.SetActive(false);
        GameplayController.GameStart();
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
        _clickRelease = false;
        GameHandler.SetActive(false);
    }

    /// <summary>
    /// Установка флага в true после клика по заглушке
    /// </summary>
    public void ClickRelease()
    {
        _clickRelease = true;
    }
}
