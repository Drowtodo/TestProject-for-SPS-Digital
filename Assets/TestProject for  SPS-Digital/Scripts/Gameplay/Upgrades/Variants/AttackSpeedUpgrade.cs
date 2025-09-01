using UnityEngine;

[CreateAssetMenu(fileName = "AttackSpeedUpgrade", menuName = "Scriptable Objects/Upgrades/AttackSpeedUpgrade")]
public class AttackSpeedUpgrade : AbstractUpgrades
{
    public int Value;
    public override void SetUpgrade(Player player)
    {
        player.UpgradeAttackSpeed(Value);
    }
}
