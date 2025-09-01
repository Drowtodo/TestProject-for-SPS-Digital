//using Cysharp.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.Events;

//public class GameplayController : MonoBehaviour
//{
//    public static GameplayController Instance { get; private set; }
//    /// <summary>
//    /// Кол-во очков заработанных входе одной игры
//    /// </summary>
//    public EventableValue<int> Coins = new();
//    public int CoinsCount => Coins.Value;
//    [Tooltip("Событие вызывается при изменении кол-ва заработанных пользователем очков")]
//    public UnityEvent<int> OnCoinsChange;

//    public EventableValue<int> Wave = new();
//    public UnityEvent<int> OnWaveChange;

//    [SerializeField]
//    private Player PlayerObj;

//    private Queue<UnitWrapper> EnemysQueue;
//    private List<UnitWrapper> CurrentEnemys = new();
//    [SerializeField]
//    private List<EnemysPosition> EnemysPositions;

//    private bool _isInBattle = false;

//    private void Start()
//    {
//        if (Instance != null)
//        {
//            Debug.LogError("Уже существует другой экземпляр GameplayController!\nТекущий объект будет уничтожен.");
//            Destroy(gameObject);
//            return;
//        }

//        Instance = this;
//        Coins.OnValueChange += (x) => OnCoinsChange?.Invoke(x);
//        Wave.OnValueChange += (x) => OnWaveChange?.Invoke(x); 
//    }

//    /// <summary>
//    /// Используется для начала игры
//    /// </summary>
//    public static async void GameStart()
//    {
//        await Instance.PlayerObj.Init();
//        Instance._isInBattle = true;
//        Instance.Wave.Set(1);
//        Instance.EnemysQueue = new();
//        while(Instance._isInBattle)
//        {
//            int countInWave = Mathf.Clamp(3 + Instance.Wave.Value / 3, 3, EnemyPool.MaximumEnemy);
//            for (int i = 0; i < countInWave; i++)
//            {
//                var enemy = EnemyPool.GetRandom();
//                enemy.OnUnitDie.AddListener(Instance.OnEnemyDie);
//                Instance.EnemysQueue.Enqueue(enemy);
//            }

//            await Instance.FillEmptyEnemyPositions();
//            await Instance.BattleLoop();
//        }
//    }

//    /// <summary>
//    /// Используется для окончания игры
//    /// </summary>
//    public static void GameEnd()
//    {
//        Instance._isInBattle = false;

//        if(Instance.CurrentEnemys.Count > 0)
//        {
//            foreach(var enemy in Instance.CurrentEnemys)
//            {
//                EnemyPool.Return(enemy);
//            }
//            Instance.CurrentEnemys.Clear();
//        }

//        if(Instance.EnemysQueue.Count > 0)
//        {
//            foreach (var enemy in Instance.EnemysQueue)
//            {
//                EnemyPool.Return(enemy);
//            }
//            Instance.EnemysQueue.Clear();
//        }
//    }

//    private async UniTask FillEmptyEnemyPositions()
//    {
//        foreach(var pos in EnemysPositions)
//        {
//            if (pos.IsFree && EnemysQueue.Count > 0)
//            {
//                var enemy = EnemysQueue.Dequeue();
//                CurrentEnemys.Add(enemy);
//                await enemy.MoveTo(pos.transform);
//                pos.Set(enemy);
//            }    
//        }
//    }


//    private async void OnEnemyDie(UnitWrapper unit)
//    {
//        unit.OnUnitDie.RemoveListener(OnEnemyDie);
//        Coins.Set(Coins.Value + unit.Reward);
//        CurrentEnemys.Remove(unit);
//        var pos = EnemysPositions.FirstOrDefault( x => x.CheckEnemy(unit));
//        if (pos != null)
//        {
//            pos.Free();
//        }

//        await FillEmptyEnemyPositions();
//    }

//    private async UniTask BattleLoop()
//    {
//        while(_isInBattle)
//        {
//            await PlayerObj.Attack(CurrentEnemys.Select(x => x.transform).ToList());

//            foreach(var enemy in CurrentEnemys)
//            {
//                await enemy.Attack(new List<Transform>() { PlayerObj.transform });
//            }

//            if(CurrentEnemys.Count == 0 && EnemysQueue.Count == 0)
//            {
//                await UpgradesController.ChooseUpgrade();
//                Wave.Set(Wave.Value + 1);
//                break;
//            }

//            await UniTask.Yield();
//        }
//    }
//}
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance { get; private set; }

    public EventableValue<int> Coins = new();
    public int CoinsCount => Coins.Value;
    public UnityEvent<int> OnCoinsChange;

    public EventableValue<int> Wave = new();
    public UnityEvent<int> OnWaveChange;

    [SerializeField] private Player PlayerObj;

    private Queue<UnitWrapper> EnemysQueue;
    private List<UnitWrapper> CurrentEnemys = new();
    [SerializeField] private List<EnemysPosition> EnemysPositions;

    private bool _isInBattle = false;
    private CancellationTokenSource _battleCts;

    private void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Уже существует другой экземпляр GameplayController!\nТекущий объект будет уничтожен.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Coins.OnValueChange += (x) => OnCoinsChange?.Invoke(x);
        Wave.OnValueChange += (x) => OnWaveChange?.Invoke(x);
    }

    public static async void GameStart()
    {
        Instance._battleCts?.Cancel();
        Instance._battleCts = new CancellationTokenSource();

        try
        {
            await Instance.PlayerObj.Init();
            Instance._isInBattle = true;
            Instance.Wave.Set(1);
            Instance.EnemysQueue = new();

            while (Instance._isInBattle && !Instance._battleCts.Token.IsCancellationRequested)
            {
                int countInWave = Mathf.Clamp(3 + Instance.Wave.Value / 3, 3, EnemyPool.MaximumEnemy);
                for (int i = 0; i < countInWave; i++)
                {
                    var enemy = EnemyPool.GetRandom();
                    enemy.OnUnitDie.AddListener(Instance.OnEnemyDie);
                    Instance.EnemysQueue.Enqueue(enemy);
                }

                await Instance.FillEmptyEnemyPositions(Instance._battleCts.Token);
                await Instance.BattleLoop(Instance._battleCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // тишина — отмена ожидаемая
        }
    }

    public static void GameEnd()
    {
        Instance._isInBattle = false;
        Instance._battleCts?.Cancel();

        if (Instance.CurrentEnemys.Count > 0)
        {
            foreach (var enemy in Instance.CurrentEnemys)
                EnemyPool.Return(enemy);
            Instance.CurrentEnemys.Clear();
        }

        if (Instance.EnemysQueue?.Count > 0)
        {
            foreach (var enemy in Instance.EnemysQueue)
                EnemyPool.Return(enemy);
            Instance.EnemysQueue.Clear();
        }
    }

    private async UniTask FillEmptyEnemyPositions(CancellationToken token)
    {
        foreach (var pos in EnemysPositions)
        {
            token.ThrowIfCancellationRequested();

            if (pos.IsFree && EnemysQueue.Count > 0)
            {
                var enemy = EnemysQueue.Dequeue();
                CurrentEnemys.Add(enemy);
                await enemy.MoveTo(pos.transform).AttachExternalCancellation(token);
                pos.Set(enemy);
            }
        }
    }

    private async void OnEnemyDie(UnitWrapper unit)
    {
        unit.OnUnitDie.RemoveListener(OnEnemyDie);
        Coins.Set(Coins.Value + unit.Reward);
        CurrentEnemys.Remove(unit);

        var pos = EnemysPositions.FirstOrDefault(x => x.CheckEnemy(unit));
        if (pos != null)
            pos.Free();

        if (_battleCts?.IsCancellationRequested == false)
            await FillEmptyEnemyPositions(_battleCts.Token);
    }

    private async UniTask BattleLoop(CancellationToken token)
    {
        while (_isInBattle && !token.IsCancellationRequested)
        {
            token.ThrowIfCancellationRequested();

            await PlayerObj.Attack(CurrentEnemys.Select(x => x.transform).ToList())
                .AttachExternalCancellation(token);

            foreach (var enemy in CurrentEnemys)
            {
                await enemy.Attack(new List<Transform> { PlayerObj.transform })
                    .AttachExternalCancellation(token);
            }

            if (CurrentEnemys.Count == 0 && EnemysQueue.Count == 0)
            {
                await UpgradesController.ChooseUpgrade()
                    .AttachExternalCancellation(token);

                Wave.Set(Wave.Value + 1);
                break;
            }

            await UniTask.Yield(token);
        }
    }
}

