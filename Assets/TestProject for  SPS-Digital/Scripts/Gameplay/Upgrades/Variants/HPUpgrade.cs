using UnityEngine;

[CreateAssetMenu(fileName = "HPUpgrade", menuName = "Scriptable Objects/Upgrades/HPUpgrade")]
public class HPUpgrade : AbstractUpgrades
{
    public int Value;
    public override void SetUpgrade(Player player)
    {
        player.UpgradeHP(Value);
    }
}
