using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class AbstractAttack : ScriptableObject
{
    public int Damage;
    public int AttackWeight;
    public Sprite Sprite;
    [Range(0, 100)]
    public int ReleaseChance;
    [Range(1f, 5f)]
    public float Speed;

    public abstract UniTask ReleaseAttack(List<Transform> targets, int additionalDamage,  Func<Transform, UniTask> action);
}
