using UnityEngine;

public class EnemysPosition : MonoBehaviour
{
    private UnitWrapper Enemy;
    public bool IsFree { get { return Enemy == null; } }

    public void Set (UnitWrapper enemy)
    {
        Enemy = enemy;
    }

    public void Free()
    {
        Enemy = null;
    }

    public bool CheckEnemy(UnitWrapper enemy)
    {
        return Enemy == enemy;
    }
}
