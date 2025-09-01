using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesController : MonoBehaviour
{
    public static UpgradesController Instance { get; private set; }

    [SerializeField]
    private List<AbstractUpgrades> Upgrades;
    [SerializeField]
    private List<UpgradesPanel> UpgradesPanels;
    [SerializeField]
    private Player PlayerObj;
    [SerializeField]
    private GameObject MainPanel;

    private bool _isChooseRelease = false;

    private void Start()
    {
        if( Instance != null )
        {
            Debug.LogError("Уже существует другой экземпляр UpgradesController!\nТекущий объект будет уничтожен.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static async UniTask ChooseUpgrade()
    {
        if(Instance.UpgradesPanels.Count > Instance.Upgrades.Count)
        {
            Debug.LogError("Недостаточно апгрейдов для всех панелей!");
            return;
        }
        
        List<AbstractUpgrades> availableUpgrades = new(Instance.Upgrades);
        foreach(var panel in Instance.UpgradesPanels)
        {
            int randomIndex = Random.Range(0, availableUpgrades.Count);
            AbstractUpgrades chosenUpgrade = availableUpgrades[randomIndex];
            panel.Init(chosenUpgrade);
            availableUpgrades.RemoveAt(randomIndex);
        }

        Instance.MainPanel.SetActive(true);
        Instance._isChooseRelease = false;
        await UniTask.WaitWhile(() => !Instance._isChooseRelease);
        Instance.MainPanel.SetActive(false);
    }

    public static void ReleaseChoose(AbstractUpgrades upgrade)
    {
        if(!Instance.Upgrades.Contains(upgrade))
        {
            Debug.LogError("В пуле нет запрашиваемого улучшения");
            return;
        }

        if(GameplayController.Instance.CoinsCount < upgrade.Cost)
        {
            return;
        }

        if(upgrade is NewAttackUpgrade)
        {
            Instance.Upgrades.Remove(upgrade);
        }

        upgrade.SetUpgrade(Instance.PlayerObj);
        GameplayController.Instance.Coins.Set(GameplayController.Instance.Coins.Value - upgrade.Cost);
        Instance._isChooseRelease = true;
    }
}
