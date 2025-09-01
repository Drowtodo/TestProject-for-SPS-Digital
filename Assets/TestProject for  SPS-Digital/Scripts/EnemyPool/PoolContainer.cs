using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolContainer : MonoBehaviour
{
    private List<UnitWrapper> _enemys = new();

    /// <summary>
    /// Счётчик свободных Circle.
    /// </summary>
    public int FreeCount { get; private set; } = 0;

    /// <summary>
    /// Возращает true, если в контейнере есть свободные Circle.
    /// </summary>
    public bool HasFree { get { return FreeCount > 0; } }

    /// <summary>
    /// Добавление нового Circle в контейнер.
    /// </summary>
    /// <param name="enemy">Добавляемый Circle</param>
    public void Add(UnitWrapper enemy)
    {
        _enemys.Add(enemy);
        Return(enemy);
    }

    /// <summary>
    /// Используется для возвращения уже существуещого Circle в контейнер.
    /// </summary>
    /// <param name="enemy">Возвращаемый Circle</param>
    public void Return(UnitWrapper enemy)
    {
        if(_enemys.Contains(enemy))
        {
            enemy.ReturnToContainer(transform);
            FreeCount++;
        }
    }

    /// <summary>
    /// Возвращает true, если удалось получить из контейнера свободный Circle.
    /// </summary>
    /// <param name="circle">Circle, в который будет записан один из свободных Circle в контейнере</param>
    /// <returns></returns>
    public bool TryGet(out UnitWrapper circle)
    {
        if(HasFree)
        {
            circle = _enemys.Where((x) => !x.gameObject.activeSelf).FirstOrDefault();
            circle.gameObject.SetActive(true);
            FreeCount--;
            return true;
        }
        circle = null;
        return false;
    }
}
