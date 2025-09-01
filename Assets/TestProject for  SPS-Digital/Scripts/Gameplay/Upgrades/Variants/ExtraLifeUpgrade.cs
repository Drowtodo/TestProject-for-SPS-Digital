using UnityEngine;

[CreateAssetMenu(fileName = "ExtraLifeUpgrade", menuName = "Scriptable Objects/Upgrades/ExtraLifeUpgrade")]
public class ExtraLifeUpgrade : AbstractUpgrades
{
    public int Value;
    public override void SetUpgrade(Player player)
    {
        player.UpgradeExtraLife(Value);
    }

}
