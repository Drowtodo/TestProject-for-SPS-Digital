using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField, Tooltip("������ ������������ ��� �������� ������� Enemy �� �����")]
    private GameObject EnemyPrefab;

    [SerializeField, Tooltip("������ ��������� ����������� ��� ��������� ���� ��������")]
    private List<AbstractUnitTemplate> EnemysList;
    [SerializeField, Range(3, 100), Tooltip("���-�� ��������������� �������� ������ ����")]
    private int MaxEnemyInOneType = 9;
    public bool IsGenerated { get; private set; } = false;

    public static int MaximumEnemy { get { return Instance.EnemysList.Count * Instance.MaxEnemyInOneType; } }

    private Dictionary<Type, PoolContainer> _containers;
    private void Start()
    {
        if(Instance != null)
        {
            Debug.LogError("��� ���������� ������ ��������� EnemyPool!\n������� ������ ����� ���������.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _containers = new();
    }

    /// <summary>
    /// ������������ ��� �����������, ���� �� �� ��� ������������ ������.
    /// </summary>
    /// <param name="progress">������ IProgress ������������ ��� �������� ��������� ���������.</param>
    /// <returns></returns>
    public static async UniTask Generate(IProgress<float> progress = null)
    {
        if(Instance.IsGenerated)
        {
            progress?.Report(1f);
            return;
        }

        int maxItem = Instance.EnemysList.Count * Instance.MaxEnemyInOneType;
        for (int j = 0; j < Instance.EnemysList.Count; j++)
        {
            var curCont = new GameObject().AddComponent<PoolContainer>();
            curCont.transform.SetParent(Instance.transform);
            curCont.transform.localPosition = Vector3.zero;
            curCont.name = $"{typeof(PoolContainer).Name}_{Instance.EnemysList[j].name}";
            var type = Instance.EnemysList[j].GetType();
            if (Instance._containers.ContainsKey(type))
            {
                Debug.LogError($"EnemyPool ��� �������� ��������� ��� ���� {type}");
                continue;
            }
            Instance._containers.Add(type, curCont);
            for (int i = 0; i < Instance.MaxEnemyInOneType; i++)
            {
                var enemy = Instantiate(Instance.EnemyPrefab).GetComponent<UnitWrapper>();
                await enemy.Init(Instance.EnemysList[j]);
                curCont.Add(enemy);
                progress?.Report((j * Instance.MaxEnemyInOneType + i + 1) / (float)maxItem);
                await UniTask.Yield();
            }
        }
        Instance.IsGenerated = true;
    }

    /// <summary>
    /// ����� ���������� ��������� ��������� UnitWrapper. ���� ����� ���, �� ���������� null.
    /// </summary>
    /// <returns></returns>
    public static UnitWrapper GetRandom()
    {
        var containers = Instance._containers.Values.Where(x => x.HasFree).ToList();
        while (containers.Count > 0)
        {
            var index = Random.Range(0, containers.Count);
            var cont = containers[index];
            if (cont.TryGet(out UnitWrapper unit))
                return unit;

            containers.RemoveAt(index);
        }
        return null;
    }

    /// <summary>
    /// ������������ ��� ����������� UnitWrapper ������� � ��� ���������.
    /// </summary>
    /// <param name="enemy">UnitWrapper ��� �����������</param>
    public static void Return(UnitWrapper enemy)
    {
        if(enemy != null)
        {
            Instance._containers[enemy.TemplateType].Return(enemy);
        }
    }
}
