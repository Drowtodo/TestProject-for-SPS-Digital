using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CritAttack", menuName = "Scriptable Objects/Attacks/CritAttack")]
public class CritAttack : AbstractAttack
{
    public override async UniTask ReleaseAttack(List<Transform> targets, int additionalDamage, Func<Transform, UniTask> action)
    {
        Damage = additionalDamage;
        await action.Invoke(targets[0]);
    }
}
