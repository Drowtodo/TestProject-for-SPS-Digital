using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractUnitTemplate : ScriptableObject
{
    public int HP;
    public int AttackSpeed;
    public int AttackValue;
    public int CoinDrop;
    public List<AbstractAttack> Attacks;
    public List<Sprite> Sprites;
}
