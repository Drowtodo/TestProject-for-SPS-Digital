using UnityEngine;

public abstract class AbstractUpgrades : ScriptableObject
{
    public int Cost;
    public Sprite Icon;
    public string Description;

    public abstract void SetUpgrade(Player player);
}
