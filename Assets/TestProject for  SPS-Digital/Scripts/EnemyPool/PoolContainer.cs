using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolContainer : MonoBehaviour
{
    private List<UnitWrapper> _enemys = new();

    /// <summary>
    /// ������� ��������� Circle.
    /// </summary>
    public int FreeCount { get; private set; } = 0;

    /// <summary>
    /// ��������� true, ���� � ���������� ���� ��������� Circle.
    /// </summary>
    public bool HasFree { get { return FreeCount > 0; } }

    /// <summary>
    /// ���������� ������ Circle � ���������.
    /// </summary>
    /// <param name="enemy">����������� Circle</param>
    public void Add(UnitWrapper enemy)
    {
        _enemys.Add(enemy);
        Return(enemy);
    }

    /// <summary>
    /// ������������ ��� ����������� ��� ������������� Circle � ���������.
    /// </summary>
    /// <param name="enemy">������������ Circle</param>
    public void Return(UnitWrapper enemy)
    {
        if(_enemys.Contains(enemy))
        {
            enemy.ReturnToContainer(transform);
            FreeCount++;
        }
    }

    /// <summary>
    /// ���������� true, ���� ������� �������� �� ���������� ��������� Circle.
    /// </summary>
    /// <param name="circle">Circle, � ������� ����� ������� ���� �� ��������� Circle � ����������</param>
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
