using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackUpgrade", menuName = "Scriptable Objects/Upgrades/NewAttackUpgrade")]
public class NewAttackUpgrade : AbstractUpgrades
{
    public AbstractAttack Attack;
    public async override void SetUpgrade(Player player)
    {
        await player.UpgradeAttack(Attack);
    }
}
