using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Description;
    [SerializeField]
    private Image Icon;
    [SerializeField]
    private TMP_Text Cost;
    private AbstractUpgrades _currentUpgrade;

    public void Init(AbstractUpgrades upgrades)
    {
        _currentUpgrade = upgrades;
        Description.text = upgrades.Description;
        Icon.sprite = upgrades.Icon;
        Cost.text = upgrades.Cost.ToString();
        if(upgrades.Cost > GameplayController.Instance.CoinsCount)
        {
            Cost.color = Color.red;
        }
        else
        {
            Cost.color = Color.green;
        }
    }

    public void OnClick()
    {
        UpgradesController.ReleaseChoose(_currentUpgrade);
    }
}
