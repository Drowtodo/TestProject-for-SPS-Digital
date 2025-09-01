using UnityEngine;

[CreateAssetMenu(fileName = "AttackValueUpgrade", menuName = "Scriptable Objects/Upgrades/AttackValueUpgrade")]
public class AttackValueUpgrade : AbstractUpgrades
{
    public int Value;
    public override void SetUpgrade(Player player)
    {
        player.UpgradeAttackValue(Value);
    }
}
