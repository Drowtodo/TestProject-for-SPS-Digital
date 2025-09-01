using System.Collections.Generic;
using UnityEngine;

public static class AlliveTargetFilters
{
    public static void GetAlliveTargtes(ref List<Transform> targets)
    {   
        targets.RemoveAll(t =>
            !t.gameObject.activeSelf ||
            !(t.TryGetComponent<IDamagable>(out var dmg) && dmg.CurrentHP.Value > 0));
    }
}
