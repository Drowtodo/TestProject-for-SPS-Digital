using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseAttack", menuName = "Scriptable Objects/Attacks/BaseAttack")]
public class BaseAttack : AbstractAttack
{
    public override async UniTask ReleaseAttack(List<Transform> targets, int additionalDamage, Func<Transform, UniTask> action)
    {
        await action.Invoke(targets[0]);
    }
}
