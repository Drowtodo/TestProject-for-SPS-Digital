using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitAttack", menuName = "Scriptable Objects/Attacks/SplitAttack")]
public class SplitAttack : AbstractAttack
{
    public override async UniTask ReleaseAttack(List<Transform> targets, int additionalDamage, Func<Transform, UniTask> action)
    {
        Damage = 0;
        await action.Invoke(targets[0]);
        Damage = - Mathf.RoundToInt(additionalDamage / 2);
        AlliveTargetFilters.GetAlliveTargtes(ref targets);
        if (targets.Count == 0)
        {
            return;
        }

        if (targets.Count > 1)
        {
            await action.Invoke(targets[1]);
        }
        else
        {
            await action.Invoke(targets[0]);
        }
    }
}
