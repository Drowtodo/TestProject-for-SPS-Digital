using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField, Tooltip("Префаб используемый для создания объекта Enemy на сцене")]
    private GameObject EnemyPrefab;

    [SerializeField, Tooltip("Список возможных противников для генерации пула объектов")]
    private List<AbstractUnitTemplate> EnemysList;
    [SerializeField, Range(3, 100), Tooltip("Кол-во сгенерированных объектов одного типа")]
    private int MaxEnemyInOneType = 9;
    public bool IsGenerated { get; private set; } = false;

    public static int MaximumEnemy { get { return Instance.EnemysList.Count * Instance.MaxEnemyInOneType; } }

    private Dictionary<Type, PoolContainer> _containers;
    private void Start()
    {
        if(Instance != null)
        {
            Debug.LogError("Уже существует другой экземпляр EnemyPool!\nТекущий объект будет уничтожен.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _containers = new();
    }

    /// <summary>
    /// Генерируется пул противников, если он не был сгенерирован раньше.
    /// </summary>
    /// <param name="progress">Объект IProgress используется для передачи прогресса генерации.</param>
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
                Debug.LogError($"EnemyPool уже содержит контейнер для типа {type}");
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
    /// Метод возвращает случайный свободный UnitWrapper. Если таких нет, то возвращает null.
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
    /// Используется для возвращения UnitWrapper обратно в пул свободных.
    /// </summary>
    /// <param name="enemy">UnitWrapper для возвращения</param>
    public static void Return(UnitWrapper enemy)
    {
        if(enemy != null)
        {
            Instance._containers[enemy.TemplateType].Return(enemy);
        }
    }
}
